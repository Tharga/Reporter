using System.Drawing;
using AwesomeAssertions;
using Tharga.Reporter.Entity;
using Tharga.Reporter.Entity.Element;
using Xunit;
using Font = Tharga.Reporter.Entity.Font;
using Image = Tharga.Reporter.Entity.Element.Image;
using Rectangle = Tharga.Reporter.Entity.Element.Rectangle;

namespace Tharga.Reporter.Tests.Serializing;

public class Section_Test
{
    [Fact]
    public void Section_with_a_name()
    {
        //Arrange
        const string name = "ABC";
        var section = new Section
        {
            Name = name
        };
        var template = new Template(section);
        var xml = template.ToXml();

        //Act
        var otherTemplate = Template.Load(xml);

        //Assert
        var other = otherTemplate.SectionList.Single();
        other.Name.Should().Be(name);
        other.DefaultFont.FontName.Should().Be(section.DefaultFont.FontName);
        other.DefaultFont.Size.Should().Be(section.DefaultFont.Size);
        other.DefaultFont.Color.Should().Be(section.DefaultFont.Color);
        otherTemplate.ToXml().OuterXml.Should().Be(xml.OuterXml);
    }

    [Fact]
    public void Section_with_a_defaultFont()
    {
        //Arrange
        var section = new Section
        {
            DefaultFont = new Font
            {
                FontName = "Times",
                Size = 11,
                Color = Color.MediumTurquoise
            }
        };
        var xme = section.ToXme();

        //Act
        var other = Section.Load(xme);

        //Assert
        other.Name.Should().Be(section.Name);
        other.DefaultFont.FontName.Should().Be("Times");
        other.DefaultFont.Size.Should().Be(11);
        other.DefaultFont.Color.ToArgb().Should().Be(Color.MediumTurquoise.ToArgb());
        other.ToXme().OuterXml.Should().Be(xme.OuterXml);
    }

    [Fact]
    public void Section_with_a_margin()
    {
        //Arrange
        var margin = new UnitRectangle { Top = UnitValue.Parse("1cm"), Left = UnitValue.Parse("2px"), Bottom = UnitValue.Parse("30mm"), Right = UnitValue.Parse("4cm") };
        var section = new Section
        {
            Margin = margin
        };
        var template = new Template(section);
        var xml = template.ToXml();

        //Act
        var otherTemplate = Template.Load(xml);

        //Assert
        var otherMargin = otherTemplate.SectionList.Single().Margin;
        otherMargin.Left.Should().Be(margin.Left);
        otherMargin.Top.Should().Be(margin.Top);
        otherMargin.Right.Should().Be(margin.Right);
        otherMargin.Bottom.Should().Be(margin.Bottom);
        otherMargin.Width.Should().BeNull();
        otherMargin.Height.Should().BeNull();
        otherMargin.Should().Be(margin);
        otherTemplate.ToXml().OuterXml.Should().Be(xml.OuterXml);
    }

    [Fact]
    public void Section_with_header_height()
    {
        //Arrange
        var section = new Section();
        section.Header.Height = UnitValue.Parse("4inch");
        var template = new Template(section);
        var xml = template.ToXml();

        //Act
        var otherTemplate = Template.Load(xml);

        //Assert
        otherTemplate.SectionList.Single().Header.Height.Should().Be(section.Header.Height);
        otherTemplate.ToXml().OuterXml.Should().Be(xml.OuterXml);
    }

    [Fact]
    public void Section_with_footer_height()
    {
        //Arrange
        var section = new Section();
        section.Footer.Height = UnitValue.Parse("40");
        var template = new Template(section);
        var xml = template.ToXml();

        //Act
        var otherTemplate = Template.Load(xml);

        //Assert
        otherTemplate.SectionList.Single().Footer.Height.Should().Be(section.Footer.Height);
        otherTemplate.ToXml().OuterXml.Should().Be(xml.OuterXml);
    }

    [Fact]
    public void Section_with_panes_with_element()
    {
        //Arrange
        var section = new Section();
        section.Header.ElementList.Add(new Line { Left = UnitValue.Parse("1cm") });
        section.Pane.ElementList.Add(new Line { Left = UnitValue.Parse("2cm") });
        section.Footer.ElementList.Add(new Line { Left = UnitValue.Parse("3cm") });
        var template = new Template(section);
        var xml = template.ToXml();

        //Act
        var otherTemplate = Template.Load(xml);

        //Assert
        var other = otherTemplate.SectionList.Single();
        other.Header.ElementList.Single().Left.Should().Be(UnitValue.Parse("1cm"));
        other.Pane.ElementList.Single().Left.Should().Be(UnitValue.Parse("2cm"));
        other.Footer.ElementList.Single().Left.Should().Be(UnitValue.Parse("3cm"));
        otherTemplate.ToXml().OuterXml.Should().Be(xml.OuterXml);
    }

    [Fact]
    public void Section_with_all_types_of_elements()
    {
        //Arrange
        var section = new Section();
        section.Pane.ElementList.Add(new Image());
        section.Pane.ElementList.Add(new Line());
        section.Pane.ElementList.Add(new Rectangle());
        section.Pane.ElementList.Add(new Table());
        section.Pane.ElementList.Add(new Text());
        section.Pane.ElementList.Add(new TextBox());
        section.Pane.ElementList.Add(new ReferencePoint());
        section.Pane.ElementList.Add(new BarCode());
        var template = new Template(section);
        var xml = template.ToXml();

        //Act
        var otherTemplate = Template.Load(xml);

        //Assert
        otherTemplate.SectionList.Single().Pane.ElementList.Select(x => x.GetType()).Should().Equal(section.Pane.ElementList.Select(x => x.GetType()));
        otherTemplate.ToXml().OuterXml.Should().Be(xml.OuterXml);
    }
}
