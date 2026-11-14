using AwesomeAssertions;
using PdfSharp;
using Tharga.Reporter.Entity;
using Tharga.Reporter.Entity.Element;
using Tharga.Reporter.Fonts;
using Xunit;

namespace Tharga.Reporter.Tests.Rendering;

public class Rendering_a_reference_point_in_a_vertical_stack
{
    public Rendering_a_reference_point_in_a_vertical_stack()
    {
        ReporterFontResolver.EnsureInstalled();
    }

    [Fact]
    public void A_nested_reference_point_is_drawn_below_the_element_before_it()
    {
        //Arrange
        var inner = new ReferencePoint();
        inner.ElementList.Add(new Text { Value = "Inner" });
        var outer = new ReferencePoint { Stack = ReferencePoint.StackMethod.Vertical };
        outer.ElementList.Add(new Text { Value = "First" });
        outer.ElementList.Add(inner);
        outer.ElementList.Add(new Text { Value = "After" });
        var drawn = Render(outer);

        //Act
        var first = drawn["First"];
        var innerY = drawn["Inner"];
        var after = drawn["After"];

        //Assert
        innerY.Should().BeGreaterThan(first);
        after.Should().BeGreaterThan(innerY);
    }

    [Fact]
    public void A_nested_reference_point_with_a_top_of_its_own_is_not_moved_by_the_stack()
    {
        //Arrange
        var inner = new ReferencePoint { Top = "0" };
        inner.ElementList.Add(new Text { Value = "Inner" });
        var outer = new ReferencePoint { Stack = ReferencePoint.StackMethod.Vertical };
        outer.ElementList.Add(new Text { Value = "First" });
        outer.ElementList.Add(inner);

        //Act
        var drawn = Render(outer);

        //Assert
        drawn["Inner"].Should().Be(drawn["First"]);
    }

    private static Dictionary<string, double> Render(ReferencePoint outer)
    {
        var section = new Section();
        section.Pane.ElementList.Add(outer);
        var graphics = new RecordingGraphicsFactory();
        new Renderer(graphics, new Template(section)).GetPdfBinary(PageSize.A4);
        return graphics.Strings.ToDictionary(x => x.Text, x => x.Location.Y);
    }
}
