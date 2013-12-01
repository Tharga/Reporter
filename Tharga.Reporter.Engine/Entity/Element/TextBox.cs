using System;
using System.Drawing;
using System.Text;
using System.Xml;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Tharga.Reporter.Engine.Entity.Area;
using Tharga.Reporter.Engine.Helper;

namespace Tharga.Reporter.Engine.Entity.Element
{
    public class TextBox : MultiPageAreaElement
    {
        private readonly Font _defaultFont = new Font();

        private Font _font;
        private string _fontClass;
        private string _value;
        private string _hideValue;
        private string[] _words;
        private int _wordPointer;

        public string Value { get { return _value ?? string.Empty; } set { _value = value; } }
        public string HideValue { get { return _hideValue ?? string.Empty; } set { _hideValue = value; } }
        public Font Font
        {
            get { return _font ?? _defaultFont; }
            set
            {
                if (!string.IsNullOrEmpty(_fontClass)) throw new InvalidOperationException("Cannot set both Font and FontClass. FontClass has already been set.");
                _font = value;
            }
        }

        public string FontClass
        {
            get { return _fontClass ?? string.Empty; }
            set
            {
                if (_font != null) throw new InvalidOperationException("Cannot set both Font and FontClass. Font has already been set.");
                _fontClass = value;
            }
        }

        protected internal override void ClearRenderPointer()
        {
            _words = null;
            _wordPointer = 0;
        }

        protected internal override bool Render(PdfPage page, XRect parentBounds, DocumentData documentData, out XRect elementBounds, bool includeBackground, bool debug, PageNumberInfo pageNumberInfo, Section section)
        {
            elementBounds = GetBounds(parentBounds);

            if (!includeBackground && IsBackground) return false;

            using (var gfx = XGraphics.FromPdfPage(page))
            {
                if (debug)
                {
                    var debugPen = new XPen(XColor.FromArgb(Color.Blue), 0.1);
                    gfx.DrawRectangle(debugPen, elementBounds);
                }

                var font = new XFont(_font.GetName(section), _font.GetSize(section), _font.GetStyle(section));
                var brush = new XSolidBrush(XColor.FromArgb(_font.GetColor(section)));

                var text = GetValue(documentData, pageNumberInfo);
                var textSize = gfx.MeasureString(text, font, XStringFormats.TopLeft);

                var left = elementBounds.Left;
                var top = elementBounds.Top;

                //Cut the string, so that it can fit within the rectangle
                if (textSize.Width > elementBounds.Width)
                {
                    _words = text.Split(' ');
                    
                    var sb = new StringBuilder();
                    for (var i = _wordPointer; i < _words.Length; i++)
                    {
                        var word = _words[i];

                        var ready = sb.ToString();
                        sb.AppendFormat("{0} ", word);
                        var newTextSize = gfx.MeasureString(sb.ToString(), font, XStringFormats.TopLeft);
                        if (newTextSize.Width > elementBounds.Width)
                        {
                            if (string.IsNullOrEmpty(ready))
                            {
                                //One singe word that is too long, print it anyway
                                gfx.DrawString(sb.ToString(), font, brush, left, top, XStringFormats.TopLeft);
                            }
                            else
                            {
                                gfx.DrawString(ready, font, brush, left, top, XStringFormats.TopLeft);
                                sb.Clear();
                                sb.AppendFormat("{0} ", word);
                            }
                            top += newTextSize.Height;

                            if (top > elementBounds.Bottom - newTextSize.Height)
                            {
                                //TODO: Span to next page!
                                //TODO: store a pointer in this section that can be resumed on next page
                                //_wordsLeft = words.GetRange()
                                _wordPointer = i;
                                return true;
                            }
                        }
                        //var t = sb.ToString();
                        //gfx.DrawString(t, font, brush, left, top, XStringFormats.TopLeft);
                    }
                }
                else
                    gfx.DrawString(text, font, brush, left, top, XStringFormats.TopLeft);
            }
            return false;
        }

        private string GetValue(DocumentData documentData, PageNumberInfo pageNumberInfo)
        {
            if (!string.IsNullOrEmpty(HideValue))
            {
                var result = documentData.Get(HideValue);
                if (string.IsNullOrEmpty(result))
                    return string.Empty;
            }

            return Value.ParseValue(documentData,pageNumberInfo);
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
                xme.SetAttribute("FontClass", _fontClass);

            if (_hideValue != null)
                xme.SetAttribute("HideValue", _hideValue);

            if (_value != null)
                xme.SetAttribute("Value", _value);

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
                text.FontClass = xmlFontClass.Value;

            var xmlHideValue = xme.Attributes["HideValue"];
            if (xmlHideValue != null)
                text.HideValue = xmlHideValue.Value;

            var xmlValue = xme.Attributes["Value"];
            if (xmlValue != null)
                text.Value = xmlValue.Value;

            return text;
        }
    }
}