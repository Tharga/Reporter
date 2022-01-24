using System.Drawing;
using PdfSharp.Drawing;
using Tharga.Reporter.Engine.Interface;

namespace Tharga.Reporter.Engine
{
    internal class DebugData : IDebugData
    {
        public DebugData()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            Pen = new XPen(XColor.FromKnownColor(XKnownColor.Blue), 0.1);
            Brush = new XSolidBrush(XColor.FromKnownColor(XKnownColor.Blue));
            Font = new XFont("Verdana", 10);
            //Font = new XFont(Font.GdiFontFamily, 10, XFontStyle.Regular);
        }

        public XPen Pen { get; private set; }
        public XBrush Brush { get; private set; }
        public XFont Font { get; private set; }
    }
}