using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Tharga.Reporter.Interface;

namespace Tharga.Reporter;

internal class MyGraphicsFactory : IGraphicsFactory
{
    public IGraphics PrepareGraphics(PdfPage page)
    {
        return new MyGraphics(XGraphics.FromPdfPage(page));
    }
}
