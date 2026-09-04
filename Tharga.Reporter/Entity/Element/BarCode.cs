using System.Xml;
using Aspose.BarCode.Generation;
using Aspose.Drawing;
using PdfSharp.Drawing;
using Tharga.Reporter.Entity.Element.Base;
using Tharga.Reporter.Entity.Util;
using Tharga.Reporter.Extensions;
using Tharga.Reporter.Interface;

namespace Tharga.Reporter.Entity.Element;

public class BarCode : SinglePageAreaElement
{
    private const int ScanLineOffset = 10;
    private const int DarkThreshold = 128;

    private string _code;

    public string Code
    {
        get => _code ?? string.Empty;
        set => _code = value;
    }

    internal override void Render(IRenderData renderData)
    {
        if (string.IsNullOrEmpty(Code))
        {
            throw new InvalidOperationException("Code has not been set.");
        }

        if (IsNotVisible(renderData)) return;

        var bounds = GetBounds(renderData.ParentBounds);

        renderData.ElementBounds = bounds;

        if (!IsBackground || renderData.IncludeBackground)
        {
            var code = GetCode(renderData.DocumentData, renderData.PageNumberInfo);

            foreach (var bar in GetBars(code, bounds))
            {
                renderData.Graphics.DrawRectangle(XBrushes.Black, bar);
            }
        }
    }

    internal static IReadOnlyList<XRect> GetBars(string code, XRect bounds)
    {
        var pattern = GetBarPattern(code);
        if (pattern.Count == 0) return [];

        var scale = bounds.Width / pattern.Count;

        return GetRuns(pattern)
            .Select(x => new XRect(bounds.Left + x.Start * scale, bounds.Top, x.Length * scale, bounds.Height))
            .ToArray();
    }

    private static IReadOnlyList<bool> GetBarPattern(string code)
    {
        var generator = new BarcodeGenerator(EncodeTypes.Code39, code);

        using var stream = new MemoryStream();
        generator.Save(stream, BarCodeImageFormat.Png);
        stream.Position = 0;

        using var image = new Bitmap(stream);

        var scanLine = Math.Min(ScanLineOffset, image.Height - 1);

        return Enumerable.Range(0, image.Width)
            .Select(x => IsDark(image.GetPixel(x, scanLine)))
            .ToArray();
    }

    private static IEnumerable<(int Start, int Length)> GetRuns(IReadOnlyList<bool> pattern)
    {
        var start = -1;

        for (var i = 0; i <= pattern.Count; i++)
        {
            var isBar = i < pattern.Count && pattern[i];

            if (isBar && start < 0)
            {
                start = i;
            }
            else if (!isBar && start >= 0)
            {
                yield return (start, i - start);
                start = -1;
            }
        }
    }

    private static bool IsDark(Color color)
    {
        return (color.R + color.G + color.B) / 3 < DarkThreshold;
    }

    private string GetCode(IDocumentData documentData, PageNumberInfo pageNumberInfo)
    {
        return Code.ParseValue(documentData, pageNumberInfo);
    }

    internal override XmlElement ToXme()
    {
        var xme = base.ToXme();

        if (_code != null)
        {
            xme.SetAttribute("Code", _code);
        }

        return xme;
    }

    internal static BarCode Load(XmlElement xme)
    {
        var item = new BarCode();
        item.AppendData(xme);

        var xmlCode = xme.Attributes["Code"];
        if (xmlCode != null)
        {
            item.Code = xmlCode.Value;
        }

        return item;
    }
}
