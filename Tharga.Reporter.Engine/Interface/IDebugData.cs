using PdfSharp.Drawing;

namespace Tharga.Reporter.Interface
{
    internal interface IDebugData
    {
        XPen Pen { get; }
        XBrush Brush { get; }
        XFont Font { get; }
    }
}