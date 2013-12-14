using MigraDoc.Rendering;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace Tharga.Reporter.Engine.Interface
{
    internal interface IGraphicsFactory
    {
        IGraphics PrepareGraphics(PdfPage page, DocumentRenderer docRenderer, int ii);
    }

    internal interface IGraphics
    {
        XSize MeasureString(string code, XFont legendFont);
        XSize MeasureString(string code, XFont legendFont, XStringFormat topLeft);
        void DrawImage(XImage image, XRect xRect);
        void DrawString(string code, XFont legendFont, XSolidBrush legendBrush, XPoint xPoint, XStringFormat topLeft);
        void DrawString(string code, XFont legendFont, XSolidBrush legendBrush, XRect rect);
        void DrawString(string code, XFont legendFont, XSolidBrush legendBrush, double x, double y);
        void DrawString(string text, XFont font, XSolidBrush brush, XRect elementBounds, XStringFormat formats);
        void DrawString(string line, XFont font, XBrush brush, double left, double top, XStringFormat formats);
        void DrawString(string text, XFont font, XBrush brush, XPoint point);
        void DrawLine(XPen debugPen, double left, double p2, double p3, double height);
        void DrawRectangle(XPen debugPen, double left, double top, double width, double height);
        void DrawRectangle(XPen pen, XBrush brush, XRect rect);
        void DrawRectangle(XPen pen, XRect rect);
        void DrawRectangle(XPen borderPen, XBrush brush, double p1, double p2, double p3, double p4);
        void DrawEllipse(XPen pen, double d, double d1, int i, int i1);
    }
}