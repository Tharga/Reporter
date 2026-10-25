using AwesomeAssertions;
using Tharga.Reporter.Entity;
using Xunit;

namespace Tharga.Reporter.Tests;

public class Engine
{
    [Fact]
    public void Create_template_with_default_section()
    {
        //Arrange
        var section = new Section();

        //Act
        var template = new Template(section);

        //Assert
        template.SectionList.Count.Should().Be(1);

        var first = template.SectionList.First();
        first.Header.Should().NotBeNull();
        first.Header.ElementList.Count.Should().Be(0);
        first.Header.Height.Should().Be(UnitValue.Parse("0"));

        first.Footer.Should().NotBeNull();
        first.Footer.ElementList.Count.Should().Be(0);
        first.Footer.Height.Should().Be(UnitValue.Parse("0"));

        first.Pane.Should().NotBeNull();
        first.Pane.ElementList.Count.Should().Be(0);

        first.Margin.Should().NotBeNull();
        first.Margin.Left.Should().Be(UnitValue.Parse("0"));
        first.Margin.Right.Should().Be(UnitValue.Parse("0"));
        first.Margin.Top.Should().Be(UnitValue.Parse("0"));
        first.Margin.Bottom.Should().Be(UnitValue.Parse("0"));
        first.Margin.Height.Should().BeNull();
        first.Margin.Width.Should().BeNull();
    }
}