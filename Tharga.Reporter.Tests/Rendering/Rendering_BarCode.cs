using Aspose.BarCode.BarCodeRecognition;
using FluentAssertions;
using Moq;
using PdfSharp.Drawing;
using Tharga.Reporter.Entity;
using Tharga.Reporter.Entity.Element;
using Tharga.Reporter.Entity.Util;
using Tharga.Reporter.Interface;
using Xunit;

namespace Tharga.Reporter.Tests.Rendering;

public class Rendering_BarCode
{
    private const string Code = "ABC123";
    private const int Code39BarsPerCharacter = 5;
    private const int Code39StartAndStopCharacters = 2;

    [Fact]
    public void Bar_count_matches_the_Code39_symbology()
    {
        //Arrange
        var bounds = new XRect(0, 0, 600, 100);

        //Act
        var bars = BarCode.GetBars(Code, bounds);

        //Assert
        var expected = (Code.Length + Code39StartAndStopCharacters) * Code39BarsPerCharacter;
        bars.Count.Should().Be(expected);
    }

    [Fact]
    public void Bars_stay_within_the_element_bounds()
    {
        //Arrange
        var bounds = new XRect(10, 20, 600, 100);

        //Act
        var bars = BarCode.GetBars(Code, bounds);

        //Assert
        bars.Should().OnlyContain(x => x.Left >= bounds.Left && x.Right <= bounds.Right);
        bars.Should().OnlyContain(x => x.Top == bounds.Top && x.Height == bounds.Height);
        bars.Should().OnlyContain(x => x.Width > 0);
    }

    [Fact]
    public void Bars_are_ordered_and_never_overlap()
    {
        //Arrange
        var bounds = new XRect(0, 0, 600, 100);

        //Act
        var bars = BarCode.GetBars(Code, bounds);

        //Assert
        bars.Zip(bars.Skip(1), (left, right) => right.Left >= left.Right).Should().OnlyContain(x => x);
    }

    [Theory]
    [InlineData("ABC123")]
    [InlineData("S1309799801")]
    [InlineData("1")]
    public void Rendered_bars_decode_back_to_the_original_code(string code)
    {
        //Arrange
        var png = BarCodeSampleWriter.Rasterize(code, BarCodeSampleWriter.ScannableWidth(code), 300);
        using var stream = new MemoryStream(png);
        using var bitmap = new Aspose.Drawing.Bitmap(stream);

        //Act
        using var reader = new BarCodeReader(bitmap, DecodeType.Code39);
        var results = reader.ReadBarCodes();

        //Assert
        results.Should().ContainSingle();
        results[0].CodeText.Should().Be(code);
    }

    [Fact]
    public void Render_draws_one_rectangle_for_every_bar()
    {
        //Arrange
        var barCode = new BarCode { Code = Code, Width = "600", Height = "100" };
        var drawn = new List<XRect>();

        var graphicsMock = new Mock<IGraphics>(MockBehavior.Strict);
        graphicsMock
            .Setup(x => x.DrawRectangle(It.IsAny<XBrush>(), It.IsAny<XRect>()))
            .Callback<XBrush, XRect>((_, rect) => drawn.Add(rect));

        var renderDataMock = new Mock<IRenderData>(MockBehavior.Strict);
        renderDataMock.Setup(x => x.ParentBounds).Returns(new XRect(0, 0, 600, 100));
        renderDataMock.Setup(x => x.DocumentData).Returns((DocumentData)null);
        renderDataMock.Setup(x => x.PageNumberInfo).Returns(new PageNumberInfo(1, 1));
        renderDataMock.Setup(x => x.Graphics).Returns(graphicsMock.Object);
        renderDataMock.Setup(x => x.IncludeBackground).Returns(true);
        renderDataMock.SetupSet(x => x.ElementBounds = It.IsAny<XRect>());

        //Act
        barCode.Render(renderDataMock.Object);

        //Assert
        var expected = (Code.Length + Code39StartAndStopCharacters) * Code39BarsPerCharacter;
        drawn.Count.Should().Be(expected);
    }

    [Fact]
    public void Render_never_touches_the_image_path()
    {
        //Arrange
        var barCode = new BarCode { Code = Code, Width = "600", Height = "100" };

        var graphicsMock = new Mock<IGraphics>(MockBehavior.Strict);
        graphicsMock.Setup(x => x.DrawRectangle(It.IsAny<XBrush>(), It.IsAny<XRect>()));

        var renderDataMock = new Mock<IRenderData>(MockBehavior.Strict);
        renderDataMock.Setup(x => x.ParentBounds).Returns(new XRect(0, 0, 600, 100));
        renderDataMock.Setup(x => x.DocumentData).Returns((DocumentData)null);
        renderDataMock.Setup(x => x.PageNumberInfo).Returns(new PageNumberInfo(1, 1));
        renderDataMock.Setup(x => x.Graphics).Returns(graphicsMock.Object);
        renderDataMock.Setup(x => x.IncludeBackground).Returns(true);
        renderDataMock.SetupSet(x => x.ElementBounds = It.IsAny<XRect>());

        //Act
        barCode.Render(renderDataMock.Object);

        //Assert
        graphicsMock.Verify(x => x.DrawImage(It.IsAny<XImage>(), It.IsAny<XRect>()), Times.Never);
    }

    [Fact]
    public void A_template_containing_a_barcode_renders_to_a_pdf()
    {
        //Arrange
        var section = new Section();
        section.Pane.ElementList.Add(new BarCode { Code = Code, Top = "10mm", Left = "8mm", Bottom = "20mm", Right = "8mm" });
        var renderer = new Renderer(new Template(section));

        //Act
        var pdf = renderer.GetPdfBinary(PageFormat.PlasticCard);

        //Assert
        pdf.Should().NotBeEmpty();
        System.Text.Encoding.ASCII.GetString(pdf, 0, 4).Should().Be("%PDF");
    }
}
