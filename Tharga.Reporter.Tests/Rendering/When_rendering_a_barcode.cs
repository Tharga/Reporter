using AwesomeAssertions;
using Moq;
using PdfSharp.Drawing;
using Tharga.Reporter.Entity.Element;
using Tharga.Reporter.Entity.Util;
using Tharga.Reporter.Interface;
using Xunit;

namespace Tharga.Reporter.Tests.Rendering;

public class When_rendering_a_barcode
{
    private XRect _elementBounds;

    public When_rendering_a_barcode()
    {
        //Arrange
        var barCode = new BarCode { Code = "ABC123" };

        var documentData = new Mock<IDocumentData>(MockBehavior.Strict);

        var graphicsMock = new Mock<IGraphics>(MockBehavior.Strict);
        graphicsMock.Setup(x => x.DrawRectangle(It.IsAny<XBrush>(), It.IsAny<XRect>()));

        var renderDataMock = new Mock<IRenderData>(MockBehavior.Strict);
        renderDataMock.Setup(x => x.ParentBounds).Returns(new XRect { Width = 20, Height = 20 });
        renderDataMock.SetupSet(x => x.ElementBounds = It.IsAny<XRect>()).Callback<XRect>(x => _elementBounds = x);
        renderDataMock.Setup(x => x.DocumentData).Returns(documentData.Object);
        renderDataMock.Setup(x => x.ElementBounds).Returns(new XRect { Width = 10, Height = 10 });
        renderDataMock.Setup(x => x.Graphics).Returns(graphicsMock.Object);
        renderDataMock.Setup(x => x.StackTop).Returns((double?)null);
        renderDataMock.Setup(x => x.PageNumberInfo).Returns(new PageNumberInfo(1, 2));

        //Act
        barCode.Render(renderDataMock.Object);
    }

    [Fact]
    public void Then_the_element_bounds_is_set_to_some_width()
    {
        //Assert
        _elementBounds.Width.Should().NotBe(0);
    }

    [Fact]
    public void Then_the_element_bounds_is_set_to_some_height()
    {
        //Assert
        _elementBounds.Height.Should().NotBe(0);
    }
}
