// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M1 — scaffold smoke test (replaced by real tests from M2 onwards)

using System.Reflection;
using Xunit;

namespace Chuvadi.Barcodes.Decoders.Tests;

public sealed class AssemblySmokeTests
{
    [Theory]
    [InlineData("Chuvadi.Barcodes.Decoders")]
    public void ReferencedLibrary_LoadsByName(string assemblyName)
    {
        Assembly assembly = Assembly.Load(assemblyName);

        Assert.Equal(assemblyName, assembly.GetName().Name);
    }
}
