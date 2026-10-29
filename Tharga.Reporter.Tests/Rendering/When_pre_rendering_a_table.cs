using Moq;
using PdfSharp.Drawing;
using Tharga.Reporter.Entity;
using Tharga.Reporter.Entity.Element;
using Tharga.Reporter.Entity.Util;
using Tharga.Reporter.Fonts;
using Tharga.Reporter.Interface;
using Xunit;

namespace Tharga.Reporter.Tests.Rendering;

public class When_pre_rendering_a_table
{
    private readonly Mock<IGraphics> _graphicsMock;

    public When_pre_rendering_a_table()
    {
        //Arrange
        ReporterFontResolver.EnsureInstalled();

        var table = new Table();
        table.AddColumn("A1", "A2");
        table.AddColumn("B1", "B2");

        var documentData = new DocumentData();
        documentData.Add("A1", "Data_A1");
        documentData.Add("A2", "Data_A2");
        documentData.Add("B1", "Data_B1");
        documentData.Add("B2", "Data_B2");

        _graphicsMock = new Mock<IGraphics>(MockBehavior.Strict);
        _graphicsMock.Setup(x => x.MeasureString(It.IsAny<string>(), It.IsAny<XFont>(), It.IsAny<XStringFormat>())).Returns(new XSize());
        _graphicsMock.Setup(x => x.DrawLine(It.IsAny<XPen>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()));

        var renderDataMock = new Mock<IRenderData>(MockBehavior.Strict);
        renderDataMock.Setup(x => x.ParentBounds).Returns(new XRect());
        renderDataMock.SetupSet(x => x.ElementBounds = It.IsAny<XRect>());
        renderDataMock.Setup(x => x.ElementBounds).Returns(new XRect());
        renderDataMock.Setup(x => x.Section).Returns(new Section());
        renderDataMock.Setup(x => x.Graphics).Returns(_graphicsMock.Object);
        renderDataMock.Setup(x => x.Context).Returns(new RenderContext());
        renderDataMock.Setup(x => x.StackTop).Returns((double?)null);
        renderDataMock.Setup(x => x.DocumentData).Returns(documentData);

        //Act
        table.PreRender(renderDataMock.Object);
    }

    [Fact]
    public void Nothing_is_drawn()
    {
        //Assert
        _graphicsMock.VerifyNothingDrawn();
    }
}
