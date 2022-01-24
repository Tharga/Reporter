using MigraDoc.Rendering;
using PdfSharp.Pdf;

namespace Tharga.Reporter.Interface;

internal interface IGraphicsFactory
{
    IGraphics PrepareGraphics(PdfPage page, DocumentRenderer docRenderer, int ii);
}