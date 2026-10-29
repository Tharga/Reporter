using System.Drawing;
using AwesomeAssertions;
using Tharga.Reporter.Entity;
using Xunit;
using Font = Tharga.Reporter.Entity.Font;

namespace Tharga.Reporter.Tests.Serializing;

public class FontClass_Tests
{
    [Fact]
    public void Font_classes_of_a_section_survive_a_round_trip()
    {
        //Arrange
        var section = new Section();
        section.FontClasses["Heading"] = new Font { FontName = "Times", Size = 20, Bold = true, Color = Color.Red };
        section.FontClasses["Small"] = new Font { Size = 7 };
        var template = new Template(section);
        var xml = template.ToXml();

        //Act
        var other = Template.Load(xml).SectionList.Single();

        //Assert
        other.FontClasses.Keys.Should().BeEquivalentTo("Heading", "Small");
        other.FontClasses["Heading"].FontName.Should().Be("Times");
        other.FontClasses["Heading"].Size.Should().Be(20);
        other.FontClasses["Heading"].Bold.Should().BeTrue();
        other.FontClasses["Heading"].Color.ToArgb().Should().Be(Color.Red.ToArgb());
        other.FontClasses["Small"].Size.Should().Be(7);
        new Template(other).ToXml().OuterXml.Should().Be(xml.OuterXml);
    }

    [Fact]
    public void A_section_without_font_classes_writes_no_font_class_element()
    {
        //Act
        var xme = new Section().ToXme();

        //Assert
        xme.SelectNodes("FontClass")!.Count.Should().Be(0);
    }
}
