// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M2a — CLI with the encode command (decode arrives in M4)

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using Chuvadi.Barcodes;
using Chuvadi.Barcodes.Encoders;
using Chuvadi.Barcodes.Rendering;

string version = typeof(Program).Assembly
    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "unknown";

if (args.Length == 0 || args[0] is "-h" or "--help" or "help")
{
    PrintUsage(version);
    return 0;
}

if (args[0] is "--version" or "version")
{
    Console.WriteLine($"chuvadi-barcodes {version}");
    return 0;
}

if (args[0] != "encode")
{
    Console.Error.WriteLine($"Unknown command '{args[0]}'. Run 'chuvadi-barcodes --help'.");
    return 2;
}

try
{
    return Encode(ParseOptions(args, 1));
}
catch (Exception ex) when (ex is BarcodeEncodingException or ArgumentException or FormatException or IOException)
{
    Console.Error.WriteLine($"error: {ex.Message}");
    return 1;
}

static int Encode(Dictionary<string, string> o)
{
    string format = Get(o, "format", "qr").ToLowerInvariant();
    string data = o.TryGetValue("data-file", out string? file) ? File.ReadAllText(file) : Require(o, "data");
    string output = Require(o, "out");
    int scale = GetInt(o, "scale", 8);
    int dpi = GetInt(o, "dpi", 0);
    QrCharacterSet charset = Get(o, "charset", "auto").ToLowerInvariant() switch
    {
        "auto" => QrCharacterSet.Auto,
        "latin1" => QrCharacterSet.Latin1,
        "utf8" => QrCharacterSet.Utf8,
        "shiftjis" => QrCharacterSet.ShiftJis,
        string other => throw new ArgumentException($"Unknown charset '{other}'."),
    };

    List<QrSymbol> symbols = [];
    switch (format)
    {
        case "qr":
            {
                QrErrorCorrectionLevel level = Enum.Parse<QrErrorCorrectionLevel>(Get(o, "ecc", "M"), ignoreCase: true);
                int? fixedVersion = o.ContainsKey("version") ? GetInt(o, "version", 0) : null;
                QrEncodeOptions options = new()
                {
                    ErrorCorrection = level,
                    MinVersion = fixedVersion ?? 1,
                    MaxVersion = fixedVersion ?? 40,
                    Mask = o.ContainsKey("mask") ? GetInt(o, "mask", 0) : null,
                    CharacterSet = charset,
                    EmitEci = !o.ContainsKey("no-eci"),
                    Gs1 = o.ContainsKey("gs1"),
                    BoostErrorCorrection = o.ContainsKey("boost"),
                };
                if (o.ContainsKey("structured-append"))
                {
                    symbols.AddRange(QrEncoder.EncodeStructuredAppend(data, GetInt(o, "structured-append", 2), options));
                }
                else
                {
                    symbols.Add(QrEncoder.Encode(data, options));
                }

                break;
            }

        case "microqr":
            {
                QrErrorCorrectionLevel level = Enum.Parse<QrErrorCorrectionLevel>(Get(o, "ecc", "L"), ignoreCase: true);
                int? fixedVersion = o.ContainsKey("version") ? GetInt(o, "version", 0) : null;
                symbols.Add(MicroQrEncoder.Encode(data, new MicroQrEncodeOptions
                {
                    ErrorCorrection = level,
                    MinVersion = fixedVersion ?? 1,
                    MaxVersion = fixedVersion ?? 4,
                    Mask = o.ContainsKey("mask") ? GetInt(o, "mask", 0) : null,
                    CharacterSet = charset,
                }));
                break;
            }

        default:
            throw new ArgumentException($"Format '{format}' is not implemented yet. Available: qr, microqr.");
    }

    for (int i = 0; i < symbols.Count; i++)
    {
        QrSymbol symbol = symbols[i];
        string path = symbols.Count == 1 ? output : NumberedPath(output, i + 1);
        int quiet = o.ContainsKey("quiet") ? GetInt(o, "quiet", 4) : symbol.QuietZone;
        string extension = Path.GetExtension(path).ToLowerInvariant();
        if (extension == ".svg")
        {
            File.WriteAllText(path, SvgRenderer.Render(symbol.Matrix, new SvgRenderOptions
            {
                ModuleSize = scale,
                QuietZone = quiet,
            }));
        }
        else if (extension == ".png")
        {
            File.WriteAllBytes(path, PngRenderer.Render(symbol.Matrix, new RasterRenderOptions
            {
                ModuleSize = scale,
                QuietZone = quiet,
                Dpi = dpi,
            }));
        }
        else
        {
            throw new ArgumentException("Output file must end in .svg or .png.");
        }

        string name = symbol.Format == BarcodeFormat.MicroQrCode ? $"M{symbol.Version}" : symbol.Version.ToString(CultureInfo.InvariantCulture);
        string sequence = symbol.StructuredAppendCount > 0
            ? $", part {symbol.StructuredAppendIndex + 1}/{symbol.StructuredAppendCount}"
            : string.Empty;
        Console.WriteLine(
            $"{path}: {symbol.Format} version {name}, level {symbol.ErrorCorrection}, mask {symbol.Mask}, " +
            $"{symbol.Matrix.Width}x{symbol.Matrix.Height} modules{sequence}");
    }

    return 0;
}

static Dictionary<string, string> ParseOptions(string[] args, int start)
{
    Dictionary<string, string> result = new(StringComparer.Ordinal);
    for (int i = start; i < args.Length; i++)
    {
        string arg = args[i];
        if (!arg.StartsWith("--", StringComparison.Ordinal))
        {
            throw new ArgumentException($"Unexpected argument '{arg}'.");
        }

        string key = arg[2..];
        bool isFlag = key is "gs1" or "boost" or "no-eci";
        if (isFlag)
        {
            result[key] = "true";
        }
        else if (i + 1 < args.Length)
        {
            result[key] = args[++i];
        }
        else
        {
            throw new ArgumentException($"Option '{arg}' needs a value.");
        }
    }

    return result;
}

static string Get(Dictionary<string, string> o, string key, string fallback) =>
    o.TryGetValue(key, out string? value) ? value : fallback;

static string Require(Dictionary<string, string> o, string key) =>
    o.TryGetValue(key, out string? value) ? value : throw new ArgumentException($"Missing --{key}.");

static int GetInt(Dictionary<string, string> o, string key, int fallback) =>
    o.TryGetValue(key, out string? value) ? int.Parse(value, CultureInfo.InvariantCulture) : fallback;

static string NumberedPath(string path, int n)
{
    string directory = Path.GetDirectoryName(path) ?? string.Empty;
    string name = Path.GetFileNameWithoutExtension(path);
    return Path.Combine(directory, $"{name}-{n}{Path.GetExtension(path)}");
}

static void PrintUsage(string version)
{
    Console.WriteLine($"chuvadi-barcodes {version}");
    Console.WriteLine();
    Console.WriteLine("Usage:");
    Console.WriteLine("  chuvadi-barcodes encode --format qr|microqr --data TEXT --out FILE.svg|FILE.png [options]");
    Console.WriteLine();
    Console.WriteLine("Options:");
    Console.WriteLine("  --data-file PATH           read the data from a UTF-8 file instead of --data");
    Console.WriteLine("  --ecc L|M|Q|H              error correction level (QR default M, Micro QR default L)");
    Console.WriteLine("  --version N                force a version (QR 1-40, Micro QR 1-4)");
    Console.WriteLine("  --mask N                   force a mask (QR 0-7, Micro QR 0-3)");
    Console.WriteLine("  --charset auto|latin1|utf8|shiftjis");
    Console.WriteLine("  --no-eci                   do not emit ECI 26 for UTF-8 data");
    Console.WriteLine("  --gs1                      GS1 QR Code (use U+001D between variable-length fields)");
    Console.WriteLine("  --boost                    raise error correction while keeping the version");
    Console.WriteLine("  --structured-append N      split across N linked QR symbols (writes FILE-1, FILE-2, ...)");
    Console.WriteLine("  --scale N                  pixels (PNG) or units (SVG) per module, default 8");
    Console.WriteLine("  --quiet N                  quiet zone in modules (default 4 for QR, 2 for Micro QR)");
    Console.WriteLine("  --dpi N                    resolution recorded in PNG files");
    Console.WriteLine();
    Console.WriteLine("Decoding arrives in milestone M4.");
}
