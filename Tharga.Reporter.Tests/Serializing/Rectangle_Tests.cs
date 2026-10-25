using System.Drawing;
using AwesomeAssertions;
using Tharga.Reporter.Entity;
using Xunit;
using Rectangle = Tharga.Reporter.Entity.Element.Rectangle;

namespace Tharga.Reporter.Tests.Serializing;

public class Rectangle_Tests
{
    [Fact]
    public void Default_Rectangle()
    {
        //Arrange
        var rectangle = new Rectangle();
        var xme = rectangle.ToXme();

        //Act
        var other = Rectangle.Load(xme);

        //Assert
        other.Left.Should().Be(rectangle.Left);
        other.Top.Should().Be(rectangle.Top);
        other.Right.Should().Be(rectangle.Right);
        other.Bottom.Should().Be(rectangle.Bottom);
        other.Width.Should().Be(rectangle.Width);
        other.Height.Should().Be(rectangle.Height);
        other.BackgroundColor.Should().Be(rectangle.BackgroundColor);
        other.BorderColor.ToArgb().Should().Be(rectangle.BorderColor.ToArgb());
        other.BorderWidth.Should().Be(rectangle.BorderWidth);
        other.IsBackground.Should().Be(rectangle.IsBackground);
        other.Name.Should().Be(rectangle.Name);
        other.ToXme().OuterXml.Should().Be(xme.OuterXml);
    }

    [Fact]
    public void Rectangle_with_all_propreties_set()
    {
        //Arrange
        var rectangle = new Rectangle
        {
            Top = UnitValue.Parse("1cm"),
            Left = UnitValue.Parse("2cm"),
            Width = UnitValue.Parse("40%"),
            Height = UnitValue.Parse("50%"),
            BackgroundColor = Color.Fuchsia,
            BorderColor = Color.ForestGreen,
            BorderWidth = UnitValue.Parse("0.1cm"),
            IsBackground = true,
            Name = "Rea Padda"
        };
        var xme = rectangle.ToXme();

        //Act
        var other = Rectangle.Load(xme);

        //Assert
        other.Left.Should().Be(rectangle.Left);
        other.Top.Should().Be(rectangle.Top);
        other.Right.Should().Be(rectangle.Right);
        other.Bottom.Should().Be(rectangle.Bottom);
        other.Width.Should().Be(rectangle.Width);
        other.Height.Should().Be(rectangle.Height);
        other.BackgroundColor.Should().NotBeNull();
        other.BackgroundColor.GetValueOrDefault().ToArgb().Should().Be(Color.Fuchsia.ToArgb());
        other.BorderColor.ToArgb().Should().Be(Color.ForestGreen.ToArgb());
        other.BorderWidth.Should().Be(UnitValue.Parse("0.1cm"));
        other.IsBackground.Should().BeTrue();
        other.Name.Should().Be("Rea Padda");
        other.ToXme().OuterXml.Should().Be(xme.OuterXml);
    }
}
