using AwesomeAssertions;
using Moq;
using PdfSharp.Drawing;
using Tharga.Reporter.Entity.Element;
using Tharga.Reporter.Entity.Element.Base;
using Tharga.Reporter.Entity.Util;
using Tharga.Reporter.Interface;
using Xunit;

namespace Tharga.Reporter.Tests.Rendering;

public class When_rendering_a_rectangle_with_visibility_first_page_on_the_first_page
{
    private readonly Mock<IGraphics> _graphicsMock;
    private XRect _elementBounds;

    public When_rendering_a_rectangle_with_visibility_first_page_on_the_first_page()
    {
        //Arrange
        var rectangle = new Rectangle { Visibility = PageVisibility.FirstPage };

        _graphicsMock = new Mock<IGraphics>(MockBehavior.Strict);
        _graphicsMock.Setup(x => x.DrawRectangle(It.IsAny<XPen>(), It.IsAny<XRect>()));

        var renderDataMock = new Mock<IRenderData>(MockBehavior.Strict);
        renderDataMock.Setup(x => x.ParentBounds).Returns(new XRect { Width = 20, Height = 20 });
        renderDataMock.SetupSet(x => x.ElementBounds = It.IsAny<XRect>()).Callback<XRect>(x => _elementBounds = x);
        renderDataMock.Setup(x => x.ElementBounds).Returns(new XRect { Width = 10, Height = 10 });
        renderDataMock.Setup(x => x.Graphics).Returns(_graphicsMock.Object);
        renderDataMock.Setup(x => x.StackTop).Returns((double?)null);
        renderDataMock.Setup(x => x.PageNumberInfo).Returns(new PageNumberInfo(1, 2));

        //Act
        rectangle.Render(renderDataMock.Object);
    }

    [Fact]
    public void Then_the_rectangle_is_drawn()
    {
        //Assert
        _graphicsMock.Verify(x => x.DrawRectangle(It.IsAny<XPen>(), It.IsAny<XRect>()), Times.Once);
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
