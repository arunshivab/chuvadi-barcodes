// Copyright 2026 Chuvadi Contributors
// SPDX-License-Identifier: Apache-2.0
// PHASE: M2a — QR data segments

using System.Collections.Generic;
using Chuvadi.Barcodes.Common;

namespace Chuvadi.Barcodes.Encoders.Qr;

/// <summary>A run of characters encoded in one mode.</summary>
internal sealed class QrSegment
{
    public QrSegment(QrMode mode, List<QrCharacter> characters)
    {
        Mode = mode;
        Characters = characters;
    }

    public QrMode Mode { get; }

    public List<QrCharacter> Characters { get; }

    /// <summary>The value written in the character-count indicator.</summary>
    public int CharacterCount
    {
        get
        {
            int count = 0;
            foreach (QrCharacter c in Characters)
            {
                count += Mode switch
                {
                    QrMode.Alphanumeric => c.AlphanumericValues.Length,
                    QrMode.Byte => c.Bytes.Length,
                    _ => 1,
                };
            }

            return count;
        }
    }

    /// <summary>Length of the data part only (no mode indicator or count).</summary>
    public int DataBitLength
    {
        get
        {
            int n = CharacterCount;
            return Mode switch
            {
                QrMode.Numeric => ((n / 3) * 10) + (n % 3 == 0 ? 0 : n % 3 == 1 ? 4 : 7),
                QrMode.Alphanumeric => ((n / 2) * 11) + ((n % 2) * 6),
                QrMode.Byte => n * 8,
                _ => n * 13,
            };
        }
    }

    /// <summary>Appends the data part (no mode indicator or count).</summary>
    public void WriteData(BitBuffer buffer)
    {
        switch (Mode)
        {
            case QrMode.Numeric:
                {
                    int accumulator = 0;
                    int digits = 0;
                    foreach (QrCharacter c in Characters)
                    {
                        accumulator = (accumulator * 10) + c.NumericValue;
                        digits++;
                        if (digits == 3)
                        {
                            buffer.Append(accumulator, 10);
                            accumulator = 0;
                            digits = 0;
                        }
                    }

                    if (digits > 0)
                    {
                        buffer.Append(accumulator, (digits * 3) + 1);
                    }

                    break;
                }

            case QrMode.Alphanumeric:
                {
                    List<int> values = [];
                    foreach (QrCharacter c in Characters)
                    {
                        values.AddRange(c.AlphanumericValues);
                    }

                    int i = 0;
                    for (; i + 1 < values.Count; i += 2)
                    {
                        buffer.Append((values[i] * 45) + values[i + 1], 11);
                    }

                    if (i < values.Count)
                    {
                        buffer.Append(values[i], 6);
                    }

                    break;
                }

            case QrMode.Byte:
                foreach (QrCharacter c in Characters)
                {
                    foreach (byte b in c.Bytes)
                    {
                        buffer.Append(b, 8);
                    }
                }

                break;

            default:
                foreach (QrCharacter c in Characters)
                {
                    buffer.Append(c.KanjiValue, 13);
                }

                break;
        }
    }
}
