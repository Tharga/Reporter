using Moq;
using PdfSharp.Drawing;
using Tharga.Reporter.Entity;
using Tharga.Reporter.Entity.Element;
using Tharga.Reporter.Entity.Util;
using Tharga.Reporter.Fonts;
using Tharga.Reporter.Interface;
using Xunit;

namespace Tharga.Reporter.Tests.Rendering;

public class When_pre_rendering_reference_point
{
    private readonly Mock<IGraphics> _graphicsMock;

    public When_pre_rendering_reference_point()
    {
        //Arrange
        ReporterFontResolver.EnsureInstalled();

        var documentData = new DocumentData();

        _graphicsMock = new Mock<IGraphics>(MockBehavior.Strict);
        _graphicsMock.Setup(x => x.MeasureString(It.IsAny<string>(), It.IsAny<XFont>(), It.IsAny<XStringFormat>())).Returns(new XSize { Height = 1000, Width = 1000 });

        var referencePoint = new ReferencePoint();
        referencePoint.ElementList.Add(new TextBox { Value = "Some text!" });

        var renderDataMock = new Mock<IRenderData>(MockBehavior.Strict);
        renderDataMock.Setup(x => x.ParentBounds).Returns(new XRect());
        renderDataMock.Setup(x => x.Graphics).Returns(_graphicsMock.Object);
        renderDataMock.Setup(x => x.Context).Returns(new RenderContext());
        renderDataMock.Setup(x => x.Section).Returns(new Section());
        renderDataMock.Setup(x => x.DocumentData).Returns(documentData);
        renderDataMock.Setup(x => x.PageNumberInfo).Returns(new PageNumberInfo(1, 2));
        renderDataMock.Setup(x => x.DebugData).Returns((IDebugData)null);
        renderDataMock.Setup(x => x.IncludeBackground).Returns(false);
        renderDataMock.Setup(x => x.ElementBounds).Returns(new XRect());
        renderDataMock.SetupSet(x => x.ElementBounds = It.IsAny<XRect>());

        //Act
        referencePoint.PreRender(renderDataMock.Object);
    }

    [Fact]
    public void Then_nothing_is_drawn()
    {
        //Assert
        _graphicsMock.VerifyNothingDrawn();
    }
}
