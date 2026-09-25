// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M2a — QR Code round trips through an independent decoder (ZXing.Net)

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xunit;
using ZXing;

namespace Chuvadi.Barcodes.Encoders.Tests;

public sealed class QrEncoderRoundTripTests
{
    private const string Latin1 = "ISO-8859-1";

    public static TheoryData<int, QrErrorCorrectionLevel> AllVersionsAndLevels()
    {
        TheoryData<int, QrErrorCorrectionLevel> data = [];
        for (int v = 1; v <= 40; v++)
        {
            foreach (QrErrorCorrectionLevel level in Enum.GetValues<QrErrorCorrectionLevel>())
            {
                data.Add(v, level);
            }
        }

        return data;
    }

    [Theory]
    [MemberData(nameof(AllVersionsAndLevels))]
    public void EveryVersionAndLevel_DecodesToTheSameText(int version, QrErrorCorrectionLevel level)
    {
        // Mixed content sized to nearly fill the symbol, exercising segmentation and every block layout.
        Random random = new((version * 10) + (int)level);
        string text = RandomMixedText(random, version, level);
        QrSymbol symbol = QrEncoder.Encode(text, new QrEncodeOptions
        {
            ErrorCorrection = level,
            MinVersion = version,
            MaxVersion = version,
        });

        Result? result = ZxingOracle.DecodeQr(symbol.Matrix, Latin1);

        Assert.NotNull(result);
        Assert.Equal(text, result.Text);
        Assert.Equal(version, symbol.Version);
        Assert.Equal(level.ToString(), result.ResultMetadata[ResultMetadataType.ERROR_CORRECTION_LEVEL]?.ToString());
    }

    [Theory]
    [InlineData("")]
    [InlineData("0")]
    [InlineData("HELLO WORLD")]
    [InlineData("https://github.com/arunshivab/chuvadi-barcodes")]
    [InlineData("Grüße, Ä Ö Ü ß ÿ")]
    public void Latin1Text_RoundTrips(string text)
    {
        QrSymbol symbol = QrEncoder.Encode(text);

        Result? result = ZxingOracle.DecodeQr(symbol.Matrix, Latin1);

        Assert.NotNull(result);
        Assert.Equal(text, result.Text);
    }

    [Theory]
    [InlineData("நன்றி — தமிழ்")]
    [InlineData("😀 emoji 🚑")]
    [InlineData("中文 العربية हिन्दी")]
    public void Utf8TextWithEci_RoundTripsWithoutHints(string text)
    {
        QrSymbol symbol = QrEncoder.Encode(text);

        Result? result = ZxingOracle.DecodeQr(symbol.Matrix);

        Assert.NotNull(result);
        Assert.Equal(text, result.Text);
    }

    [Fact]
    public void ShiftJisKanji_RoundTrips()
    {
        const string Text = "漢字テスト 123 ABC";
        QrSymbol symbol = QrEncoder.Encode(Text, new QrEncodeOptions { CharacterSet = QrCharacterSet.ShiftJis });

        Result? result = ZxingOracle.DecodeQr(symbol.Matrix, "Shift_JIS");

        Assert.NotNull(result);
        Assert.Equal(Text, result.Text);
    }

    [Fact]
    public void Gs1_RoundTripsWithGroupSeparators()
    {
        const string Text = "010950110153000317251231" + "10ABC123\u001D" + "21XYZ%42";
        QrSymbol symbol = QrEncoder.Encode(Text, new QrEncodeOptions { Gs1 = true });

        Result? result = ZxingOracle.DecodeQr(symbol.Matrix, Latin1);

        Assert.NotNull(result);
        Assert.Equal(Text, result.Text);
    }

    [Fact]
    public void StructuredAppend_EveryPartDecodesAndCarriesSequence()
    {
        const string Text = "Structured append splits long data across several linked QR Code symbols.";
        IReadOnlyList<QrSymbol> symbols = QrEncoder.EncodeStructuredAppend(Text, 4);
        IReadOnlyList<string> parts = QrEncoder.SplitForStructuredAppend(Text, 4);

        StringBuilder reassembled = new();
        for (int i = 0; i < symbols.Count; i++)
        {
            Result? result = ZxingOracle.DecodeQr(symbols[i].Matrix, Latin1);
            Assert.NotNull(result);
            Assert.Equal(parts[i], result.Text);
            Assert.Equal(i, symbols[i].StructuredAppendIndex);
            Assert.Equal(4, symbols[i].StructuredAppendCount);
            int sequence = Convert.ToInt32(result.ResultMetadata[ResultMetadataType.STRUCTURED_APPEND_SEQUENCE], CultureInfo.InvariantCulture);
            Assert.Equal((i << 4) | 3, sequence);
            reassembled.Append(result.Text);
        }

        Assert.Equal(Text, reassembled.ToString());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    public void EveryMask_RoundTrips(int mask)
    {
        QrSymbol symbol = QrEncoder.Encode("MASK TEST 0123456789", new QrEncodeOptions { Mask = mask, MinVersion = 7 });

        Result? result = ZxingOracle.DecodeQr(symbol.Matrix, Latin1);

        Assert.NotNull(result);
        Assert.Equal(mask, symbol.Mask);
        Assert.Equal("MASK TEST 0123456789", result.Text);
    }

    [Fact]
    public void BoostErrorCorrection_RaisesLevelWithoutGrowingVersion()
    {
        QrSymbol plain = QrEncoder.Encode("HELLO", new QrEncodeOptions { ErrorCorrection = QrErrorCorrectionLevel.L });
        QrSymbol boosted = QrEncoder.Encode("HELLO", new QrEncodeOptions
        {
            ErrorCorrection = QrErrorCorrectionLevel.L,
            BoostErrorCorrection = true,
        });

        Assert.Equal(plain.Version, boosted.Version);
        Assert.Equal(QrErrorCorrectionLevel.H, boosted.ErrorCorrection);
    }

    [Fact]
    public void TooMuchData_Throws()
    {
        string text = new('x', 3000);

        Assert.Throws<BarcodeEncodingException>(() => QrEncoder.Encode(text, new QrEncodeOptions { ErrorCorrection = QrErrorCorrectionLevel.H }));
    }

    [Fact]
    public void MaximumNumericCapacity_Version40L_Fits()
    {
        string text = new('9', 7089);

        QrSymbol symbol = QrEncoder.Encode(text, new QrEncodeOptions { ErrorCorrection = QrErrorCorrectionLevel.L });

        Assert.Equal(40, symbol.Version);
        Assert.Throws<BarcodeEncodingException>(() => QrEncoder.Encode(text + "9", new QrEncodeOptions { ErrorCorrection = QrErrorCorrectionLevel.L }));
    }

    [Fact]
    public void Latin1CharacterSet_RejectsCharactersAboveU00FF()
    {
        Assert.Throws<BarcodeEncodingException>(() => QrEncoder.Encode("€", new QrEncodeOptions { CharacterSet = QrCharacterSet.Latin1 }));
    }

    private static string RandomMixedText(Random random, int version, QrErrorCorrectionLevel level)
    {
        // Capacity in bytes (byte mode, worst case) minus headroom, then a mix of runs.
        int dataBytes = Qr.QrTables.DataCodewords(version, level);
        int target = Math.Max(1, (dataBytes - 4) * 9 / 10);
        const string Alnum = "ABCDEFGHIJKLMNOPQRSTUVWXYZ $%*+-./:";
        const string Lower = "abcdefghijklmnopqrstuvwxyz,;!?";
        StringBuilder sb = new();
        while (sb.Length < target)
        {
            int run = random.Next(1, 12);
            int kind = random.Next(3);
            for (int i = 0; i < run && sb.Length < target; i++)
            {
                sb.Append(kind switch
                {
                    0 => (char)('0' + random.Next(10)),
                    1 => Alnum[random.Next(Alnum.Length)],
                    _ => Lower[random.Next(Lower.Length)],
                });
            }
        }

        return sb.ToString();
    }
}
