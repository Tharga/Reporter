using System;
using System.Drawing;
using System.Xml;
using PdfSharp.Drawing;
using Tharga.Reporter.Engine.Entity.Area;

namespace Tharga.Reporter.Engine.Entity.Element
{
    public abstract class TextBase : SinglePageElement
    {
        private readonly Font _defaultFont = new Font();
        private const Alignment _defaultTextAlignmen = Alignment.Left;

        public enum Alignment { Left, Right }

        private Font _font;
        private string _fontClass;
        private Alignment? _textAlignment;

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

        public Alignment TextAlignment { get { return _textAlignment ?? _defaultTextAlignmen; } set { _textAlignment = value; } }

        protected internal override void Render(PdfSharp.Pdf.PdfPage page, XRect parentBounds,
                                                DocumentData documentData, out XRect elementBounds, bool includeBackground, bool debug, PageNumberInfo pageNumberInfo)
        {
            var bounds = GetBounds(parentBounds);

            using (var gfx = XGraphics.FromPdfPage(page))
            {
                var font = new XFont(GetName(), GetSize(), XFontStyle.Regular);
                var brush = new XSolidBrush(XColor.FromArgb(GetColor()));

                var text = GetValue(documentData, pageNumberInfo);
                var textSize = gfx.MeasureString(text, font, XStringFormats.TopLeft);

                var offset = 0D;
                if (TextAlignment == Alignment.Right)
                {
                    offset = bounds.Width - textSize.Width;
                }

                elementBounds = new XRect(bounds.Left + offset, bounds.Y, textSize.Width, textSize.Height);

                if (includeBackground || !IsBackground)
                {
                    gfx.DrawString(text, font, brush, elementBounds, XStringFormats.TopLeft);
                    //gfx.DrawString(text, font, brush, bounds, XStringFormats.TopLeft);

                    if (debug)
                    {
                        var debugPen = new XPen(XColor.FromArgb(Color.LightBlue), 0.1);
                        gfx.DrawRectangle(debugPen, elementBounds.Left, elementBounds.Top, textSize.Width, textSize.Height);
                    }
                }
                //TODO: Manually create line feed so that text can fit inside a box. (Spanning on multipler pages)
            }
        }

        protected abstract string GetValue(DocumentData documentData, PageNumberInfo pageNumberInfo);

        private string GetName()
        {
            return Font.GetRenderName(FontClass);
        }

        private double GetSize()
        {
            return Font.GetRenderSize(FontClass);
        }

        private Color GetColor()
        {
            return Font.GetRenderColor(FontClass);
        }

        protected override void AppendData(XmlElement xme)
        {
            base.AppendData(xme);

            var xmlAlignment = xme.Attributes["Alignment"];
            if (xmlAlignment != null)
                TextAlignment = (Alignment)Enum.Parse(typeof(Alignment), xmlAlignment.Value);

            foreach (XmlElement child in xme)
            {
                switch (child.Name)
                {
                    case "Font":
                        _font = Font.Load(child);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(string.Format("Unknown subelement {0} to text base.", child.Name));
                }
            }

            var xmlFontClass = xme.Attributes["FontClass"];
            if (xmlFontClass != null)
                FontClass = xmlFontClass.Value;
        }
                
        internal override XmlElement ToXme()
        {
            var xme = base.ToXme();

            if (_textAlignment != null)
                xme.SetAttribute("Alignment", _textAlignment.ToString());

            if (_font != null)
            {
                var fontXme = _font.ToXme();
                var importedFont = xme.OwnerDocument.ImportNode(fontXme, true);
                xme.AppendChild(importedFont);
            }

            if (_fontClass != null)
                xme.SetAttribute("FontClass", _fontClass);

            return xme;
        }
    }
}