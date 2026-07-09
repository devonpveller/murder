using System.Buffers.Binary;

namespace Murder.Utilities.Serialization;

/// <summary>
/// QOI Decoder — decodes QOI image data into raw RGBA pixels.
/// </summary>
public static class QoiDecoder
{
    /// <summary>
    /// Decodes QOI image data into raw RGBA pixel array.
    /// </summary>
    /// <param name="data">QOI encoded data.</param>
    /// <returns>Raw RGBA pixel data.</returns>
    public static byte[] Decode(ReadOnlySpan<byte> data)
    {
        if (data.Length < QoiCodec.HeaderSize)
        {
            throw new Exception("QOI data too small for header");
        }

        if (!QoiCodec.IsValidMagic(data[..4]))
        {
            throw new Exception("Invalid QOI magic");
        }

        int width = BinaryPrimitives.ReadInt32BigEndian(data[4..8]);
        int height = BinaryPrimitives.ReadInt32BigEndian(data[8..12]);
        byte channels = data[12];
        // byte colorSpace = data[13]; // Not needed for decoding

        if (width <= 0 || height <= 0 || width * height >= QoiCodec.MaxPixels)
        {
            throw new Exception($"Invalid QOI dimensions: {width}x{height}");
        }

        if (channels != 3 && channels != 4)
        {
            throw new Exception($"Invalid QOI channels: {channels}");
        }

        int pixelCount = width * height;
        byte[] pixels = new byte[pixelCount * 4]; // Always output RGBA

        byte[] index = new byte[QoiCodec.HashTableSize * 4];

        byte prevR = 0;
        byte prevG = 0;
        byte prevB = 0;
        byte prevA = 255;

        int p = QoiCodec.HeaderSize;
        int pxPos = 0;

        while (pxPos < pixelCount)
        {
            byte byte1 = data[p++];

            if ((byte1 & QoiCodec.Mask2) == 0x00)
            {
                // Index opcode: 6-bit index into color index table
                int idx = (byte1 & 0x3F) * 4;
                prevR = index[idx];
                prevG = index[idx + 1];
                prevB = index[idx + 2];
                prevA = index[idx + 3];
            }
            else if ((byte1 & QoiCodec.Mask2) == QoiCodec.Diff)
            {
                int vr = ((byte1 >> 4) & 0x03) - 2;
                int vg = ((byte1 >> 2) & 0x03) - 2;
                int vb = (byte1 & 0x03) - 2;

                prevR = (byte)(prevR + vr);
                prevG = (byte)(prevG + vg);
                prevB = (byte)(prevB + vb);
            }
            else if ((byte1 & QoiCodec.Mask2) == QoiCodec.Luma)
            {
                int vg = (byte1 & 0x3F) - 32;
                byte byte2 = data[p++];
                int vgr = ((byte2 >> 4) & 0x0F) - 8;
                int vgb = (byte2 & 0x0F) - 8;

                prevR = (byte)(prevG + vg + vgr);
                prevG = (byte)(prevG + vg);
                prevB = (byte)(prevG + vg + vgb);
            }
            else if ((byte1 & QoiCodec.Mask2) == QoiCodec.Run)
            {
                int run = (byte1 & 0x3F) + 1;
                int end = Math.Min(pxPos + run, pixelCount);
                while (pxPos < end)
                {
                    pixels[pxPos * 4] = prevR;
                    pixels[pxPos * 4 + 1] = prevG;
                    pixels[pxPos * 4 + 2] = prevB;
                    pixels[pxPos * 4 + 3] = prevA;
                    pxPos++;
                }
                continue;
            }
            else if (byte1 == QoiCodec.Rgb)
            {
                prevR = data[p++];
                prevG = data[p++];
                prevB = data[p++];
            }
            else if (byte1 == QoiCodec.Rgba)
            {
                prevR = data[p++];
                prevG = data[p++];
                prevB = data[p++];
                prevA = data[p++];
            }

            int indexPos = QoiCodec.CalculateHashTableIndex(prevR, prevG, prevB, prevA);
            index[indexPos] = prevR;
            index[indexPos + 1] = prevG;
            index[indexPos + 2] = prevB;
            index[indexPos + 3] = prevA;

            pixels[pxPos * 4] = prevR;
            pixels[pxPos * 4 + 1] = prevG;
            pixels[pxPos * 4 + 2] = prevB;
            pixels[pxPos * 4 + 3] = prevA;
            pxPos++;
        }

        return pixels;
    }
}
