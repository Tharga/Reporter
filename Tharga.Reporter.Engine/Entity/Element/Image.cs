using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Tharga.Reporter.Engine.Entity.Area;
using Tharga.Reporter.Engine.Helper;

namespace Tharga.Reporter.Engine.Entity.Element
{
    public class Image : SinglePageAreaElement
    {
        private string _source;

        public string Source { get { return _source ?? string.Empty; } set { _source = value; } }

        protected internal override void Render(PdfPage page, XRect parentBounds, DocumentData documentData,
            out XRect elementBounds, bool includeBackground, bool debug, PageNumberInfo pageNumberInfo)
        {
            var bounds = GetBounds(parentBounds);
            var imageData = GetImage(documentData, bounds);
            elementBounds = GetImageBounds(imageData, bounds);

            if (includeBackground || !IsBackground)
            {
                using (var gfx = XGraphics.FromPdfPage(page))
                {
                    using (var image = XImage.FromGdiPlusImage(imageData))
                    {
                        gfx.DrawImage(image, elementBounds);
                    }
                }
            }

            imageData.Dispose();
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

        public static string BytesToLongString(byte[] bytes)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < bytes.Length; i++)
                sb.AppendFormat("{0}/", bytes[i].ToString(CultureInfo.InvariantCulture));
            var imgData = sb.ToString();
            return imgData;
        }

        private static byte[] LongStringToBytes(string source)
        {
            var stringParts = source.Split('/');
            var bytes = new byte[stringParts.Length - 1];
            for (var i = 0; i < bytes.Length; i++)
                bytes[i] = (byte)int.Parse(stringParts[i]);
            return bytes;
        }

        private System.Drawing.Image GetImage(DocumentData documentData, XRect bounds)
        {
            var source = Source.ParseValue(documentData, null, false);

            System.Drawing.Image imageData = null;
            if (File.Exists(source))
            {
                imageData = System.Drawing.Image.FromFile(source);
            }
            else if (!string.IsNullOrEmpty(source))
            {
                try
                {
                    using (var stream = new MemoryStream(LongStringToBytes(source)))
                    {
                        using (var image = System.Drawing.Image.FromStream(stream))
                        {
                            imageData = new Bitmap(image.Width, image.Height);
                            using (var gfx = Graphics.FromImage(imageData))
                            {
                                gfx.DrawImage(image, 0, 0, image.Width, image.Height);
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    System.Diagnostics.Debug.WriteLine(exception.Message);
                }
            }

            if (imageData == null)
            {
                imageData = new Bitmap((int)bounds.Width, (int)bounds.Height);
                using (var gfx = Graphics.FromImage(imageData))
                {
                    var pen = new Pen(Color.Red);
                    gfx.DrawLine(pen, 0, 0, imageData.Width, imageData.Height);
                    gfx.DrawLine(pen, imageData.Width, 0, 0, imageData.Height);
                    var font = new System.Drawing.Font("Verdana", 10);
                    var brush = new SolidBrush(Color.DarkRed);
                    gfx.DrawString(string.Format("Image '{0}' is missing.", source), font, brush, 0, 0);
                }
            }

            return imageData;
        }

        internal override XmlElement ToXme()
        {
            var xme = base.ToXme();

            if (_source != null)
                xme.SetAttribute("Source", _source);

            return xme;
        }

        internal static Image Load(XmlElement xme)
        {
            var image = new Image();
            
            image.AppendData(xme);

            var xmlSource = xme.Attributes["Source"];
            if (xmlSource != null)
                image.Source = xmlSource.Value;

            return image;
        }
    }
}