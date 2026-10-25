using System.Drawing;
using PdfSharp.Drawing;

namespace Tharga.Reporter.Entity.Element.Extensions;

internal static class FontExtensions
{
    internal static XFontStyleEx GetStyle(this Font font, Section section)
    {
        //TODO: Use the font class (Somehow)

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

        //TODO: Use the font class.

        return section.DefaultFont.FontName;
    }

    internal static double GetSize(this Font font, Section section)
    {
        if (font != null)
        {
            return font.Size;
        }

        //TODO: Use the font class

        return section.DefaultFont.Size;
    }

    internal static Color GetColor(this Font font, Section section)
    {
        if (font != null)
        {
            return font.Color;
        }

        //TODO: Use the font class

        return section.DefaultFont.Color;
    }
}