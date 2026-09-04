using PdfSharp.Drawing;
using Tharga.Reporter.Entity.Element;

namespace Tharga.Reporter.Tests.Rendering;

internal static class BarCodeSampleWriter
{
    private const int PixelsPerCharacter = 100;
    private const int SampleHeight = 300;
    private const int Code39StartAndStopCharacters = 2;
    private const byte Black = 0x00;
    private const byte White = 0xFF;

    internal static string OutputDirectory => Path.Combine(AppContext.BaseDirectory, "barcode-samples");

    internal static int ScannableWidth(string code)
    {
        return (code.Length + Code39StartAndStopCharacters) * PixelsPerCharacter;
    }

    internal static byte[] Rasterize(string code, int width, int height)
    {
        return PngWriter.FromGreyscale(RasterizePixels(code, width, height));
    }

    internal static byte[,] RasterizePixels(string code, int width, int height)
    {
        var bars = BarCode.GetBars(code, new XRect(0, 0, width, height));

        var pixels = new byte[height, width];
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                pixels[y, x] = White;
            }
        }

        foreach (var bar in bars)
        {
            var from = Math.Max((int)Math.Round(bar.Left), 0);
            var to = Math.Min((int)Math.Round(bar.Left + bar.Width), width);

            for (var x = from; x < to; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    pixels[y, x] = Black;
                }
            }
        }

        return pixels;
    }

    internal static string WritePng(string code)
    {
        Directory.CreateDirectory(OutputDirectory);

        var path = Path.Combine(OutputDirectory, $"barcode-{code}.png");
        File.WriteAllBytes(path, Rasterize(code, ScannableWidth(code), SampleHeight));

        return path;
    }
}
