using System;
using System.Drawing;
using System.Xml;
using PdfSharp.Drawing;

namespace Tharga.Reporter.Engine.Entity.Element
{
    public abstract class TextBase : Element
    {
        public enum Alignment { Left, Right }

        private Font _font;
        private string _fontClass;


        public Alignment TextAlignment { get; set; }

        internal TextBase()
        {
            
        }

        protected TextBase(XmlElement xmlElement)
            :base(xmlElement)
        {
            if ( xmlElement.Attributes.GetNamedItem("FontClass") != null)
                FontClass = xmlElement.Attributes.GetNamedItem("FontClass").Value;

            foreach(XmlElement subElement in xmlElement.ChildNodes)
            {
                switch(subElement.Name)
                {
                    case "Font":
                        _font = new Font(subElement);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(string.Format("Unknown sub type {0} to TextBase.", subElement.Name));
                }
            }
        }

        protected TextBase(string fontClass, UnitRectangle relativeAlignment)
            : base(relativeAlignment)
        {
            FontClass = fontClass;
        }

        protected TextBase(string fontClass)
        {
            FontClass = fontClass;
        }

        protected internal override void Render(PdfSharp.Pdf.PdfPage page, XRect parentBounds,
                                                DocumentData documentData, out XRect elementBounds, bool includeBackground, bool debug)
        {
            var bounds = GetBounds(parentBounds);

            using (var gfx = XGraphics.FromPdfPage(page))
            {
                var font = new XFont(GetName(), GetSize(), XFontStyle.Regular);
                var brush = new XSolidBrush(XColor.FromArgb(GetColor()));

                var text = GetValue(documentData);
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

        protected abstract string GetValue(DocumentData documentData);

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

        internal string GetName()
        {
            return Font.GetRenderName(FontClass);
        }

        internal double GetSize()
        {
            return Font.GetRenderSize(FontClass);
        }

        internal Color GetColor()
        {
            return Font.GetRenderColor(FontClass);
        }

        protected internal override XmlElement AppendXml(ref XmlElement xmePane)
        {
            if (xmePane == null) throw new ArgumentNullException("xmePane");

            var xmeElement = AppendXmlBase(ref xmePane);

            if (!string.IsNullOrEmpty(FontClass))
                xmeElement.SetAttribute("FontClass", FontClass);

            if (xmePane.OwnerDocument == null) throw new ArgumentNullException("xmePane", "xmeSection has no owner document.");

            if (_font != null)
                xmeElement.AppendChild(xmePane.OwnerDocument.ImportNode(_font.ToXml(), true));

            return xmeElement;
        }
    }
}