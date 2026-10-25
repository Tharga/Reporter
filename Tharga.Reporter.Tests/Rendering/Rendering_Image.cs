using System.Net;
using System.Net.Sockets;
using AwesomeAssertions;
using Moq;
using PdfSharp.Drawing;
using Tharga.Reporter.Entity;
using Tharga.Reporter.Entity.Element;
using Tharga.Reporter.Entity.Util;
using Tharga.Reporter.Interface;
using Xunit;

namespace Tharga.Reporter.Tests.Rendering;

public class Rendering_Image : IDisposable
{
    private const string MissingPath = "missing";

    private static readonly byte[] Png = PngWriter.FromGreyscale(new byte[,] { { 0, 255 }, { 255, 0 } });

    private readonly LocalHttpServer _server = new(Png, MissingPath);
    private readonly List<string> _cacheFiles = new();

    [Fact]
    public void A_url_is_downloaded_into_the_cache()
    {
        //Act
        var exists = Image.WebResourceExists(_server.UrlFor(UniqueName()), out var cacheFileName);

        //Assert
        _cacheFiles.Add(cacheFileName);
        exists.Should().BeTrue();
        File.ReadAllBytes(cacheFileName).Should().Equal(Png);
    }

    [Fact]
    public void A_cached_download_is_reused_without_a_second_request()
    {
        //Arrange
        var url = _server.UrlFor(UniqueName());
        Image.WebResourceExists(url, out var cacheFileName);
        _cacheFiles.Add(cacheFileName);

        //Act
        var exists = Image.WebResourceExists(url, out var secondCacheFileName);

        //Assert
        exists.Should().BeTrue();
        secondCacheFileName.Should().Be(cacheFileName);
        _server.RequestCount.Should().Be(1);
    }

    [Fact]
    public void A_url_with_a_port_gets_a_valid_cache_file_name()
    {
        //Act
        Image.WebResourceExists(_server.UrlFor($"{UniqueName()}?size=2&mode=grey"), out var cacheFileName);

        //Assert
        _cacheFiles.Add(cacheFileName);
        Path.GetFileName(cacheFileName).IndexOfAny(Path.GetInvalidFileNameChars()).Should().Be(-1);
        Path.GetDirectoryName(cacheFileName).Should().Be(Path.GetDirectoryName(Path.GetTempPath()));
    }

    [Fact]
    public void An_http_error_status_is_reported_as_missing_and_leaves_no_cache_file()
    {
        //Act
        var exists = Image.WebResourceExists(_server.UrlFor($"{MissingPath}/{UniqueName()}"), out var cacheFileName);

        //Assert
        _cacheFiles.Add(cacheFileName);
        exists.Should().BeFalse();
        File.Exists(cacheFileName).Should().BeFalse();
    }

    [Fact]
    public void An_unreachable_host_is_reported_as_missing()
    {
        //Arrange
        var url = $"http://localhost:{LocalHttpServer.GetFreePort()}/{UniqueName()}";

        //Act
        var exists = Image.WebResourceExists(url, out var cacheFileName);

        //Assert
        _cacheFiles.Add(cacheFileName);
        exists.Should().BeFalse();
        File.Exists(cacheFileName).Should().BeFalse();
    }

    [Fact]
    public void A_file_url_reads_the_local_file()
    {
        //Arrange
        var path = Path.Combine(Path.GetTempPath(), $"{UniqueName()}.png");
        File.WriteAllBytes(path, Png);
        _cacheFiles.Add(path);

        //Act
        var exists = Image.WebResourceExists(new Uri(path).AbsoluteUri, out var cacheFileName);

        //Assert
        exists.Should().BeTrue();
        cacheFileName.Should().Be(path);
    }

    [Fact]
    public void A_source_that_is_not_a_url_is_not_downloaded()
    {
        //Act
        var exists = Image.WebResourceExists("not-a-url", out var cacheFileName);

        //Assert
        exists.Should().BeFalse();
        cacheFileName.Should().BeNull();
        _server.RequestCount.Should().Be(0);
    }

    [Fact]
    public void Render_draws_an_image_downloaded_from_a_url()
    {
        //Arrange
        var url = _server.UrlFor($"{UniqueName()}.png");
        Image.WebResourceExists(url, out var cacheFileName);
        _cacheFiles.Add(cacheFileName);
        var image = new Image { Source = url, Width = "10cm", Height = "10cm" };
        var drawnSizes = new List<(int Width, int Height)>();
        var graphicsMock = new Mock<IGraphics>(MockBehavior.Strict);
        graphicsMock.Setup(x => x.DrawImage(It.IsAny<XImage>(), It.IsAny<XRect>()))
            .Callback<XImage, XRect>((drawn, _) => drawnSizes.Add((drawn.PixelWidth, drawn.PixelHeight)));
        var renderData = new RenderData(graphicsMock.Object, new XRect(0, 0, 600, 600), new Section(), null, new PageNumberInfo(1, 1), null, true);

        //Act
        image.Render(renderData);

        //Assert
        drawnSizes.Should().Equal((2, 2));
    }

    public void Dispose()
    {
        _server.Dispose();
        foreach (var file in _cacheFiles.Where(x => x != null))
        {
            File.Delete(file);
        }
    }

    private static string UniqueName()
    {
        return $"reporter-test-{Guid.NewGuid():N}";
    }

    private sealed class LocalHttpServer : IDisposable
    {
        private readonly HttpListener _listener = new();
        private readonly byte[] _content;
        private readonly string _missingPath;
        private readonly int _port = GetFreePort();
        private int _requestCount;

        public LocalHttpServer(byte[] content, string missingPath)
        {
            _content = content;
            _missingPath = missingPath;
            _listener.Prefixes.Add($"http://localhost:{_port}/");
            _listener.Start();
            _ = Task.Run(ServeAsync);
        }

        public int RequestCount => Volatile.Read(ref _requestCount);

        public string UrlFor(string pathAndQuery)
        {
            return $"http://localhost:{_port}/{pathAndQuery}";
        }

        public static int GetFreePort()
        {
            var probe = new TcpListener(IPAddress.Loopback, 0);
            probe.Start();
            var port = ((IPEndPoint)probe.LocalEndpoint).Port;
            probe.Stop();
            return port;
        }

        private async Task ServeAsync()
        {
            while (_listener.IsListening)
            {
                HttpListenerContext context;
                try
                {
                    context = await _listener.GetContextAsync();
                }
                catch (Exception exception) when (exception is HttpListenerException or ObjectDisposedException)
                {
                    return;
                }

                Interlocked.Increment(ref _requestCount);
                using var response = context.Response;
                if (context.Request.RawUrl?.StartsWith($"/{_missingPath}/", StringComparison.Ordinal) == true)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    continue;
                }

                response.ContentType = "image/png";
                await response.OutputStream.WriteAsync(_content);
            }
        }

        public void Dispose()
        {
            _listener.Close();
        }
    }
}
