using System.Drawing;
using PdfSharp.Drawing;

namespace Tharga.Reporter.Entity.Element.Extensions;

// Font classes are not resolved here yet, only an element's own font or the section default. See #30.
internal static class FontExtensions
{
    internal static XFontStyleEx GetStyle(this Font font, Section section)
    {
        var source = font ?? section.DefaultFont;
        var bold = source.Bold;
        var italic = source.Italic;
        var underline = source.Underline;
        var strikeout = source.Strikeout;

        if (bold && italic)
        {
            return XFontStyleEx.BoldItalic;
        }

        if (bold)
        {
            return XFontStyleEx.Bold;
        }

        if (italic)
        {
            return XFontStyleEx.Italic;
        }

        if (underline)
        {
            return XFontStyleEx.Underline;
        }

        if (strikeout)
        {
            return XFontStyleEx.Strikeout;
        }

        return XFontStyleEx.Regular;
    }

    internal static string GetName(this Font font, Section section)
    {
        if (font != null)
        {
            return font.FontName;
        }

        return section.DefaultFont.FontName;
    }

    internal static double GetSize(this Font font, Section section)
    {
        if (font != null)
        {
            return font.Size;
        }

        return section.DefaultFont.Size;
    }

    internal static Color GetColor(this Font font, Section section)
    {
        if (font != null)
        {
            return font.Color;
        }

        return section.DefaultFont.Color;
    }
}