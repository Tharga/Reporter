using System;
using System.Drawing;
using System.Xml;
using PdfSharp.Drawing;
using Tharga.Reporter.Engine.Entity.Area;
using Tharga.Reporter.Engine.Interface;

namespace Tharga.Reporter.Engine.Entity.Element
{
    public abstract class TextBase : SinglePageAreaElement
    {
        private readonly Font _defaultFont = new Font();
        private readonly Alignment _defaultTextAlignmen = Alignment.Left;

        public enum Alignment { Left, Right }

        private Font _font;
        private string _fontClass;
        private Alignment? _textAlignment;

        public Font Font
        {
            get
            {
                return _font ?? _defaultFont;
            }
            set
            {
                if (!string.IsNullOrEmpty(_fontClass)) throw new InvalidOperationException("Cannot set both Font and FontClass. FontClass has already been set.");
                _font = value;
            }
        }

        internal string FontClass //TODO: Hidden because it is not yet fully implemented
        {
            get
            {
                return _fontClass ?? string.Empty;
            }
            set
            {
                if (_font != null) throw new InvalidOperationException("Cannot set both Font and FontClass. Font has already been set.");
                _fontClass = value;
            }
        }

        public Alignment TextAlignment { get { return _textAlignment ?? _defaultTextAlignmen; } set { _textAlignment = value; } }

        internal override void Render(IRenderData renderData)
        {
            var bounds = GetBounds(renderData.ParentBounds);

            var font = new XFont(_font.GetName(renderData.Section), _font.GetSize(renderData.Section), _font.GetStyle(renderData.Section));
            var brush = new XSolidBrush(XColor.FromArgb(_font.GetColor(renderData.Section)));

            var text = GetValue(renderData.DocumentData, renderData.PageNumberInfo);
            var textSize = renderData.Graphics.MeasureString(text, font, XStringFormats.TopLeft);

            var offset = 0D;
            if (TextAlignment == Alignment.Right)
            {
                offset = bounds.Width - textSize.Width;
            }

            renderData.ElementBounds = new XRect(bounds.Left + offset, bounds.Y, textSize.Width, textSize.Height);

            if (renderData.IncludeBackground || !IsBackground)
            {
                if (renderData.PageNumberInfo.TotalPages == null)
                    throw new InvalidOperationException("The prerendering step did not set the number of total pages!");

                //TODO: Move this to be performed on all elements
                if (Visibility == PageVisibility.All
                    || Visibility == PageVisibility.FirstPage && renderData.PageNumberInfo.PageNumber == 1
                    || Visibility == PageVisibility.AllButFirst && renderData.PageNumberInfo.PageNumber != 1
                    || Visibility == PageVisibility.LastPage && renderData.PageNumberInfo.PageNumber == renderData.PageNumberInfo.TotalPages
                    || Visibility == PageVisibility.AllButLast && renderData.PageNumberInfo.PageNumber != renderData.PageNumberInfo.TotalPages
                    || Visibility == PageVisibility.WhenSinglePage && renderData.PageNumberInfo.TotalPages == 1
                    || Visibility == PageVisibility.WhenMultiplePages && renderData.PageNumberInfo.TotalPages > 1)
                {
                    renderData.Graphics.DrawString(text, font, brush, renderData.ElementBounds, XStringFormats.TopLeft);

                    if (renderData.Debug)
                    {
                        var debugPen = new XPen(XColor.FromArgb(Color.LightBlue), 0.1);
                        renderData.Graphics.DrawRectangle(debugPen, renderData.ElementBounds.Left, renderData.ElementBounds.Top, textSize.Width, textSize.Height);
                    }
                }
            }
        }

        protected abstract string GetValue(DocumentData documentData, PageNumberInfo pageNumberInfo);

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