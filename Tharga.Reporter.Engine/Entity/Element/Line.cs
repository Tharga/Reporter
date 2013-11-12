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

        private Color? _borderColor;

        public Color BorderColor { get { return _borderColor ?? _defaultBorderColor; } set { _borderColor = value; } }
        private string BorderWidth { get; set; } //TODO: Make this configurable

        //public Line()
        //{
        //    //BorderColor = _defaultBorderColor;
        //}

        //[Obsolete("Use default constructor and property setters instead.")]
        //public Line(string left = null, string top = null, string width = null, string height = null,
        //            Color? borderColor = null, string borderWidth = "1px")
        //{
        //    Left = left != null ? UnitValue.Parse(left) : null;
        //    Top = top != null ? UnitValue.Parse(top) : null;
        //    Width = width != null ? UnitValue.Parse(width) : null;
        //    Height = height != null ? UnitValue.Parse(height) : null;

        //    BorderColor = borderColor ?? _defaultBorderColor;
        //    BorderWidth = borderWidth;
        //}

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

        internal override XmlElement ToXme()
        {
            var xme = base.ToXme();

            if (_borderColor != null)
                xme.SetAttribute("BorderColor", string.Format("{0}{1}{2}", _borderColor.Value.R.ToString("X2"), _borderColor.Value.G.ToString("X2"), _borderColor.Value.B.ToString("X2")));

            if (_isBackground != null)
                xme.SetAttribute("IsBackground", IsBackground.ToString());

            if (_name != null)
                xme.SetAttribute("Name", Name);

            return xme;
        }

        public static Line Load(XmlElement xme)
        {
            var line = new Line();

            line.AppendData(xme);

            var xmlBorderColor = xme.Attributes["BorderColor"];
            if (xmlBorderColor != null)
                line.BorderColor = ToColor(xmlBorderColor.Value);

            var xmlAttribute = xme.Attributes["IsBackground"];
            if (xmlAttribute != null)
                line.IsBackground = bool.Parse(xmlAttribute.Value);

            var xmlName = xme.Attributes["Name"];
            if (xmlName != null)
                line.Name = xmlName.Value;

            return line;
        }
    }
}