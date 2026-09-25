// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M2a — core types

namespace Chuvadi.Barcodes;

/// <summary>
/// Every barcode symbology known to Chuvadi Barcodes. A format being listed here does not
/// mean it is implemented yet; see the project README for the current coverage.
/// </summary>
public enum BarcodeFormat
{
    /// <summary>QR Code Model 2 (ISO/IEC 18004), versions 1 to 40.</summary>
    QrCode,

    /// <summary>QR Code Model 1 (obsolete; decode only).</summary>
    QrCodeModel1,

    /// <summary>Micro QR Code, versions M1 to M4.</summary>
    MicroQrCode,

    /// <summary>Rectangular Micro QR Code (rMQR, ISO/IEC 23941).</summary>
    RectangularMicroQrCode,

    /// <summary>Data Matrix ECC 200 (ISO/IEC 16022).</summary>
    DataMatrix,

    /// <summary>PDF417 (ISO/IEC 15438).</summary>
    Pdf417,

    /// <summary>MicroPDF417.</summary>
    MicroPdf417,

    /// <summary>Aztec Code (ISO/IEC 24778), including compact and rune symbols.</summary>
    Aztec,

    /// <summary>MaxiCode (ISO/IEC 16023).</summary>
    MaxiCode,

    /// <summary>Han Xin Code (ISO/IEC 20830).</summary>
    HanXin,

    /// <summary>DotCode (AIM ISS DotCode).</summary>
    DotCode,

    /// <summary>Codablock F stacked symbology.</summary>
    CodablockF,

    /// <summary>Code 16K stacked symbology.</summary>
    Code16K,

    /// <summary>Code 49 stacked symbology.</summary>
    Code49,

    /// <summary>Code 128, including GS1-128.</summary>
    Code128,

    /// <summary>Code 39, standard and Full ASCII.</summary>
    Code39,

    /// <summary>Code 93, standard and Full ASCII.</summary>
    Code93,

    /// <summary>Codabar.</summary>
    Codabar,

    /// <summary>EAN-13.</summary>
    Ean13,

    /// <summary>EAN-8.</summary>
    Ean8,

    /// <summary>UPC-A.</summary>
    UpcA,

    /// <summary>UPC-E.</summary>
    UpcE,

    /// <summary>Interleaved 2 of 5.</summary>
    Itf,

    /// <summary>ITF-14 (Interleaved 2 of 5 with bearer bars, GTIN-14).</summary>
    Itf14,

    /// <summary>GS1 DataBar Omnidirectional.</summary>
    DataBarOmnidirectional,

    /// <summary>GS1 DataBar Truncated.</summary>
    DataBarTruncated,

    /// <summary>GS1 DataBar Stacked.</summary>
    DataBarStacked,

    /// <summary>GS1 DataBar Stacked Omnidirectional.</summary>
    DataBarStackedOmnidirectional,

    /// <summary>GS1 DataBar Limited.</summary>
    DataBarLimited,

    /// <summary>GS1 DataBar Expanded.</summary>
    DataBarExpanded,

    /// <summary>GS1 DataBar Expanded Stacked.</summary>
    DataBarExpandedStacked,

    /// <summary>GS1 Composite (a linear component with a CC-A, CC-B or CC-C 2D component).</summary>
    Gs1Composite,

    /// <summary>Code 11.</summary>
    Code11,

    /// <summary>MSI Plessey.</summary>
    MsiPlessey,

    /// <summary>Pharmacode (Laetus), one-track.</summary>
    Pharmacode,

    /// <summary>Pharmacode (Laetus), two-track.</summary>
    PharmacodeTwoTrack,

    /// <summary>Telepen.</summary>
    Telepen,

    /// <summary>USPS Intelligent Mail barcode (IMb).</summary>
    UspsIntelligentMail,

    /// <summary>Royal Mail 4-State Customer Code (RM4SCC).</summary>
    RoyalMail4State,

    /// <summary>Australia Post 4-State.</summary>
    AustraliaPost,

    /// <summary>Japan Post 4-State.</summary>
    JapanPost,

    /// <summary>Dutch KIX (Klant IndeX) 4-State.</summary>
    Kix,
}
