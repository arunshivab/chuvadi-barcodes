// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M1 — WASM smoke test. Grows into encode -> render -> decode in M2/M4.

using System;
using System.Reflection;

string[] libraries =
[
    "Chuvadi.Barcodes",
    "Chuvadi.Barcodes.Encoders",
    "Chuvadi.Barcodes.Decoders",
    "Chuvadi.Barcodes.Imaging",
    "Chuvadi.Barcodes.Rendering",
    "Chuvadi.Barcodes.Payloads",
];

foreach (string library in libraries)
{
    Assembly assembly = Assembly.Load(library);
    Console.WriteLine($"Loaded {assembly.GetName().Name}");
}

return 0;
