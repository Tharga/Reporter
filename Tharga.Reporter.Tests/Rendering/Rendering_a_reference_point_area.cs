using AwesomeAssertions;
using Moq;
using PdfSharp;
using PdfSharp.Drawing;
using Tharga.Reporter.Entity;
using Tharga.Reporter.Entity.Element;
using Tharga.Reporter.Entity.Util;
using Tharga.Reporter.Fonts;
using Tharga.Reporter.Interface;
using Xunit;

namespace Tharga.Reporter.Tests.Rendering;

public class Rendering_a_reference_point_area
{
    public Rendering_a_reference_point_area()
    {
        ReporterFontResolver.EnsureInstalled();
    }

    [Fact]
    public void Render_reports_the_area_used_by_the_children_from_the_reference_point_position()
    {
        //Arrange
        var referencePoint = new ReferencePoint { Left = "10", Top = "20" };
        referencePoint.ElementList.Add(new Rectangle { Left = "5", Top = "5", Width = "100", Height = "50" });
        referencePoint.ElementList.Add(new Rectangle { Left = "0", Top = "40", Width = "30", Height = "30" });
        var renderData = CreateRenderData();

        //Act
        referencePoint.Render(renderData, 0);

        //Assert
        renderData.ElementBounds.Should().Be(new XRect(10, 20, 105, 70));
    }

    [Fact]
    public void Pre_render_reports_the_area_used_by_the_children_from_the_reference_point_position()
    {
        //Arrange
        var referencePoint = new ReferencePoint { Left = "10", Top = "20" };
        referencePoint.ElementList.Add(new TextBox { Value = "Text", Left = "5", Top = "5", Width = "100", Height = "50" });
        var renderData = CreateRenderData();

        //Act
        referencePoint.PreRender(renderData);

        //Assert
        renderData.ElementBounds.Should().Be(new XRect(10, 20, 105, 55));
    }

    [Fact]
    public void A_reference_point_with_no_children_reports_an_empty_area_at_its_position()
    {
        //Arrange
        var referencePoint = new ReferencePoint { Left = "10", Top = "20" };
        var renderData = CreateRenderData();

        //Act
        referencePoint.Render(renderData, 0);

        //Assert
        renderData.ElementBounds.Should().Be(new XRect(10, 20, 0, 0));
    }

    [Fact]
    public void An_element_after_a_nested_reference_point_in_a_vertical_stack_is_drawn_below_its_content()
    {
        //Arrange
        var inner = new ReferencePoint();
        inner.ElementList.Add(new Text { Value = "Inner first" });
        inner.ElementList.Add(new Text { Value = "Inner last", Top = "1cm" });
        var outer = new ReferencePoint { Stack = ReferencePoint.StackMethod.Vertical };
        outer.ElementList.Add(inner);
        outer.ElementList.Add(new Text { Value = "After" });
        var section = new Section();
        section.Pane.ElementList.Add(outer);
        var graphics = new RecordingGraphicsFactory();

        //Act
        new Renderer(graphics, new Template(section)).GetPdfBinary(PageSize.A4);

        //Assert
        var drawn = graphics.Strings.ToDictionary(x => x.Text, x => x.Location.Y);
        drawn["After"].Should().BeGreaterThan(drawn["Inner last"]);
    }

    private static RenderData CreateRenderData()
    {
        var graphics = new Mock<IGraphics>();
        graphics.Setup(x => x.MeasureString(It.IsAny<string>(), It.IsAny<XFont>(), It.IsAny<XStringFormat>())).Returns(new XSize(10, 10));
        return new RenderData(graphics.Object, new XRect(0, 0, 600, 800), new Section(), new DocumentData(), new PageNumberInfo(1, 1), null, true, new RenderContext());
    }
}
