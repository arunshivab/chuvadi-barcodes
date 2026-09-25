#!/usr/bin/env python3
"""
Generate golden encoder fixtures with Zint (https://zint.org.uk, BSD-3-Clause).

Zint is a TEST ORACLE ONLY. It is never referenced by, linked into, or shipped with any
Chuvadi.Barcodes package. The fixtures it produces are committed so CI does not need Zint.

Usage (requires the `zint` command on PATH; fixtures were generated with Zint 2.13.0):
    python3 tools/gen_zint_fixtures.py

Output: tests/Chuvadi.Barcodes.Encoders.Tests/Fixtures/Zint/<symbology>/<case>.txt

Fixture format (UTF-8, LF):
    # comment lines (the exact zint command)
    key: value            (data-hex = zint input; input-hex = Chuvadi input when it
                           differs, e.g. GS1; ecc, version, mask, charset, eci, gs1,
                           sa-index, sa-count, sa-parity, sa-full-hex)
    matrix:
    0101...               one line per row, 1 = dark
"""

from __future__ import annotations

import shutil
import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parent.parent
OUT = REPO / "tests" / "Chuvadi.Barcodes.Encoders.Tests" / "Fixtures" / "Zint"

LEVELS = "LMQH"
ECC_PER_BLOCK = [
    [-1, 7, 10, 15, 20, 26, 18, 20, 24, 30, 18, 20, 24, 26, 30, 22, 24, 28, 30, 28, 28, 28, 28, 30, 30, 26, 28, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30],
    [-1, 10, 16, 26, 18, 24, 16, 18, 22, 22, 26, 30, 22, 22, 24, 24, 28, 28, 26, 26, 26, 26, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28],
    [-1, 13, 22, 18, 26, 18, 24, 18, 22, 20, 24, 28, 26, 24, 20, 30, 24, 28, 28, 26, 30, 28, 30, 30, 30, 30, 28, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30],
    [-1, 17, 28, 22, 16, 22, 28, 26, 26, 24, 28, 24, 28, 22, 24, 24, 30, 28, 28, 26, 28, 30, 24, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30],
]
BLOCKS = [
    [-1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 4, 4, 4, 4, 4, 6, 6, 6, 6, 7, 8, 8, 9, 9, 10, 12, 12, 12, 13, 14, 15, 16, 17, 18, 19, 19, 20, 21, 22, 24, 25],
    [-1, 1, 1, 1, 2, 2, 4, 4, 4, 5, 5, 5, 8, 9, 9, 10, 10, 11, 13, 14, 16, 17, 17, 18, 20, 21, 23, 25, 26, 28, 29, 31, 33, 35, 37, 38, 40, 43, 45, 47, 49],
    [-1, 1, 1, 2, 2, 4, 4, 6, 6, 8, 8, 8, 10, 12, 16, 12, 17, 16, 18, 21, 20, 23, 23, 25, 27, 29, 34, 34, 35, 38, 40, 43, 45, 48, 51, 53, 56, 59, 62, 65, 68],
    [-1, 1, 1, 2, 4, 4, 4, 5, 6, 8, 8, 11, 11, 16, 16, 18, 16, 19, 21, 25, 25, 25, 34, 30, 32, 35, 37, 40, 42, 45, 48, 51, 54, 57, 60, 63, 66, 70, 74, 77, 81],
]


def raw_modules(v: int) -> int:
    r = (16 * v + 128) * v + 64
    if v >= 2:
        n = v // 7 + 2
        r -= (25 * n - 10) * n - 55
        if v >= 7:
            r -= 36
    return r


def data_codewords(v: int, lvl: int) -> int:
    return raw_modules(v) // 8 - ECC_PER_BLOCK[lvl][v] * BLOCKS[lvl][v]


def run_zint(args: list[str], data: str) -> list[str]:
    cmd = ["zint", *args, "--dump", "-d", data]
    res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8")
    if res.returncode != 0:
        raise RuntimeError(f"zint failed: {' '.join(cmd)}\n{res.stderr}")
    rows = [line.replace(" ", "") for line in res.stdout.splitlines() if line.strip()]
    size = len(rows)  # QR and Micro QR are square
    out = []
    for r in rows:
        bits = bin(int(r, 16))[2:].zfill(len(r) * 4)
        out.append(bits[:size])
    return out


def write_case(sym: str, name: str, zint_args: list[str], data: str, meta: dict[str, str]) -> None:
    matrix = run_zint(zint_args, data)
    d = OUT / sym
    d.mkdir(parents=True, exist_ok=True)
    lines = [f"# zint {' '.join(zint_args)} --dump -d <data>"]
    lines.append(f"data-hex: {data.encode('utf-8').hex()}")
    for k, v in meta.items():
        if k == "input-hex":
            v = v.encode("utf-8").hex()
        lines.append(f"{k}: {v}")
    lines.append("matrix:")
    lines.extend(matrix)
    (d / f"{name}.txt").write_text("\n".join(lines) + "\n", encoding="utf-8", newline="\n")


def qr_cases() -> None:
    alpha = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"
    # One case per version, cycling level and mask, byte data filling ~75% of capacity.
    for v in range(1, 41):
        lvl = (v - 1) % 4
        mask = (v - 1) % 8
        header = 4 + (8 if v <= 9 else 16)
        n = max(1, (data_codewords(v, lvl) * 8 - header) // 8 * 3 // 4)
        data = "".join("abcdefghijklmnopqrstuvwxyz"[(i * 7) % 26] for i in range(n))
        write_case("qr", f"v{v:02d}-{LEVELS[lvl]}-m{mask}-byte",
                   ["-b", "QRCODE", f"--secure={lvl + 1}", f"--vers={v}", f"--mask={mask}"], data,
                   {"ecc": LEVELS[lvl], "version": str(v), "mask": str(mask), "charset": "Latin1"})

    # Mode and feature cases: automatic version, fixed mask.
    cases = [
        ("numeric", "01234567890123456789012345678901234567", "M", 2, {}),
        ("alphanumeric", "HELLO WORLD $%*+-./: 0123456789 " + alpha, "Q", 3, {}),
        ("hello-world-1m", "HELLO WORLD", "M", 0, {}),
        ("mixed-modes", "ABC12345678901234567890def ghi 9876543210 XYZ", "L", 5, {}),
        ("latin1", "Grüße aus Pune — Ä Ö Ü ß é".replace("—", "-"), "M", 1, {}),
        ("long-numeric-v10", "7" * 400, "H", 6, {}),
        ("long-alnum-v27", ("CHUVADI BARCODES " * 60)[:1000], "L", 4, {}),
    ]
    for name, data, lvl, mask, extra in cases:
        li = LEVELS.index(lvl)
        write_case("qr", name, ["-b", "QRCODE", f"--secure={li + 1}", f"--mask={mask}"], data,
                   {"ecc": lvl, "mask": str(mask), "charset": "Latin1", **extra})

    # UTF-8 with ECI 26.
    write_case("qr", "utf8-eci26", ["-b", "QRCODE", "--secure=2", "--mask=3", "--eci=26"],
               "நன்றி — தமிழ் 😀", {"ecc": "M", "mask": "3", "charset": "Utf8", "eci": "26"})

    # Shift JIS / Kanji mode.
    write_case("qr", "kanji", ["-b", "QRCODE", "--secure=2", "--mask=2"], "漢字テスト１２３",
               {"ecc": "M", "mask": "2", "charset": "ShiftJis"})
    write_case("qr", "kanji-mixed", ["-b", "QRCODE", "--secure=1", "--mask=7"], "点茗ABC123漢字",
               {"ecc": "L", "mask": "7", "charset": "ShiftJis"})

    # GS1 QR (FNC1 first position). Our input uses GS (U+001D) after variable-length AIs.
    write_case("qr", "gs1", ["-b", "QRCODE", "--secure=2", "--mask=1", "--gs1"],
               "[01]09501101530003[17]251231[10]ABC123[21]XYZ%42",
               {"ecc": "M", "mask": "1", "charset": "Latin1", "gs1": "1",
                 "input-hex": "010950110153000317251231" "10ABC123" "\x1d" "21XYZ%42"})

    # Structured append: 3 parts, parity supplied as the ID.
    full = "Chuvadi Barcodes structured append test 0123456789"
    parts = split_even(full, 3)
    parity = 0
    for b in full.encode("latin-1"):
        parity ^= b
    for i, part in enumerate(parts):
        write_case("qr", f"structured-append-{i + 1}of3",
                   ["-b", "QRCODE", "--secure=2", "--mask=4", f"--structapp={i + 1},3,{parity}"], part,
                   {"ecc": "M", "mask": "4", "charset": "Latin1", "sa-index": str(i), "sa-count": "3",
                    "sa-parity": str(parity), "sa-full-hex": full.encode("utf-8").hex()})


def qr_auto_mask_cases() -> None:
    # Automatic mask selection must match Zint's choice (ISO penalty rules).
    import random
    rnd = random.Random(20260925)
    samples = ["HELLO WORLD", "Chuvadi", "https://github.com/arunshivab/chuvadi-barcodes", "0123456789"]
    alphabet = "abcdefghijklmnopqrstuvwxyz0123456789 "
    samples += ["".join(rnd.choice(alphabet) for _ in range(rnd.randint(1, 300))) for _ in range(28)]
    for i, data in enumerate(samples):
        lvl = i % 4
        write_case("qr", f"auto-mask-{i:02d}", ["-b", "QRCODE", f"--secure={lvl + 1}"], data,
                   {"ecc": LEVELS[lvl], "mask": "auto", "charset": "Latin1"})


def split_even(text: str, count: int) -> list[str]:
    base, extra = divmod(len(text), count)
    parts, start = [], 0
    for i in range(count):
        n = base + (1 if i < extra else 0)
        parts.append(text[start:start + n])
        start += n
    return parts


def microqr_cases() -> None:
    combos = [(1, "L"), (2, "L"), (2, "M"), (3, "L"), (3, "M"), (4, "L"), (4, "M"), (4, "Q")]
    datas = {1: "12345", 2: "AB12", 3: "Pune 1", 4: "Pune 42"}
    for v, lvl in combos:
        for mask in range(4):
            li = LEVELS.index(lvl)
            write_case("microqr", f"m{v}-{lvl}-m{mask}",
                       ["-b", "MICROQR", f"--secure={li + 1}", f"--vers={v}", f"--mask={mask}"], datas[v],
                       {"ecc": lvl, "version": str(v), "mask": str(mask), "charset": "Latin1"})
    for name, data, lvl in [("auto-numeric", "0123456789012345678901234567890123", "L"),
                            ("auto-alnum", "HELLO MICRO QR", "M"),
                            ("auto-byte", "abc", "L"),
                            ("capacity-m4-l-numeric", "1" * 35, "L")]:
        li = LEVELS.index(lvl)
        write_case("microqr", name, ["-b", "MICROQR", f"--secure={li + 1}", "--mask=0"], data,
                   {"ecc": lvl, "mask": "0", "charset": "Latin1"})
    write_case("microqr", "kanji", ["-b", "MICROQR", "--secure=1", "--mask=1"], "漢字",
               {"ecc": "L", "mask": "1", "charset": "ShiftJis"})


def microqr_auto_mask_cases() -> None:
    samples = [("12345", "L"), ("AB12", "M"), ("Pune 1", "L"), ("Pune 42", "Q"), ("HELLO MICRO QR", "M"),
               ("abc", "L"), ("99999999", "M"), ("MQR", "L")]
    for i, (data, lvl) in enumerate(samples):
        li = LEVELS.index(lvl)
        write_case("microqr", f"auto-mask-{i:02d}", ["-b", "MICROQR", f"--secure={li + 1}"], data,
                   {"ecc": lvl, "mask": "auto", "charset": "Latin1"})


def main() -> int:
    if shutil.which("zint") is None:
        print("error: zint not found on PATH", file=sys.stderr)
        return 1
    # Remove old fixture folders but keep OUT/README.md.
    for sub in ("qr", "microqr"):
        if (OUT / sub).exists():
            shutil.rmtree(OUT / sub)
    qr_cases()
    qr_auto_mask_cases()
    microqr_cases()
    microqr_auto_mask_cases()
    count = sum(1 for _ in OUT.rglob("*.txt"))
    print(f"Wrote {count} fixtures to {OUT.relative_to(REPO)}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
