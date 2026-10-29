using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Tharga.Reporter.Interface;

namespace Tharga.Reporter.Tests.Rendering;

/// <summary>
/// Renders with real graphics, so text is measured with real fonts, and records every drawing call of the last
/// document seen. The pre-render pass works on a document of its own, so the calls recorded after
/// <see cref="Renderer.GetPdfBinary"/> returns are the ones of the PDF it produced.
/// </summary>
internal sealed class RecordingGraphicsFactory : IGraphicsFactory
{
    private PdfDocument _currentDocument;
    private int _documentNumber = -1;
    private int _pageIndex;

    public List<DrawCall> Calls { get; } = new();

    /// <summary>
    /// Called with the zero-based number of a document when its first page is prepared, before anything is drawn on it.
    /// </summary>
    public Action<int> DocumentStarted { get; init; }

    public IEnumerable<DrawnString> Strings => Calls.OfType<DrawnString>();
    public IEnumerable<DrawnLine> Lines => Calls.OfType<DrawnLine>();
    public IEnumerable<DrawnRectangle> Rectangles => Calls.OfType<DrawnRectangle>();

    public IGraphics PrepareGraphics(PdfPage page)
    {
        if (!ReferenceEquals(page.Owner, _currentDocument))
        {
            _currentDocument = page.Owner;
            _documentNumber++;
            _pageIndex = 0;
            Calls.Clear();
            DocumentStarted?.Invoke(_documentNumber);
        }
        else
        {
            _pageIndex++;
        }

        return new RecordingGraphics(new MyGraphics(XGraphics.FromPdfPage(page)), Calls, _pageIndex);
    }

    private sealed class RecordingGraphics : IGraphics
    {
        private readonly IGraphics _inner;
        private readonly List<DrawCall> _calls;
        private readonly int _page;

        public RecordingGraphics(IGraphics inner, List<DrawCall> calls, int page)
        {
            _inner = inner;
            _calls = calls;
            _page = page;
        }

        public XSize MeasureString(string s, XFont font) => _inner.MeasureString(s, font);
        public XSize MeasureString(string s, XFont font, XStringFormat format) => _inner.MeasureString(s, font, format);

        public void DrawString(string s, XFont font, XBrush brush, XPoint point)
        {
            _calls.Add(new DrawnString(_page, s, font, brush, point));
            _inner.DrawString(s, font, brush, point);
        }

        public void DrawString(string s, XFont font, XBrush brush, XPoint point, XStringFormat format)
        {
            _calls.Add(new DrawnString(_page, s, font, brush, point));
            _inner.DrawString(s, font, brush, point, format);
        }

        public void DrawString(string s, XFont font, XBrush brush, XRect rect, XStringFormat format)
        {
            _calls.Add(new DrawnString(_page, s, font, brush, rect.TopLeft));
            _inner.DrawString(s, font, brush, rect, format);
        }

        public void DrawLine(XPen pen, double x1, double y1, double x2, double y2)
        {
            _calls.Add(new DrawnLine(_page, pen, new XPoint(x1, y1), new XPoint(x2, y2)));
            _inner.DrawLine(pen, x1, y1, x2, y2);
        }

        public void DrawRectangle(XPen pen, XRect rect)
        {
            _calls.Add(new DrawnRectangle(_page, pen, null, rect));
            _inner.DrawRectangle(pen, rect);
        }

        public void DrawRectangle(XPen pen, XBrush brush, XRect rect)
        {
            _calls.Add(new DrawnRectangle(_page, pen, brush, rect));
            _inner.DrawRectangle(pen, brush, rect);
        }

        public void DrawRectangle(XBrush brush, XRect rect)
        {
            _calls.Add(new DrawnRectangle(_page, null, brush, rect));
            _inner.DrawRectangle(brush, rect);
        }

        public void DrawEllipse(XPen pen, double x, double y, int width, int height) => _inner.DrawEllipse(pen, x, y, width, height);
        public void DrawImage(XImage image, XRect rect) => _inner.DrawImage(image, rect);
    }
}

internal abstract record DrawCall(int Page);

internal sealed record DrawnString(int Page, string Text, XFont Font, XBrush Brush, XPoint Location) : DrawCall(Page);

internal sealed record DrawnLine(int Page, XPen Pen, XPoint From, XPoint To) : DrawCall(Page);

internal sealed record DrawnRectangle(int Page, XPen Pen, XBrush Brush, XRect Rect) : DrawCall(Page);
