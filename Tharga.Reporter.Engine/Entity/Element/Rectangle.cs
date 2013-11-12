using System;
using System.Drawing;
using System.Xml;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Tharga.Reporter.Engine.Entity.Area;

namespace Tharga.Reporter.Engine.Entity.Element
{
    public sealed class Rectangle : SinglePageElement
    {
        private readonly Color _defaultBorderColor = Color.Black;

        public Color BorderColor { get; set; }
        //private string BorderWidth { get; set; }
        public UnitValue BorderWidth { get; set; }
        public Color? BackgroundColor { get; set; }


        public Rectangle()
        {
            BorderWidth = UnitValue.Parse("1px");
        }

        [Obsolete("Use default constructor and property setters instead.")]
        public Rectangle(string left = null, string top = null, string width = null, string height = null,
            Color? borderColor = null, string borderWidth = "1px", Color? backgroundColor = null)
        {
            Left = left != null ? UnitValue.Parse(left) : (UnitValue?)null;
            Top = top != null ? UnitValue.Parse(top) : (UnitValue?)null;
            Width = width != null ? UnitValue.Parse(width) : (UnitValue?)null;
            Height = height != null ? UnitValue.Parse(height) : (UnitValue?)null;

            BorderColor = borderColor ?? _defaultBorderColor;
            BackgroundColor = backgroundColor;
            BorderWidth = UnitValue.Parse(borderWidth);
        }

        //internal Rectangle(XmlElement xmlElement)
        //    : base(xmlElement)
        //{
        //    if ( xmlElement.Attributes.GetNamedItem("Color") != null)
        //    {
        //        int result;
        //        var value = xmlElement.Attributes.GetNamedItem("Color").Value;
        //        if ( !int.TryParse(value,out result))
        //            throw new InvalidOperationException(string.Format("Cannot parse {0} as an int.", value));
        //        BorderColor = Color.FromArgb(result);
        //    }
        //    else
        //    {
        //        BorderColor = _defaultBorderColor;
        //    }
        //}

        protected internal override void Render(PdfPage page, XRect parentBounds, DocumentData documentData,
            out XRect elementBounds, bool includeBackground, bool debug, PageNumberInfo pageNumberInfo)
        {
            elementBounds = GetBounds(parentBounds);

            if (includeBackground || !IsBackground)
            {
                using (var gfx = XGraphics.FromPdfPage(page))
                {
                    //var borderWidth = UnitValue.Parse(BorderWidth);
                    var pen = new XPen(XColor.FromArgb(BorderColor), BorderWidth.GetXUnitValue(0));

                    if (BackgroundColor != null)
                    {
                        var brush = new XSolidBrush(XColor.FromArgb(BackgroundColor.Value));
                        gfx.DrawRectangle(pen, brush, elementBounds);
                    }
                    else
                        gfx.DrawRectangle(pen, elementBounds);
                }
            }
        }

        public static Rectangle Load(XmlElement xme)
        {
            var rectangle = new Rectangle();

            rectangle.AppendData(xme);

            var xmlBackgroundColor = xme.Attributes["BackgroundColor"];
            if (xmlBackgroundColor != null)
                rectangle.BackgroundColor = ToColor(xmlBackgroundColor.Value);

            var xmlBorderColor = xme.Attributes["BorderColor"];
            if (xmlBorderColor != null)
                rectangle.BorderColor = ToColor(xmlBorderColor.Value);


            return rectangle;
        }
    }
}