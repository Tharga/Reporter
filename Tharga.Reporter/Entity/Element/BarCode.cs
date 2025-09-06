using System.Drawing;
using System.Drawing.Imaging;
using System.Xml;
using Aspose.BarCode.Generation;
using PdfSharp.Drawing;
using Tharga.Reporter.Entity.Element.Base;
using Tharga.Reporter.Entity.Util;
using Tharga.Reporter.Extensions;
using Tharga.Reporter.Interface;

namespace Tharga.Reporter.Entity.Element;

public class BarCode : SinglePageAreaElement
{
    private string _code;

    public string Code
    {
        get => _code ?? string.Empty;
        set => _code = value;
    }

    internal override void Render(IRenderData renderData)
    {
        if (string.IsNullOrEmpty(Code))
        {
            throw new InvalidOperationException("Code has not been set.");
        }

        if (IsNotVisible(renderData)) return;

        var bounds = GetBounds(renderData.ParentBounds);

        renderData.ElementBounds = bounds;

        if (!IsBackground || renderData.IncludeBackground)
        {
            //var generatorA = new BarcodeGenerator(EncodeTypes.Code128, "1234567");
            //generatorA.Parameters.Barcode.XDimension.Millimeters = 1f;
            //generatorA.Save("c:\\temp\\output.jpg", BarCodeImageFormat.Jpeg);

            //var b = new BarCodeBuilder { SymbologyType = Symbology.Code39Standard, CodeText = GetCode(renderData.DocumentData, renderData.PageNumberInfo) };
            var generator = new BarcodeGenerator(EncodeTypes.Code39, GetCode(renderData.DocumentData, renderData.PageNumberInfo));
            var memStream = new MemoryStream();
            //b.BarCodeImage.Save(memStream, ImageFormat.Png);
            generator.Save(memStream, BarCodeImageFormat.Png);
            var imageData = System.Drawing.Image.FromStream(memStream);

            //NOTE: Paint over the license info
            //using (var g = Graphics.FromImage(imageData))
            //{
            //    g.FillRectangle(new SolidBrush(generator.Parameters.BackColor), 0, 0, imageData.Width, 14);
            //}

            //renderData.ElementBounds = new XRect(renderData.ElementBounds.Left, renderData.ElementBounds.Top, imageData.Width, imageData.Height);
            //renderData.ElementBounds = new XRect(renderData.ElementBounds.Left, renderData.ElementBounds.Top, raw.Width / 2, raw.Height * 10);

            //using (var image = XImage.FromGdiPlusImage(imageData))
            //{
            //    renderData.Graphics.DrawImage(image, new XRect(renderData.ElementBounds.Left, renderData.ElementBounds.Top, renderData.ElementBounds.Width, renderData.ElementBounds.Height)); // - legendFontSize.Height));
            //}

            //NOTE: Create a new image 1 pixel height, and paint an untouched part of the barcode on that.
            var targetImageHeight = 1;
            var raw = new Bitmap(imageData, new Size(imageData.Width, targetImageHeight));
            using (var g = Graphics.FromImage(raw))
            {
                g.DrawImage(imageData,
                    new[] { new PointF(0, 0), new PointF(imageData.Width, 0), new PointF(0, targetImageHeight) },
                    new RectangleF(0, 10, imageData.Width, 1), GraphicsUnit.Pixel);
            }

            {
                using var strm = new MemoryStream();
                //imageData.Save(strm, ImageFormat.Png);
                raw.Save(strm, ImageFormat.Png);
                using var xfoto = XImage.FromStream(strm);
                renderData.Graphics.DrawImage(xfoto, new XRect(renderData.ElementBounds.Left, renderData.ElementBounds.Top, renderData.ElementBounds.Width, renderData.ElementBounds.Height)); // - legendFontSize.Height));
            }

            imageData.Dispose();
            raw.Dispose();
        }
    }

    private string GetCode(IDocumentData documentData, PageNumberInfo pageNumberInfo)
    {
        return Code.ParseValue(documentData, pageNumberInfo);
    }

    internal override XmlElement ToXme()
    {
        var xme = base.ToXme();

        if (_code != null)
        {
            xme.SetAttribute("Code", _code);
        }

        return xme;
    }

    internal static BarCode Load(XmlElement xme)
    {
        var item = new BarCode();
        item.AppendData(xme);

        var xmlCode = xme.Attributes["Code"];
        if (xmlCode != null)
        {
            item.Code = xmlCode.Value;
        }

        return item;
    }
}