using PdfSharp.Drawing;
using Tharga.Reporter.Interface;

namespace Tharga.Reporter
{
    internal class DebugData : IDebugData
    {
        public DebugData()
        {
            Pen = new XPen(XColor.FromKnownColor(XKnownColor.Blue), 0.1);
            Brush = new XSolidBrush(XColor.FromKnownColor(XKnownColor.Blue));
            Font = new XFont("Verdana", 10);
        }

        public XPen Pen { get; private set; }
        public XBrush Brush { get; private set; }
        public XFont Font { get; private set; }
    }
}