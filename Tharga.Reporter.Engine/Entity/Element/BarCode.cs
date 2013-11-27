using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Text;
using System.Xml;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Tharga.Reporter.Engine.Entity.Area;
using Tharga.Reporter.Engine.Helper;

namespace Tharga.Reporter.Engine.Entity.Element
{
    public class BarCode : SinglePageAreaElement
    {
        private readonly Font _defaultFont = new Font();

        private string _code;
        private Font _font;
        private string _fontClass;

        public string Code { get { return _code ?? string.Empty; } set { _code = value; } }
        public Font Font
        {
            get { return _font ?? _defaultFont; }
            set
            {
                if (!string.IsNullOrEmpty(_fontClass)) throw new InvalidOperationException("Cannot set both Font and FontClass. FontClass has already been set.");
                _font = value;
            }
        }
        protected internal override void Render(PdfPage page, XRect parentBounds, DocumentData documentData, out XRect elementBounds, bool includeBackground, bool debug, PageNumberInfo pageNumberInfo, Section section)
        {
            var bounds = GetBounds(parentBounds);
            var imageData = GetImage(documentData, bounds, pageNumberInfo, section);
            elementBounds = bounds;

            if (includeBackground || !IsBackground)
            {
                using (var gfx = XGraphics.FromPdfPage(page))
                {
                    var legendFont = new XFont(GetFontName(section), GetFontSize(section), XFontStyle.Regular);
                    var legendBrush = new XSolidBrush(XColor.FromArgb(GetColor(section)));
                    var legendFontSize = gfx.MeasureString(Code, legendFont);

                    using (var image = XImage.FromGdiPlusImage(imageData))
                    {
                        gfx.DrawImage(image, new XRect(elementBounds.Left, elementBounds.Top, elementBounds.Width, elementBounds.Height - legendFontSize.Height));
                    }

                    //TODO: Possible to hide the text (Just show the barcode)
                    var code = GetCode(documentData, pageNumberInfo);
                    //gfx.DrawString(code, legendFont, legendBrush, elementBounds, XStringFormats.BottomCenter);
                    gfx.DrawString(code, legendFont, legendBrush, new XPoint(elementBounds.Left, elementBounds.Bottom - legendFontSize.Height), XStringFormats.TopLeft);
                }
            }

            imageData.Dispose();
        }

        private string GetCode(DocumentData documentData, PageNumberInfo pageNumberInfo)
        {
            //if (!string.IsNullOrEmpty(HideValue))
            //{
            //    var result = documentData.Get(HideValue);
            //    if (string.IsNullOrEmpty(result))
            //        return string.Empty;
            //}

            return Code.ParseValue(documentData, pageNumberInfo);
        }

        private string GetFontName(Section section)
        {
            if (_font != null)
                return _font.FontName;

            //TODO: Use font class

            return section.DefaultFont.FontName;
        }

        private double GetFontSize(Section section)
        {
            if (_font != null)
                return _font.Size;

            //TODO: Use font class

            return section.DefaultFont.Size;
        }

        private Color GetColor(Section section)
        {
            if (_font != null)
                return _font.Color;

            //TODO: Use font class

            return section.DefaultFont.Color;
        }

        private System.Drawing.Image GetImage(DocumentData documentData, XRect bounds, PageNumberInfo pageNumberInfo, Section section)
        {
            var code = GetCode(documentData, pageNumberInfo);

            const string filename = "FREE3OF9.TTF";

            if (!File.Exists(filename))
                throw new InvalidOperationException(string.Format("The file {0} cannot be found.", filename));

            var pfc = new PrivateFontCollection();
            pfc.AddFontFile(filename);
            var family = new FontFamily("Free 3 of 9", pfc);

            const float fontSize = 100;
            var c39Font = new System.Drawing.Font(family, fontSize);

            var matches = System.Text.RegularExpressions.Regex.Matches(code, @"[^A-Z0-9* \-$%./+]");
            if (matches.Count > 0)
            {
                var sb = new StringBuilder();
                sb.Append("Invalid characters '");
                foreach (System.Text.RegularExpressions.Group match in matches)
                    sb.AppendFormat(match.Value);
                sb.Append("' in code string.");
                throw new ArgumentException(sb.ToString());
            }

            SizeF barCodeSize;
            var tmp = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
            using (var objGraphics = Graphics.FromImage(tmp))
            {
                barCodeSize = objGraphics.MeasureString(code, c39Font);
            }

            if (Math.Abs(barCodeSize.Height - 0) < 0.001)
                return tmp;

            //TODO: Possible to have specific color for the barcode
            //var brush = new SolidBrush(Color.Black);
            var brush = new SolidBrush(GetColor(section));

            var bmp = new Bitmap((int)barCodeSize.Width, (int)barCodeSize.Height, PixelFormat.Format32bppArgb);
            using (var objGraphics = Graphics.FromImage(bmp))
            {
                objGraphics.DrawString(code, c39Font, brush, 0, 0, StringFormat.GenericTypographic);
            }
            return bmp;
        }

        private static int XCentered(int localWidth, int globalWidth)
        {
            return ((globalWidth - localWidth) / 2);
        }

        private static XRect GetImageBounds(System.Drawing.Image imageData, XRect bounds)
        {
            var imageBounds = bounds;
            if (Math.Abs(imageBounds.Width / imageBounds.Height - imageData.Width / (double)imageData.Height) > 0.01)
            {
                if ((imageBounds.Width / imageBounds.Height - imageData.Width / (double)imageData.Height) > 0)
                    imageBounds.Width = (imageBounds.Height * imageData.Width) / imageData.Height;
                else
                    imageBounds.Height = (imageBounds.Width * imageData.Height) / imageData.Width;
            }
            return imageBounds;
        }

        internal override XmlElement ToXme()
        {
            var xme = base.ToXme();

            if (_code != null)
                xme.SetAttribute("Code", _code);

            if (_font != null)
            {
                var fontXme = _font.ToXme();
                var importedFont = xme.OwnerDocument.ImportNode(fontXme, true);
                xme.AppendChild(importedFont);
            }

            return xme;
        }

        internal static BarCode Load(XmlElement xme)
        {
            var item = new BarCode();
            item.AppendData(xme);

            var xmlCode = xme.Attributes["Code"];
            if (xmlCode != null)
                item.Code = xmlCode.Value;

            foreach (XmlElement child in xme)
            {
                switch (child.Name)
                {
                    case "Font":
                        item.Font = Font.Load(child);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(string.Format("Unknown subelement {0} to text base.", child.Name));
                }
            }

            return item;
        }
    }
}