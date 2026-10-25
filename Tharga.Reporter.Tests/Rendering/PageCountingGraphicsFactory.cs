using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Tharga.Reporter.Interface;

namespace Tharga.Reporter.Tests.Rendering;

/// <summary>
/// Renders with real graphics and reports how many pages the final document got. The pre-render pass works on
/// a document of its own, so the pages of the last document seen are the ones <see cref="Renderer.GetPdfBinary"/> returns.
/// </summary>
internal sealed class PageCountingGraphicsFactory : IGraphicsFactory
{
    private PdfDocument _currentDocument;

    public int RenderedPageCount { get; private set; }

    public IGraphics PrepareGraphics(PdfPage page)
    {
        if (!ReferenceEquals(page.Owner, _currentDocument))
        {
            _currentDocument = page.Owner;
            RenderedPageCount = 0;
        }

        RenderedPageCount++;
        return new MyGraphics(XGraphics.FromPdfPage(page));
    }
}
