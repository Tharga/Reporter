using System.Text;
using System.Xml;
using PdfSharp.Drawing;
using Tharga.Reporter.Entity.Element.Base;
using Tharga.Reporter.Entity.Element.Extensions;
using Tharga.Reporter.Entity.Util;
using Tharga.Reporter.Extensions;
using Tharga.Reporter.Interface;

namespace Tharga.Reporter.Entity.Element;

public class TextBox : MultiPageAreaElement
{
    private readonly Font _defaultFont = new();

    private Font _font;
    private string _fontClass;
    private string _hideValue;
    private string _value;

    public string Value
    {
        get => _value ?? string.Empty;
        set => _value = value;
    }

    public string HideValue
    {
        get => _hideValue ?? string.Empty;
        set => _hideValue = value;
    }

    public Font Font
    {
        get => _font ?? _defaultFont;
        set
        {
            if (!string.IsNullOrEmpty(_fontClass)) throw new InvalidOperationException("Cannot set both Font and FontClass. FontClass has already been set.");
            _font = value;
        }
    }

    public string FontClass
    {
        get => _fontClass ?? string.Empty;
        set
        {
            if (_font != null) throw new InvalidOperationException("Cannot set both Font and FontClass. Font has already been set.");
            _fontClass = value;
        }
    }

    internal override int PreRender(IRenderData renderData)
    {
        var pageText = new List<string[]>();
        renderData.Context.SetPages(this, pageText);

        renderData.ElementBounds = GetBounds(renderData);

        if (!renderData.IncludeBackground && IsBackground)
        {
            return 0;
        }

        var elementFont = _font.OrFontClass(_fontClass, renderData.Section);
        var font = new XFont(elementFont.GetName(renderData.Section), elementFont.GetSize(renderData.Section), elementFont.GetStyle(renderData.Section));

        var text = GetValue(renderData.DocumentData, renderData.PageNumberInfo);
        var textSize = renderData.Graphics.MeasureString(text, font, XStringFormats.TopLeft);

        var top = renderData.ElementBounds.Top;

        if (textSize.Width > renderData.ElementBounds.Width)
        {
            //Need to set data over more than one page
            var words = text.Split(' ');

            var sb = new StringBuilder();
            var lines = new List<string>();
            foreach (var nextWord in words)
            {
                var textSoFar = sb.ToString();
                sb.AppendFormat("{0} ", nextWord);
                var nextTextSize = renderData.Graphics.MeasureString(sb.ToString(), font, XStringFormats.TopLeft);
                if (nextTextSize.Width > renderData.ElementBounds.Width) //Now we are over the limit (Previous state will fit)
                {
                    if (string.IsNullOrEmpty(textSoFar))
                    {
                        //One singe word that is too long, print it anyway
                        //renderData.Gfx.DrawString(sb.ToString(), font, brush, left, top, XStringFormats.TopLeft);
                        lines.Add(sb.ToString());
                        sb.Clear();
                    }
                    else
                    {
                        //renderData.Gfx.DrawString(ready, font, brush, left, top, XStringFormats.TopLeft);
                        lines.Add(textSoFar);
                        sb.Clear();
                        sb.AppendFormat("{0} ", nextWord);
                    }

                    top += nextTextSize.Height;

                    if (top > renderData.ElementBounds.Bottom - nextTextSize.Height)
                    {
                        //Now we have reached the limit of the page
                        pageText.Add(lines.ToArray());
                        lines.Clear();
                        top = renderData.ElementBounds.Top;
                    }
                }
            }

            if (sb.Length > 0)
            {
                lines.Add(sb.ToString());
            }

            if (lines.Count > 0 || pageText.Count == 0)
            {
                pageText.Add(lines.ToArray());
            }
        }
        else
        {
            pageText.Add(new[] { text });
        }

        return pageText.Count;
    }

    internal override void Render(IRenderData renderData, int page)
    {
        if (IsNotVisible(renderData))
        {
            return;
        }

        var pageText = renderData.Context.GetPages<string[]>(this);
        if (pageText == null) throw new InvalidOperationException("Pre-render has not been performed.");

        renderData.ElementBounds = GetBounds(renderData);

        if (!renderData.IncludeBackground && IsBackground)
        {
            return;
        }

        if (renderData.DebugData != null)
        {
            renderData.Graphics.DrawRectangle(renderData.DebugData.Pen, renderData.ElementBounds);
        }

        var elementFont = _font.OrFontClass(_fontClass, renderData.Section);
        var font = new XFont(elementFont.GetName(renderData.Section), elementFont.GetSize(renderData.Section), elementFont.GetStyle(renderData.Section));
        var brush = new XSolidBrush(elementFont.GetColor(renderData.Section).ToXColor());

        var left = renderData.ElementBounds.Left;
        var top = renderData.ElementBounds.Top;

        if (pageText.Count > page - renderData.Context.GetPageOffset(renderData.Section))
        {
            foreach (var line in pageText[page - renderData.Context.GetPageOffset(renderData.Section)])
            {
                renderData.Graphics.DrawString(line, font, brush, new XPoint(left, top), XStringFormats.TopLeft);
                var newTextSize = renderData.Graphics.MeasureString(line, font, XStringFormats.TopLeft);
                top += newTextSize.Height;
            }
        }
    }

    private string GetValue(IDocumentData documentData, PageNumberInfo pageNumberInfo)
    {
        if (!string.IsNullOrEmpty(HideValue))
        {
            var result = documentData.Get(HideValue);
            if (string.IsNullOrEmpty(result))
            {
                return string.Empty;
            }
        }

        return Value.ParseValue(documentData, pageNumberInfo);
    }

    internal override XmlElement ToXme()
    {
        var xme = base.ToXme();

        if (_font != null)
        {
            var fontXme = _font.ToXme();
            var importedFont = xme.OwnerDocument.ImportNode(fontXme, true);
            xme.AppendChild(importedFont);
        }

        if (_fontClass != null)
        {
            xme.SetAttribute("FontClass", _fontClass);
        }

        if (_hideValue != null)
        {
            xme.SetAttribute("HideValue", _hideValue);
        }

        if (_value != null)
        {
            xme.SetAttribute("Value", _value);
        }

        return xme;
    }

    internal static TextBox Load(XmlElement xme)
    {
        var text = new TextBox();

        text.AppendData(xme);

        foreach (XmlElement child in xme)
        {
            switch (child.Name)
            {
                case "Font":
                    text.Font = Font.Load(child);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Unknown subelement {0} to text base.", child.Name));
            }
        }

        var xmlFontClass = xme.Attributes["FontClass"];
        if (xmlFontClass != null)
        {
            text.FontClass = xmlFontClass.Value;
        }

        var xmlHideValue = xme.Attributes["HideValue"];
        if (xmlHideValue != null)
        {
            text.HideValue = xmlHideValue.Value;
        }

        var xmlValue = xme.Attributes["Value"];
        if (xmlValue != null)
        {
            text.Value = xmlValue.Value;
        }

        return text;
    }
}