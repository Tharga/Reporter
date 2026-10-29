using System.Drawing;
using PdfSharp.Drawing;

namespace Tharga.Reporter.Entity.Element.Extensions;

internal static class FontExtensions
{
    /// <summary>
    /// The font an element draws with when it has a font of its own or names a font class defined in the section.
    /// Null when it has neither, so the getters below fall back to the section default font.
    /// </summary>
    internal static Font OrFontClass(this Font font, string fontClass, Section section)
    {
        if (font != null || string.IsNullOrEmpty(fontClass)) return font;
        return section.FontClasses.GetValueOrDefault(fontClass);
    }

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