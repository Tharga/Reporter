using Moq;
using PdfSharp.Drawing;
using Tharga.Reporter.Entity;
using Tharga.Reporter.Entity.Element;
using Tharga.Reporter.Entity.Util;
using Tharga.Reporter.Fonts;
using Tharga.Reporter.Interface;
using Xunit;

namespace Tharga.Reporter.Tests.Rendering;

public class When_rendering_reference_point
{
    private readonly Mock<IGraphics> _graphicsMock;

    public When_rendering_reference_point()
    {
        //Arrange
        ReporterFontResolver.EnsureInstalled();

        var documentData = new DocumentData();

        _graphicsMock = new Mock<IGraphics>(MockBehavior.Strict);
        _graphicsMock.Setup(x => x.MeasureString(It.IsAny<string>(), It.IsAny<XFont>(), It.IsAny<XStringFormat>())).Returns(new XSize { Height = 1000, Width = 1000 });
        _graphicsMock.Setup(x => x.DrawLine(It.IsAny<XPen>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()));
        _graphicsMock.Setup(x => x.DrawString(It.IsAny<string>(), It.IsAny<XFont>(), It.IsAny<XBrush>(), It.IsAny<XPoint>(), It.IsAny<XStringFormat>()));

        var referencePoint = new ReferencePoint();
        referencePoint.ElementList.Add(new TextBox { Value = "Some text!", Width = "10cm", Height = "10cm" });
        referencePoint.ElementList.Add(new Line());

        var renderDataMock = new Mock<IRenderData>(MockBehavior.Strict);
        renderDataMock.Setup(x => x.ParentBounds).Returns(new XRect { Width = 200, Height = 200 });
        renderDataMock.Setup(x => x.Graphics).Returns(_graphicsMock.Object);
        renderDataMock.Setup(x => x.Context).Returns(new RenderContext());
        renderDataMock.Setup(x => x.Section).Returns(new Section());
        renderDataMock.Setup(x => x.DocumentData).Returns(documentData);
        renderDataMock.Setup(x => x.PageNumberInfo).Returns(new PageNumberInfo(1, 1));
        renderDataMock.Setup(x => x.DebugData).Returns((IDebugData)null);
        renderDataMock.Setup(x => x.IncludeBackground).Returns(false);
        renderDataMock.Setup(x => x.ElementBounds).Returns(new XRect { Width = 200, Height = 200 });
        renderDataMock.SetupSet(x => x.ElementBounds = It.IsAny<XRect>());

        referencePoint.PreRender(renderDataMock.Object);

        //Act
        // The page argument is zero-based, while PageNumberInfo numbers pages from one: this renders the first page.
        referencePoint.Render(renderDataMock.Object, 0);
    }

    [Fact]
    public void Then_line_is_drawn()
    {
        //Assert
        _graphicsMock.Verify(x => x.DrawLine(It.IsAny<XPen>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()), Times.Once);
    }

    [Fact]
    public void Then_string_is_drawn()
    {
        //Assert
        _graphicsMock.Verify(x => x.DrawString(It.IsAny<string>(), It.IsAny<XFont>(), It.IsAny<XBrush>(), It.IsAny<XPoint>(), It.IsAny<XStringFormat>()), Times.Once);
    }

    [Fact]
    public void Then_nothing_else_is_drawn()
    {
        //Assert
        _graphicsMock.Verify(x => x.DrawEllipse(It.IsAny<XPen>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        _graphicsMock.Verify(x => x.DrawImage(It.IsAny<XImage>(), It.IsAny<XRect>()), Times.Never);
        _graphicsMock.Verify(x => x.DrawRectangle(It.IsAny<XPen>(), It.IsAny<XRect>()), Times.Never);
        _graphicsMock.Verify(x => x.DrawRectangle(It.IsAny<XPen>(), It.IsAny<XBrush>(), It.IsAny<XRect>()), Times.Never);
        _graphicsMock.Verify(x => x.DrawRectangle(It.IsAny<XBrush>(), It.IsAny<XRect>()), Times.Never);
        _graphicsMock.Verify(x => x.DrawString(It.IsAny<string>(), It.IsAny<XFont>(), It.IsAny<XBrush>(), It.IsAny<XPoint>()), Times.Never);
        _graphicsMock.Verify(x => x.DrawString(It.IsAny<string>(), It.IsAny<XFont>(), It.IsAny<XBrush>(), It.IsAny<XRect>(), It.IsAny<XStringFormat>()), Times.Never);
    }
}
