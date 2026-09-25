// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M2a — QR / Micro QR module-for-module comparison against Zint golden fixtures

using System.Collections.Generic;
using Xunit;

namespace Chuvadi.Barcodes.Encoders.Tests;

public sealed class QrEncoderZintTests
{
    public static TheoryData<string> QrFixtures => ZintFixture.Names("qr");

    public static TheoryData<string> MicroQrFixtures => ZintFixture.Names("microqr");

    [Theory]
    [MemberData(nameof(QrFixtures))]
    public void QrCode_MatchesZintModuleForModule(string name)
    {
        ZintFixture fixture = ZintFixture.Load("qr", name);
        QrEncodeOptions options = new()
        {
            ErrorCorrection = fixture.Ecc,
            Mask = fixture.Mask,
            MinVersion = fixture.Version ?? 1,
            MaxVersion = fixture.Version ?? 40,
            CharacterSet = fixture.CharacterSet,
            Gs1 = fixture.Gs1,
        };

        QrSymbol symbol;
        if (fixture.StructuredAppendCount > 0)
        {
            IReadOnlyList<QrSymbol> symbols = QrEncoder.EncodeStructuredAppend(
                fixture.StructuredAppendFullText, fixture.StructuredAppendCount, options);
            symbol = symbols[fixture.StructuredAppendIndex];
        }
        else
        {
            symbol = QrEncoder.Encode(fixture.Input, options);
        }

        Assert.Equal(fixture.Matrix.ToString(), symbol.Matrix.ToString());
    }

    [Theory]
    [MemberData(nameof(MicroQrFixtures))]
    public void MicroQr_MatchesZintModuleForModule(string name)
    {
        ZintFixture fixture = ZintFixture.Load("microqr", name);
        MicroQrEncodeOptions options = new()
        {
            ErrorCorrection = fixture.Ecc,
            Mask = fixture.Mask,
            MinVersion = fixture.Version ?? 1,
            MaxVersion = fixture.Version ?? 4,
            CharacterSet = fixture.CharacterSet,
        };

        QrSymbol symbol = MicroQrEncoder.Encode(fixture.Input, options);

        Assert.Equal(fixture.Matrix.ToString(), symbol.Matrix.ToString());
    }
}
