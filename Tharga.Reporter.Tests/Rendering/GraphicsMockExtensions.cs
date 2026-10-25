using Moq;
using PdfSharp.Drawing;
using Tharga.Reporter.Interface;

namespace Tharga.Reporter.Tests.Rendering;

internal static class GraphicsMockExtensions
{
    /// <summary>
    /// Verifies that no drawing call of any kind reached the graphics. Measuring is allowed.
    /// </summary>
    public static void VerifyNothingDrawn(this Mock<IGraphics> graphicsMock)
    {
        graphicsMock.Verify(x => x.DrawLine(It.IsAny<XPen>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
        graphicsMock.Verify(x => x.DrawEllipse(It.IsAny<XPen>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        graphicsMock.Verify(x => x.DrawImage(It.IsAny<XImage>(), It.IsAny<XRect>()), Times.Never);
        graphicsMock.Verify(x => x.DrawRectangle(It.IsAny<XPen>(), It.IsAny<XRect>()), Times.Never);
        graphicsMock.Verify(x => x.DrawRectangle(It.IsAny<XPen>(), It.IsAny<XBrush>(), It.IsAny<XRect>()), Times.Never);
        graphicsMock.Verify(x => x.DrawRectangle(It.IsAny<XBrush>(), It.IsAny<XRect>()), Times.Never);
        graphicsMock.Verify(x => x.DrawString(It.IsAny<string>(), It.IsAny<XFont>(), It.IsAny<XBrush>(), It.IsAny<XPoint>()), Times.Never);
        graphicsMock.Verify(x => x.DrawString(It.IsAny<string>(), It.IsAny<XFont>(), It.IsAny<XBrush>(), It.IsAny<XPoint>(), It.IsAny<XStringFormat>()), Times.Never);
        graphicsMock.Verify(x => x.DrawString(It.IsAny<string>(), It.IsAny<XFont>(), It.IsAny<XBrush>(), It.IsAny<XRect>(), It.IsAny<XStringFormat>()), Times.Never);
    }
}
