using System.Buffers.Binary;

namespace Murder.Utilities;

/// <summary>
/// QOI image decoder. Decodes QOI format into RGBA byte array.
/// </summary>
public static class QoiDecoder
{
    private const int HashTableSize = 64;
    private const int HeaderSize = 14;

    public static byte[] Decode(ReadOnlySpan<byte> data, out int width, out int height)
    {
        if (data.Length < HeaderSize)
        {
            throw new InvalidOperationException("Invalid QOI data: too short");
        }

        if (BinaryPrimitives.ReadInt32BigEndian(data[..4]) != ('q' << 24 | 'o' << 16 | 'i' << 8 | 'f'))
        {
            throw new InvalidOperationException("Invalid QOI data: bad magic");
        }

        width = BinaryPrimitives.ReadInt32BigEndian(data[4..8]);
        height = BinaryPrimitives.ReadInt32BigEndian(data[8..12]);

        if (width <= 0 || height <= 0 || width * height > 400_000_000)
        {
            throw new InvalidOperationException($"Invalid QOI dimensions: {width}x{height}");
        }

        int channels = data[12];
        if (channels != 3 && channels != 4)
        {
            throw new InvalidOperationException($"Unsupported QOI channels: {channels}");
        }

        // We always decode to RGBA
        int pixelCount = width * height;
        byte[] pixels = new byte[pixelCount * 4];

        // Index table
        QoiPixel[] index = new QoiPixel[HashTableSize];

        // Default to opaque white
        QoiPixel prev = new(0, 0, 0, 255);

        int offset = HeaderSize;
        int pixelIndex = 0;

        while (offset < data.Length)
        {
            byte b0 = data[offset];

            if (b0 == 0x00)
            {
                // Index entry
                int idx = data[offset + 1];
                prev = index[idx];
                offset += 2;
            }
            else if ((b0 & 0xC0) == 0x40)
            {
                // Diff RGB
                byte diff = (byte)(b0 - 0x40);
                int r = (prev.R + diff + 1) & 0xFF;
                int g = (prev.G + diff + 1) & 0xFF;
                int b = (prev.B + diff + 1) & 0xFF;
                prev = new QoiPixel(r, g, b, prev.A);
                offset += 1;
            }
            else if ((b0 & 0xC0) == 0x80)
            {
                // Diff RGBA (luma)
                byte luma = (byte)(b0 - 0x80);
                int r = (prev.R + luma - 8) & 0xFF;
                int g = (prev.G + luma - 8) & 0xFF;
                int b = (prev.B + luma - 8) & 0xFF;
                prev = new QoiPixel(r, g, b, prev.A);
                offset += 1;
            }
            else if ((b0 & 0xC0) == 0xC0)
            {
                // Run
                int run = (b0 & 0x3F) + 1;
                offset += 1;
                for (int i = 0; i < run; i++)
                {
                    SetPixel(pixels, pixelIndex, prev);
                    pixelIndex++;
                }
                continue;
            }
            else if (b0 == 0xFE)
            {
                // RGB
                prev = new QoiPixel(data[offset + 1], data[offset + 2], data[offset + 3], prev.A);
                offset += 4;
            }
            else if (b0 == 0xFF)
            {
                // RGBA
                prev = new QoiPixel(data[offset + 1], data[offset + 2], data[offset + 3], data[offset + 4]);
                offset += 5;
            }
            else
            {
                throw new InvalidOperationException($"Unknown QOI opcode: 0x{b0:X2}");
            }

            int hash = ((prev.R & 0xFF) * 3 + (prev.G & 0xFF) * 5 + (prev.B & 0xFF) * 7 + (prev.A & 0xFF) * 11) % HashTableSize;
            index[hash] = prev;

            SetPixel(pixels, pixelIndex, prev);
            pixelIndex++;
        }

        return pixels;
    }

    private static void SetPixel(byte[] pixels, int pixelIndex, QoiPixel pixel)
    {
        int idx = pixelIndex * 4;
        pixels[idx] = (byte)pixel.R;
        pixels[idx + 1] = (byte)pixel.G;
        pixels[idx + 2] = (byte)pixel.B;
        pixels[idx + 3] = (byte)pixel.A;
    }

    private readonly struct QoiPixel
    {
        public readonly int R;
        public readonly int G;
        public readonly int B;
        public readonly int A;

        public QoiPixel(int r, int g, int b, int a)
        {
            R = r; G = g; B = b; A = a;
        }
    }
}
