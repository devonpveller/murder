using System.Buffers.Binary;

namespace Murder.Utilities.Serialization;

/// <summary>
/// QOI (Quite OK Image) decoder.
/// Decodes QOI image data into raw RGBA pixel data.
/// </summary>
public static class QoiDecoder
{
    public const int HeaderSize = 14;
    public const int HashTableSize = 64;
    public const int Magic = 'q' << 24 | 'o' << 16 | 'i' << 8 | 'f';

    private static int CalculateHashTableIndex(int r, int g, int b, int a) =>
        ((r & 0xFF) * 3 + (g & 0xFF) * 5 + (b & 0xFF) * 7 + (a & 0xFF) * 11) % HashTableSize * 4;

    public static byte[] Decode(ReadOnlySpan<byte> data)
    {
        if (data.Length < HeaderSize)
            throw new Exception("Invalid QOI data: too short");

        // Read header
        int magic = BinaryPrimitives.ReadInt32BigEndian(data[..4]);
        if (magic != Magic)
            throw new Exception("Invalid QOI magic");

        int width = BinaryPrimitives.ReadInt32BigEndian(data[4..8]);
        int height = BinaryPrimitives.ReadInt32BigEndian(data[8..12]);
        byte channels = data[12];

        if (width <= 0 || height <= 0)
            throw new Exception($"Invalid QOI dimensions: {width}x{height}");

        if (channels != 3 && channels != 4)
            throw new Exception($"Invalid QOI channels: {channels}");

        int totalPixels = width * height;

        // Allocate output buffer (always RGBA)
        byte[] output = new byte[totalPixels * 4];

        // Hash table for index entries
        byte[] index = new byte[HashTableSize * 4];

        byte prevR = 0, prevG = 0, prevB = 0, prevA = 255;
        int p = HeaderSize;
        int outPos = 0;

        while (p < data.Length - 8) // stop before padding
        {
            byte op = data[p++];
            int opType = op & 0xC0;

            if (opType == 0x00) // Index entry
            {
                int indexPos = (op & 0x3F) * 4;
                prevR = index[indexPos];
                prevG = index[indexPos + 1];
                prevB = index[indexPos + 2];
                prevA = index[indexPos + 3];
            }
            else if (opType == 0x40) // Diff
            {
                int dr = ((op >> 4) & 0x03) - 2;
                int dg = ((op >> 2) & 0x03) - 2;
                int db = (op & 0x03) - 2;
                prevR = (byte)(prevR + dr);
                prevG = (byte)(prevG + dg);
                prevB = (byte)(prevB + db);
            }
            else if (opType == 0x80) // Luma
            {
                byte vg = (byte)(data[p++] - 32);
                byte vgr = (byte)((data[p] >> 4) - 8);
                byte vgb = (byte)((data[p++] & 0x0F) - 8);
                prevR = (byte)(prevR + vg + vgr);
                prevG = (byte)(prevG + vg);
                prevB = (byte)(prevB + vg + vgb);
            }
            else if (opType == 0xC0) // Run
            {
                int run = (op & 0x3F) + 1;
                // Clamp run to not exceed total pixels (some QOI files have extra padding)
                int remaining = totalPixels - (outPos / 4);
                if (run > remaining) run = remaining;
                for (int i = 0; i < run; i++)
                {
                    output[outPos++] = prevR;
                    output[outPos++] = prevG;
                    output[outPos++] = prevB;
                    output[outPos++] = prevA;
                }
                continue;
            }
            else if (op == 0xFE) // RGB
            {
                prevR = data[p++];
                prevG = data[p++];
                prevB = data[p++];
            }
            else if (op == 0xFF) // RGBA
            {
                prevR = data[p++];
                prevG = data[p++];
                prevB = data[p++];
                prevA = data[p++];
            }

            // Update hash table
            int hashIdx = CalculateHashTableIndex(prevR, prevG, prevB, prevA);
            index[hashIdx] = prevR;
            index[hashIdx + 1] = prevG;
            index[hashIdx + 2] = prevB;
            index[hashIdx + 3] = prevA;

            // Write to output
            output[outPos++] = prevR;
            output[outPos++] = prevG;
            output[outPos++] = prevB;
            output[outPos++] = prevA;
        }

        return output;
    }
}
