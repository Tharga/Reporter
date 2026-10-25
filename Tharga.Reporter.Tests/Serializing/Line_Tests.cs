using System.Drawing;
using AwesomeAssertions;
using Tharga.Reporter.Entity;
using Tharga.Reporter.Entity.Element;
using Xunit;

namespace Tharga.Reporter.Tests.Serializing;

public class Line_Tests
{
    [Fact]
    public void Default_line()
    {
        //Arrange
        var line = new Line();
        var xme = line.ToXme();

        //Act
        var other = Line.Load(xme);

        //Assert
        other.Left.Should().Be(line.Left);
        other.Right.Should().Be(line.Right);
        other.Width.Should().Be(line.Width);
        other.Top.Should().Be(line.Top);
        other.Bottom.Should().Be(line.Bottom);
        other.Height.Should().Be(line.Height);
        other.Color.ToArgb().Should().Be(line.Color.ToArgb());
        other.IsBackground.Should().Be(line.IsBackground);
        other.Name.Should().Be(line.Name);
        other.Thickness.Should().Be(line.Thickness);
        other.ToXme().OuterXml.Should().Be(xme.OuterXml);
    }

    [Fact]
    public void Line_with_all_propreties_set()
    {
        //Arrange
        var line = new Line
        {
            Left = UnitValue.Parse("1mm"),
            Right = UnitValue.Parse("2mm"),
            Top = UnitValue.Parse("3mm"),
            Bottom = UnitValue.Parse("4mm"),
            IsBackground = true,
            Color = Color.Aquamarine,
            Name = "Bob Loblaw",
            Thickness = "1cm"
        };
        var xme = line.ToXme();

        //Act
        var other = Line.Load(xme);

        //Assert
        other.Left.Should().Be(line.Left);
        other.Right.Should().Be(line.Right);
        other.Width.Should().Be(line.Width);
        other.Top.Should().Be(line.Top);
        other.Bottom.Should().Be(line.Bottom);
        other.Height.Should().Be(line.Height);
        other.Color.ToArgb().Should().Be(Color.Aquamarine.ToArgb());
        other.IsBackground.Should().BeTrue();
        other.Name.Should().Be("Bob Loblaw");
        other.Thickness.Should().Be(UnitValue.Parse("1cm"));
        other.ToXme().OuterXml.Should().Be(xme.OuterXml);
    }
}
