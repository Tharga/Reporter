using System.IO.Compression;
using System.Text;

namespace Tharga.Reporter.Tests.Rendering;

internal static class PngWriter
{
    private static readonly byte[] Signature = [0x89, (byte)'P', (byte)'N', (byte)'G', 0x0D, 0x0A, 0x1A, 0x0A];
    private const byte GreyscaleColorType = 0;
    private const byte BitDepth = 8;

    internal static byte[] FromGreyscale(byte[,] pixels)
    {
        var height = pixels.GetLength(0);
        var width = pixels.GetLength(1);

        using var png = new MemoryStream();
        png.Write(Signature);

        WriteChunk(png, "IHDR", BuildHeader(width, height));
        WriteChunk(png, "IDAT", Deflate(BuildScanLines(pixels, width, height)));
        WriteChunk(png, "IEND", []);

        return png.ToArray();
    }

    private static byte[] BuildHeader(int width, int height)
    {
        using var header = new MemoryStream();
        header.Write(BigEndian(width));
        header.Write(BigEndian(height));
        header.WriteByte(BitDepth);
        header.WriteByte(GreyscaleColorType);
        header.WriteByte(0);
        header.WriteByte(0);
        header.WriteByte(0);

        return header.ToArray();
    }

    private static byte[] BuildScanLines(byte[,] pixels, int width, int height)
    {
        var raw = new byte[height * (width + 1)];

        for (var y = 0; y < height; y++)
        {
            var offset = y * (width + 1);
            raw[offset] = 0;

            for (var x = 0; x < width; x++)
            {
                raw[offset + 1 + x] = pixels[y, x];
            }
        }

        return raw;
    }

    private static byte[] Deflate(byte[] raw)
    {
        using var compressed = new MemoryStream();
        using (var deflate = new ZLibStream(compressed, CompressionLevel.Optimal, true))
        {
            deflate.Write(raw);
        }

        return compressed.ToArray();
    }

    private static void WriteChunk(Stream target, string type, byte[] payload)
    {
        var typeBytes = Encoding.ASCII.GetBytes(type);

        target.Write(BigEndian(payload.Length));
        target.Write(typeBytes);
        target.Write(payload);
        target.Write(BigEndian(unchecked((int)Crc32([.. typeBytes, .. payload]))));
    }

    private static byte[] BigEndian(int value)
    {
        return [(byte)(value >> 24), (byte)(value >> 16), (byte)(value >> 8), (byte)value];
    }

    private static uint Crc32(byte[] data)
    {
        var crc = 0xFFFFFFFFu;

        foreach (var b in data)
        {
            crc ^= b;
            for (var i = 0; i < 8; i++)
            {
                crc = (crc & 1) != 0 ? (crc >> 1) ^ 0xEDB88320u : crc >> 1;
            }
        }

        return crc ^ 0xFFFFFFFFu;
    }
}
