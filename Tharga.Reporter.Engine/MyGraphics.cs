using PdfSharp.Drawing;
using Tharga.Reporter.Engine.Interface;

namespace Tharga.Reporter.Engine
{
    internal class MyGraphics : IGraphics
    {
        private readonly XGraphics _gfx;

        public MyGraphics(XGraphics gfx)
        {
            _gfx = gfx;
        }

        public XSize MeasureString(string code, XFont legendFont)
        {
            return _gfx.MeasureString(code, legendFont);
        }

        public XSize MeasureString(string code, XFont legendFont, XStringFormat topLeft)
        {
            return _gfx.MeasureString(code, legendFont, topLeft);
        }

        public void DrawImage(XImage image, XRect xRect)
        {
            _gfx.DrawImage(image, xRect);
        }

        public void DrawString(string code, XFont legendFont, XSolidBrush legendBrush, XPoint xPoint, XStringFormat topLeft)
        {
            _gfx.DrawString(code, legendFont, legendBrush, xPoint, topLeft);
        }

        public void DrawString(string code, XFont legendFont, XSolidBrush legendBrush, XRect rect)
        {
            _gfx.DrawString(code, legendFont, legendBrush, rect);
        }

        public void DrawString(string code, XFont legendFont, XSolidBrush legendBrush, double x, double y)
        {
            _gfx.DrawString(code, legendFont, legendBrush, x, y);
        }

        public void DrawString(string text, XFont font, XSolidBrush brush, XRect elementBounds, XStringFormat formats)
        {
            _gfx.DrawString(text, font, brush, elementBounds, formats);
        }

        public void DrawString(string line, XFont font, XBrush brush, double left, double top, XStringFormat formats)
        {
            _gfx.DrawString(line, font, brush, left, top, formats);
        }

        public void DrawString(string text, XFont font, XBrush brush, XPoint point)
        {
            _gfx.DrawString(text, font, brush, point);
        }

        public void DrawLine(XPen debugPen, double left, double p2, double p3, double height)
        {
            _gfx.DrawLine(debugPen, left, p2, p3, height);
        }

        public void DrawRectangle(XPen debugPen, double left, double top, double width, double height)
        {
            _gfx.DrawRectangle(debugPen, left, top, width, height);
        }

        public void DrawRectangle(XPen pen, XBrush brush, XRect rect)
        {
            _gfx.DrawRectangle(pen, brush, rect);
        }

        public void DrawRectangle(XPen pen, XRect rect)
        {
            _gfx.DrawRectangle(pen, rect);
        }

        public void DrawRectangle(XPen borderPen, XBrush brush, double p1, double p2, double p3, double p4)
        {
            _gfx.DrawRectangle(borderPen, brush, p1, p2, p3, p4);
        }

        public void DrawEllipse(XPen pen, double d, double d1, int i, int i1)
        {
            _gfx.DrawEllipse(pen, d, d1, i, i1);
        }
    }
}