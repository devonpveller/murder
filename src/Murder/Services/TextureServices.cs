using Microsoft.Xna.Framework.Graphics;
using Murder.Core.Graphics;
using Murder.Diagnostics;
using Murder.Utilities.Serialization;
using System.IO.Compression;

namespace Murder.Services;

public static class TextureServices
{
    public const string QOI_GZ_EXTENSION = ".qoi.gz";
    public const string PNG_EXTENSION = ".png";

    // Summary:
    //     Creates a Microsoft.Xna.Framework.Graphics.Texture2D from a file, supported formats
    //     bmp, gif, jpg, png, tif and dds (only for simple textures). May work with other
    //     formats, but will not work with tga files. This internally calls Microsoft.Xna.Framework.Graphics.Texture2D.FromStream(Microsoft.Xna.Framework.Graphics.GraphicsDevice,System.IO.Stream).
    //
    // Parameters:
    //   graphicsDevice:
    //     The graphics device to use to create the texture.
    //
    //   path:
    //     The path to the image file.
    //
    // Returns:
    //     The Microsoft.Xna.Framework.Graphics.Texture2D created from the given file.
    //
    // Remarks:
    //     Note that different image decoders may generate slight differences between platforms,
    //     but perceptually the images should be identical.
    public static Texture2D FromFile(GraphicsDevice graphicsDevice, string path)
    {
        if (File.Exists(path) == false)
        {
            GameLogger.Error($"File not found: {path}");
            return SharedResources.GetOrCreatePixel();
        }

        using FileStream file = File.OpenRead(path);

        if (!path.EndsWith(QOI_GZ_EXTENSION))
        {
            return Texture2D.FromStream(graphicsDevice, file);
        }

        // First decompress the file.
        using GZipStream gzipDecodeStream = new(file, CompressionMode.Decompress);
        using MemoryStream memStream = new();

        gzipDecodeStream.CopyTo(memStream);

        // Decode QOI data to raw RGBA pixels
        byte[] qoiData = memStream.ToArray();
        byte[] rgbaPixels = QoiDecoder.Decode(qoiData);

        // Parse QOI header to get dimensions
        int width = System.Buffers.Binary.BinaryPrimitives.ReadInt32BigEndian(qoiData.AsSpan(4, 4));
        int height = System.Buffers.Binary.BinaryPrimitives.ReadInt32BigEndian(qoiData.AsSpan(8, 4));

        // Create texture from raw RGBA data
        Texture2D texture = new(graphicsDevice, width, height);
        texture.SetData(rgbaPixels);

        gzipDecodeStream.Close();
        memStream.Close();

        return texture;
    }
}