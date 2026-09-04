using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text;
using System.Xml;
using PdfSharp.Drawing;
using Tharga.Reporter.Entity.Element.Base;
using Tharga.Reporter.Extensions;
using Tharga.Reporter.Interface;

namespace Tharga.Reporter.Entity.Element;

public class Image : SinglePageAreaElement
{
    private const string MissingImageFontName = "Verdana";
    private const double MissingImageFontSize = 10;

    private string _source;

    public string Source
    {
        get => _source ?? string.Empty;
        set => _source = value;
    }

    internal override void Render(IRenderData renderData)
    {
        if (IsNotVisible(renderData)) return;

        var bounds = GetBounds(renderData.ParentBounds);
        var source = Source.ParseValue(renderData.DocumentData, null, false);

        using var imageData = GetImage(source);

        renderData.ElementBounds = imageData == null ? bounds : GetImageBounds(imageData, bounds);

        if (!renderData.IncludeBackground && IsBackground) return;

        if (imageData == null)
        {
            RenderMissingImage(renderData, source);
            return;
        }

        renderData.Graphics.DrawImage(imageData, renderData.ElementBounds);
    }

    private static XRect GetImageBounds(XImage imageData, XRect bounds)
    {
        var imageBounds = bounds;
        if (Math.Abs(imageBounds.Width / imageBounds.Height - imageData.PixelWidth / (double)imageData.PixelHeight) > 0.01)
        {
            if (imageBounds.Width / imageBounds.Height - imageData.PixelWidth / (double)imageData.PixelHeight > 0)
            {
                imageBounds.Width = imageBounds.Height * imageData.PixelWidth / imageData.PixelHeight;
            }
            else
            {
                imageBounds.Height = imageBounds.Width * imageData.PixelHeight / imageData.PixelWidth;
            }
        }

        return imageBounds;
    }

    private static void RenderMissingImage(IRenderData renderData, string source)
    {
        var bounds = renderData.ElementBounds;
        var pen = new XPen(XColor.FromKnownColor(XKnownColor.Red));

        renderData.Graphics.DrawLine(pen, bounds.Left, bounds.Top, bounds.Right, bounds.Bottom);
        renderData.Graphics.DrawLine(pen, bounds.Right, bounds.Top, bounds.Left, bounds.Bottom);

        var font = new XFont(MissingImageFontName, MissingImageFontSize);
        var brush = new XSolidBrush(XColor.FromKnownColor(XKnownColor.DarkRed));

        renderData.Graphics.DrawString($"Image '{source}' is missing.", font, brush, bounds, XStringFormats.TopLeft);
    }

    public static string BytesToLongString(byte[] bytes)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < bytes.Length; i++)
        {
            sb.AppendFormat("{0}/", bytes[i].ToString(CultureInfo.InvariantCulture));
        }

        var imgData = sb.ToString();
        return imgData;
    }

    private static byte[] LongStringToBytes(string source)
    {
        var stringParts = source.Split('/');
        var bytes = new byte[stringParts.Length - 1];
        for (var i = 0; i < bytes.Length; i++)
        {
            bytes[i] = (byte)int.Parse(stringParts[i]);
        }

        return bytes;
    }

    private static XImage GetImage(string source)
    {
        if (string.IsNullOrEmpty(source)) return null;

        if (File.Exists(source))
        {
            return XImage.FromFile(source);
        }

        if (WebResourceExists(source, out var cacheFileName))
        {
            return XImage.FromFile(cacheFileName);
        }

        try
        {
            return XImage.FromStream(new MemoryStream(LongStringToBytes(source)));
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception.Message);
            return null;
        }
    }

    private static bool WebResourceExists(string imageUrl, out string cacheFileName)
    {
        cacheFileName = null;

        if (string.IsNullOrEmpty(imageUrl) || !imageUrl.Contains("://"))
        {
            return false;
        }

        var localName = imageUrl.Substring(imageUrl.IndexOf(":", StringComparison.Ordinal) + 3).Replace("/", "_").Replace("?", "_").Replace("=", "_").Replace("&", "_");
        cacheFileName = string.Format("{0}{1}", Path.GetTempPath(), localName);

        if (!File.Exists(cacheFileName))
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile(imageUrl, cacheFileName);
                }
            }
            catch (WebException)
            {
                File.Delete(cacheFileName);
                return false;
            }
        }

        return true;
    }

    internal override XmlElement ToXme()
    {
        var xme = base.ToXme();

        if (_source != null)
        {
            xme.SetAttribute("Source", _source);
        }

        return xme;
    }

    internal static Image Load(XmlElement xme)
    {
        var image = new Image();

        image.AppendData(xme);

        var xmlSource = xme.Attributes["Source"];
        if (xmlSource != null)
        {
            image.Source = xmlSource.Value;
        }

        return image;
    }
}
