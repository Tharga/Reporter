using AwesomeAssertions;
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
    [Fact]
    public void When_rendering_text()
    {
        //Arrange
        var text = new Text { Value = "Hello" };
        var graphicsMock = new Mock<IGraphics>(MockBehavior.Strict);
        graphicsMock.Setup(x => x.MeasureString(It.IsAny<string>(), It.IsAny<XFont>(), It.IsAny<XStringFormat>())).Returns(new XSize { Height = 1000, Width = 1000 });
        graphicsMock.Setup(x => x.MeasureString(It.IsAny<string>(), It.IsAny<XFont>())).Returns(new XSize { Height = 1000, Width = 1000 });
        graphicsMock.Setup(x => x.DrawString(It.IsAny<string>(), It.IsAny<XFont>(), It.IsAny<XBrush>(), It.IsAny<XRect>(), It.IsAny<XStringFormat>()));
        var renderData = new RenderData(graphicsMock.Object, new XRect(), new Section(), null, new PageNumberInfo(1, 2), null, false);

        //Act
        text.Render(renderData);

        //Assert
        graphicsMock.Verify(x => x.DrawString("Hello", It.IsAny<XFont>(), It.IsAny<XBrush>(), It.IsAny<XRect>(), It.IsAny<XStringFormat>()), Times.Once);
        renderData.ElementBounds.Size.Should().Be(new XSize(1000, 1000));
    }
}
