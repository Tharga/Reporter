using System;
using System.Drawing;
using System.Xml;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Tharga.Reporter.Engine.Entity.Area;

namespace Tharga.Reporter.Engine.Entity.Element
{
    public sealed class Line : SinglePageElement
    {
        private readonly Color _defaultBorderColor = Color.Black;

        public Color BorderColor { get; set; }
        private string BorderWidth { get; set; }

        public Line()
        {
            
        }

        [Obsolete("Use default constructor and property setters instead.")]
        public Line(string left = null, string top = null, string width = null, string height = null,
                    Color? borderColor = null, string borderWidth = "1px")
        {
            Left = left != null ? UnitValue.Parse(left) : null;
            Top = top != null ? UnitValue.Parse(top) : null;
            Width = width != null ? UnitValue.Parse(width) : null;
            Height = height != null ? UnitValue.Parse(height) : null;

            BorderColor = borderColor ?? _defaultBorderColor;
            BorderWidth = borderWidth;
        }

        protected internal override void Render(PdfPage page, XRect parentBounds, DocumentData documentData, out XRect elementBounds, bool includeBackground, bool debug, PageNumberInfo pageNumberInfo)
        {
            elementBounds = GetBounds(parentBounds);

            if (includeBackground || !IsBackground)
            {
                using (var gfx = XGraphics.FromPdfPage(page))
                {
                    var borderWidth = UnitValue.Parse(BorderWidth);
                    var pen = new XPen(XColor.FromArgb(BorderColor), borderWidth.GetXUnitValue(0));

                    gfx.DrawLine(pen, elementBounds.Left, elementBounds.Top, elementBounds.Right, elementBounds.Bottom);
                }
            }
        }

        protected internal override XmlElement AppendXml(ref XmlElement xmePane)
        {
            throw new NotImplementedException();
        }
    }
}