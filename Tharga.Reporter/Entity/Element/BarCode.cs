using System.Xml;
using PdfSharp.Drawing;
using Tharga.Reporter.Entity.Element.Base;
using Tharga.Reporter.Entity.Util;
using Tharga.Reporter.Extensions;
using Tharga.Reporter.Interface;
using ZXing.OneD;

namespace Tharga.Reporter.Entity.Element;

public class BarCode : SinglePageAreaElement
{
    private const string Code39Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-. $/+%";
    private const int QuietZoneModules = 10;

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

    /// <summary>
    /// Code 39 encodes an uppercase alphabet only. Lower case is upper-cased rather than dropped, so a code
    /// still encodes the characters it was given.
    /// </summary>
    internal static string Normalize(string code)
    {
        var normalized = (code ?? string.Empty).ToUpperInvariant();

        var invalid = normalized.Where(x => !Code39Alphabet.Contains(x)).Distinct().ToArray();
        if (invalid.Length != 0)
        {
            throw new InvalidOperationException($"The code '{code}' cannot be encoded as Code 39. Unsupported {(invalid.Length == 1 ? "character" : "characters")}: {string.Join(", ", invalid.Select(x => $"'{x}'"))}.");
        }

        return normalized;
    }

    private static IReadOnlyList<bool> GetBarPattern(string code)
    {
        var encoded = new Code39Writer().encode(Normalize(code));

        var pattern = new bool[QuietZoneModules + encoded.Length + QuietZoneModules];
        encoded.CopyTo(pattern, QuietZoneModules);

        return pattern;
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
