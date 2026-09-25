// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M1 — scaffold smoke test (replaced by real tests from M2 onwards)

using System.Reflection;
using Xunit;

namespace Chuvadi.Barcodes.Conformance.Tests;

public sealed class AssemblySmokeTests
{
    [Theory]
    [InlineData("Chuvadi.Barcodes")]
    [InlineData("Chuvadi.Barcodes.Encoders")]
    [InlineData("Chuvadi.Barcodes.Decoders")]
    [InlineData("Chuvadi.Barcodes.Imaging")]
    [InlineData("Chuvadi.Barcodes.Rendering")]
    [InlineData("Chuvadi.Barcodes.Payloads")]
    public void ReferencedLibrary_LoadsByName(string assemblyName)
    {
        Assembly assembly = Assembly.Load(assemblyName);

        Assert.Equal(assemblyName, assembly.GetName().Name);
    }
}
