// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M1 — scaffold. Encode commands arrive in M2, decode commands in M4.

using System;
using System.Reflection;

string version = typeof(Program).Assembly
    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "unknown";

Console.WriteLine($"chuvadi-barcodes {version}");
Console.WriteLine("Scaffold build. Encode commands arrive in milestone M2, decode commands in M4.");
return 0;
