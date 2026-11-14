using System.Drawing;
using PdfSharp.Drawing;

namespace Tharga.Reporter.Entity.Element.Extensions;

internal static class FontExtensions
{
    /// <summary>
    /// The font an element draws with. Every value set on the element's own font is used; a value that is not set
    /// comes from the font class the element names, and then from the default font of the section.
    /// </summary>
    internal static Font Resolve(this Font font, string fontClass, Section section)
    {
        var classFont = string.IsNullOrEmpty(fontClass) ? null : section.FontClasses.GetValueOrDefault(fontClass);
        return (font ?? new Font()).Over(classFont).Over(section.DefaultFont);
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