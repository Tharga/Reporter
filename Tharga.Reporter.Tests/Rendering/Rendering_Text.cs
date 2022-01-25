using Moq;
using PdfSharp.Drawing;
using Tharga.Reporter.Entity;
using Tharga.Reporter.Entity.Element;
using Tharga.Reporter.Entity.Util;
using Tharga.Reporter.Interface;
using Xunit;

namespace Tharga.Reporter.Tests.Rendering;

public class Rendering_Text
{
    [Fact(Skip = "Can't gain access to internal stuff.")]
    public void When_rendering_text()
    {
        //Arrange
        var text = new Text();

        var graphicsMock = new Mock<IGraphics>(MockBehavior.Strict);
        graphicsMock.Setup(x => x.MeasureString(It.IsAny<string>(), It.IsAny<XFont>(), It.IsAny<XStringFormat>())).Returns(new XSize { Height = 1000, Width = 1000 });
        graphicsMock.Setup(x => x.MeasureString(It.IsAny<string>(), It.IsAny<XFont>())).Returns(new XSize { Height = 1000, Width = 1000 });
        graphicsMock.Setup(x => x.DrawString(It.IsAny<string>(), It.IsAny<XFont>(), It.IsAny<XBrush>(), It.IsAny<XRect>(), It.IsAny<XStringFormat>()));

        var renderDataMock = new Mock<IRenderData>(MockBehavior.Strict);
        renderDataMock.Setup(x => x.ParentBounds).Returns(new XRect());
        renderDataMock.Setup(x => x.Section).Returns(new Section());
        renderDataMock.Setup(x => x.DocumentData).Returns((DocumentData)null);
        renderDataMock.Setup(x => x.PageNumberInfo).Returns(new PageNumberInfo(1, 2));
        renderDataMock.Setup(x => x.Graphics).Returns(graphicsMock.Object);
        renderDataMock.SetupSet(x => x.ElementBounds = It.IsAny<XRect>());
        renderDataMock.Setup(x => x.IncludeBackground).Returns(false);
        renderDataMock.Setup(x => x.ElementBounds).Returns(new XRect());
        renderDataMock.Setup(x => x.DebugData).Returns((IDebugData)null);

        //Act
        text.Render(renderDataMock.Object);

        //Assert
    }
}