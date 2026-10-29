using Moq;
using PdfSharp.Drawing;
using Tharga.Reporter.Entity;
using Tharga.Reporter.Entity.Util;
using Tharga.Reporter.Interface;
using Xunit;

namespace Tharga.Reporter.Tests.Rendering;

public class When_rendering_reference_point_with_no_elements
{
    private readonly Mock<IGraphics> _graphicsMock;

    public When_rendering_reference_point_with_no_elements()
    {
        //Arrange
        var documentData = new DocumentData();

        _graphicsMock = new Mock<IGraphics>(MockBehavior.Strict);

        var referencePoint = new ReferencePoint();

        var renderDataMock = new Mock<IRenderData>(MockBehavior.Strict);
        renderDataMock.Setup(x => x.ParentBounds).Returns(new XRect());
        renderDataMock.Setup(x => x.Graphics).Returns(_graphicsMock.Object);
        renderDataMock.Setup(x => x.Context).Returns(new RenderContext());
        renderDataMock.Setup(x => x.Section).Returns(new Section());
        renderDataMock.Setup(x => x.DocumentData).Returns(documentData);
        renderDataMock.Setup(x => x.PageNumberInfo).Returns(new PageNumberInfo(1, 1));
        renderDataMock.Setup(x => x.DebugData).Returns((IDebugData)null);
        renderDataMock.Setup(x => x.IncludeBackground).Returns(false);
        renderDataMock.Setup(x => x.ElementBounds).Returns(new XRect());
        renderDataMock.SetupSet(x => x.ElementBounds = It.IsAny<XRect>());

        referencePoint.PreRender(renderDataMock.Object);

        //Act
        // The page argument is zero-based, while PageNumberInfo numbers pages from one: this renders the first page.
        referencePoint.Render(renderDataMock.Object, 0);
    }

    [Fact]
    public void Then_nothing_is_drawn()
    {
        //Assert
        _graphicsMock.VerifyNothingDrawn();
    }
}
