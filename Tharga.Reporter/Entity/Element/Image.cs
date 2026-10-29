using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
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
    private const char CacheNameReplacement = '_';
    private const string DownloadExtension = ".download";

    private static readonly HttpClient SharedHttpClient = new(new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(2) });
    private static readonly char[] InvalidCacheNameChars = Path.GetInvalidFileNameChars().Concat(['?', '=', '&']).ToArray();
    private static readonly string CacheFolder = Path.Combine(Path.GetTempPath(), "Tharga.Reporter", "Images");
    private static readonly ConcurrentDictionary<string, object> CacheLocks = new();

    private string _source;

    /// <summary>
    /// How long an image downloaded from an <c>http://</c> or <c>https://</c> source is reused before it is
    /// downloaded again. The default is one hour; <see cref="TimeSpan.Zero"/> downloads the image on every render.
    /// When a new download fails, the copy already cached is used.
    /// </summary>
    public static TimeSpan CacheDuration { get; set; } = TimeSpan.FromHours(1);

    public string Source
    {
        get => _source ?? string.Empty;
        set => _source = value;
    }

    internal override void Render(IRenderData renderData)
    {
        if (IsNotVisible(renderData)) return;

        var bounds = GetBounds(renderData);
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
            return IsCacheFile(cacheFileName) ? LoadCachedImage(source, cacheFileName) : XImage.FromFile(cacheFileName);
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

    internal static bool WebResourceExists(string imageUrl, out string cacheFileName)
    {
        cacheFileName = null;

        if (string.IsNullOrEmpty(imageUrl) || !imageUrl.Contains("://"))
        {
            return false;
        }

        if (Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri) && uri.IsFile)
        {
            cacheFileName = uri.LocalPath;
            return File.Exists(cacheFileName);
        }

        cacheFileName = Path.Combine(CacheFolder, GetCacheName(imageUrl));

        return IsFresh(cacheFileName) || TryDownload(imageUrl, cacheFileName);
    }

    private static bool IsCacheFile(string fileName)
    {
        return string.Equals(Path.GetDirectoryName(fileName), CacheFolder, StringComparison.Ordinal);
    }

    private static bool IsFresh(string cacheFileName)
    {
        return File.Exists(cacheFileName) && DateTime.UtcNow - File.GetLastWriteTimeUtc(cacheFileName) < CacheDuration;
    }

    private static XImage LoadCachedImage(string imageUrl, string cacheFileName)
    {
        var image = TryLoad(cacheFileName);
        if (image != null) return image;

        lock (GetCacheLock(cacheFileName))
        {
            TryDelete(cacheFileName);
        }

        return TryDownload(imageUrl, cacheFileName) ? TryLoad(cacheFileName) : null;
    }

    private static XImage TryLoad(string fileName)
    {
        try
        {
            return XImage.FromStream(new MemoryStream(File.ReadAllBytes(fileName)));
        }
        catch (Exception exception) when (exception is not OutOfMemoryException)
        {
            Debug.WriteLine(exception.Message);
            return null;
        }
    }

    private static string GetCacheName(string imageUrl)
    {
        var name = imageUrl.Substring(imageUrl.IndexOf(":", StringComparison.Ordinal) + 3);
        return string.Concat(name.Select(x => InvalidCacheNameChars.Contains(x) ? CacheNameReplacement : x));
    }

    /// <summary>
    /// Downloads to a name of its own and then moves the file into place, so the cache name never holds a partly
    /// written file. Downloads of one URL are serialised, so concurrent renders share a single download.
    /// </summary>
    private static bool TryDownload(string imageUrl, string cacheFileName)
    {
        lock (GetCacheLock(cacheFileName))
        {
            if (IsFresh(cacheFileName)) return true;

            var downloadFileName = $"{cacheFileName}.{Guid.NewGuid():N}{DownloadExtension}";
            try
            {
                Directory.CreateDirectory(CacheFolder);
                DownloadAsync(imageUrl, downloadFileName).GetAwaiter().GetResult();
                File.Move(downloadFileName, cacheFileName, true);
                return true;
            }
            catch (Exception exception) when (exception is not OutOfMemoryException)
            {
                Debug.WriteLine(exception.Message);
                TryDelete(downloadFileName);
                return File.Exists(cacheFileName);
            }
        }
    }

    private static object GetCacheLock(string cacheFileName)
    {
        return CacheLocks.GetOrAdd(cacheFileName, _ => new object());
    }

    private static void TryDelete(string fileName)
    {
        try
        {
            File.Delete(fileName);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            Debug.WriteLine(exception.Message);
        }
    }

    private static async Task DownloadAsync(string imageUrl, string fileName)
    {
        using var response = await SharedHttpClient.GetAsync(imageUrl).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        await using var file = File.Create(fileName);
        await response.Content.CopyToAsync(file).ConfigureAwait(false);
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
