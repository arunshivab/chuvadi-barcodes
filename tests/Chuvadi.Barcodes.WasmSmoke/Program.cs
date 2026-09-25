// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M2a — WASM smoke test: encode QR and Micro QR, render SVG and PNG in the browser sandbox.

using System;
using Chuvadi.Barcodes.Encoders;
using Chuvadi.Barcodes.Rendering;

QrSymbol qr = QrEncoder.Encode("Chuvadi Barcodes on WebAssembly");
string svg = SvgRenderer.Render(qr.Matrix);
byte[] png = PngRenderer.Render(qr.Matrix);
Console.WriteLine($"QR version {qr.Version}: SVG {svg.Length} chars, PNG {png.Length} bytes");

QrSymbol micro = MicroQrEncoder.Encode("WASM 1");
Console.WriteLine($"Micro QR M{micro.Version}: {micro.Matrix.Width}x{micro.Matrix.Height}");

return 0;
