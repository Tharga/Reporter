using PdfSharp.Pdf;

namespace Tharga.Reporter.Interface;

internal interface IGraphicsFactory
{
    IGraphics PrepareGraphics(PdfPage page);
}
