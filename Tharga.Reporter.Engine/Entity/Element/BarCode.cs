using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Xml;
using Aspose.BarCode;
using PdfSharp.Drawing;
using Tharga.Reporter.Engine.Entity.Area;
using Tharga.Reporter.Engine.Interface;

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

        internal override void Render(IRenderData renderData)
        {
            if ( string.IsNullOrEmpty(Code))
                throw new InvalidOperationException("Code has not been set.");

            if (IsNotVisible(renderData)) return;

            var bounds = GetBounds(renderData.ParentBounds);

            renderData.ElementBounds = bounds;

            if (!IsBackground || renderData.IncludeBackground)
            {
                var b = new BarCodeBuilder { SymbologyType = Symbology.Code39Standard, CodeText = GetCode(renderData.DocumentData, renderData.PageNumberInfo) };
                var memStream = new MemoryStream();
                b.BarCodeImage.Save(memStream, ImageFormat.Png);
                var imageData = System.Drawing.Image.FromStream(memStream);

                //Paint over the license info
                using (var g = Graphics.FromImage(imageData))
                {
                    g.FillRectangle(new SolidBrush(b.BackColor), 0, 0, imageData.Width, 14);
                }

                using (var image = XImage.FromGdiPlusImage(imageData))
                {
                    renderData.Graphics.DrawImage(image, new XRect(renderData.ElementBounds.Left, renderData.ElementBounds.Top, renderData.ElementBounds.Width, renderData.ElementBounds.Height)); // - legendFontSize.Height));
                }

                imageData.Dispose();
            }
        }

        private string GetCode(IDocumentData documentData, PageNumberInfo pageNumberInfo)
        {
            return Code.ParseValue(documentData, pageNumberInfo);
        }

        //private System.Drawing.Image GetImage(DocumentData documentData, XRect bounds, PageNumberInfo pageNumberInfo, Section section)
        //{
        //    //var code = GetCode(documentData, pageNumberInfo);

        //    //const string filename = "FREE3OF9.TTF";

        //    //if (!File.Exists(filename))
        //    //    throw new InvalidOperationException(string.Format("The file {0} cannot be found.", filename));

        //    //var pfc = new PrivateFontCollection();
        //    //pfc.AddFontFile(filename);
        //    //var family = new FontFamily("Free 3 of 9", pfc);

        //    //const float fontSize = 100;
        //    //var c39Font = new System.Drawing.Font(family, fontSize);

        //    //var matches = System.Text.RegularExpressions.Regex.Matches(code, @"[^A-Z0-9* \-$%./+]");
        //    //if (matches.Count > 0)
        //    //{
        //    //    var sb = new StringBuilder();
        //    //    sb.Append("Invalid characters '");
        //    //    foreach (System.Text.RegularExpressions.Group match in matches)
        //    //        sb.AppendFormat(match.Value);
        //    //    sb.Append("' in code string.");
        //    //    throw new ArgumentException(sb.ToString());
        //    //}

        //    //SizeF barCodeSize;
        //    //var tmp = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
        //    //using (var objGraphics = Graphics.FromImage(tmp))
        //    //{
        //    //    barCodeSize = objGraphics.MeasureString(code, c39Font);
        //    //}

        //    //if (Math.Abs(barCodeSize.Height - 0) < 0.001)
        //    //    return tmp;

        //    ////TODO: Possible to have specific color for the barcode
        //    ////var brush = new SolidBrush(Color.Black);
        //    //var brush = new SolidBrush(_font.GetColor(section));

        //    //var bmp = new Bitmap((int)barCodeSize.Width, (int)barCodeSize.Height, PixelFormat.Format32bppArgb);
        //    //using (var objGraphics = Graphics.FromImage(bmp))
        //    //{
        //    //    objGraphics.DrawString(code, c39Font, brush, 0, 0, StringFormat.GenericTypographic);
        //    //}
        //    //return bmp;
        //    throw new NotImplementedException();
        //}

        //private static int XCentered(int localWidth, int globalWidth)
        //{
        //    return ((globalWidth - localWidth) / 2);
        //}

        //private static XRect GetImageBounds(System.Drawing.Image imageData, XRect bounds)
        //{
        //    var imageBounds = bounds;
        //    if (Math.Abs(imageBounds.Width / imageBounds.Height - imageData.Width / (double)imageData.Height) > 0.01)
        //    {
        //        if ((imageBounds.Width / imageBounds.Height - imageData.Width / (double)imageData.Height) > 0)
        //            imageBounds.Width = (imageBounds.Height * imageData.Width) / imageData.Height;
        //        else
        //            imageBounds.Height = (imageBounds.Width * imageData.Height) / imageData.Width;
        //    }
        //    return imageBounds;
        //}

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