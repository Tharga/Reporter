using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Tharga.Reporter.Engine.Entity.Area;
using Tharga.Reporter.Engine.Helper;

namespace Tharga.Reporter.Engine.Entity.Element
{
    public class TextBox : MultiPageElement
    {
        private Font _font;
        private string _fontClass;

        public string Value { get; set; }
        public string HideValue { get; set; }

        private string[] _words;
        private int _wordPointer;

        protected internal override void ClearRenderPointer()
        {
            _words = null;
            _wordPointer = 0;
        }

        protected internal override bool Render(PdfPage page, XRect parentBounds, DocumentData documentData, out XRect elementBounds, bool includeBackground, bool debug, PageNumberInfo pageNumberInfo)
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

                var font = new XFont(GetFontName(), GetFontSize(), XFontStyle.Regular);
                var brush = new XSolidBrush(XColor.FromArgb(GetColor()));

                var text = GetValue(documentData, pageNumberInfo);
                var textSize = gfx.MeasureString(text, font, XStringFormats.TopLeft);

                //Cut the string, so that it can fit within the rectangle
                if (textSize.Width > elementBounds.Width)
                {
                    var left = elementBounds.Left;
                    var top = elementBounds.Top;

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
                    }
                }
            }
            return false;
        }

        public string FontClass
        {
            get { return _fontClass; }
            set
            {
                if (_font != null) throw new InvalidOperationException("Cannot set both Font and FontClass. Font has already been set.");
                _fontClass = value;
            }
        }

        public Font Font
        {
            get
            {
                if (!string.IsNullOrEmpty(_fontClass)) throw new InvalidOperationException("Cannot set both Font and FontClass. FontClass has already been set.");
                return _font ?? (_font = new Font());
            }
        }

        internal string GetFontName()
        {
            return Font.GetRenderName(FontClass);
        }

        internal double GetFontSize()
        {
            return Font.GetRenderSize(FontClass);
        }

        internal Color GetColor()
        {
            return Font.GetRenderColor(FontClass);
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

        protected internal override XmlElement AppendXml(ref XmlElement xmePane)
        {
            throw new NotImplementedException();
        }
    }
}