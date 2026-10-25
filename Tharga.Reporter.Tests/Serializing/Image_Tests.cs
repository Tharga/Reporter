using AwesomeAssertions;
using Tharga.Reporter.Entity;
using Tharga.Reporter.Entity.Element;
using Xunit;

namespace Tharga.Reporter.Tests.Serializing;

public class Image_Tests
{
    [Fact]
    public void Default_Image()
    {
        //Arrange
        var image = new Image();
        var xme = image.ToXme();

        //Act
        var other = Image.Load(xme);

        //Assert
        other.Left.Should().Be(image.Left);
        other.Right.Should().Be(image.Right);
        other.Width.Should().Be(image.Width);
        other.Top.Should().Be(image.Top);
        other.Bottom.Should().Be(image.Bottom);
        other.Height.Should().Be(image.Height);
        other.Source.Should().Be(image.Source);
        other.IsBackground.Should().Be(image.IsBackground);
        other.Name.Should().Be(image.Name);
        other.ToXme().OuterXml.Should().Be(xme.OuterXml);
    }

    [Fact]
    public void Image_with_all_propreties_set()
    {
        //Arrange
        var image = new Image
        {
            Right = UnitValue.Parse("1cm"),
            Width = UnitValue.Parse("2cm"),
            Bottom = UnitValue.Parse("2cm"),
            Height = UnitValue.Parse("3cm"),
            IsBackground = true,
            Name = "Blah",
            Source = "Blab_blah"
        };
        var xme = image.ToXme();

        //Act
        var other = Image.Load(xme);

        //Assert
        other.Left.Should().Be(image.Left);
        other.Right.Should().Be(image.Right);
        other.Width.Should().Be(image.Width);
        other.Top.Should().Be(image.Top);
        other.Bottom.Should().Be(image.Bottom);
        other.Height.Should().Be(image.Height);
        other.Source.Should().Be("Blab_blah");
        other.IsBackground.Should().BeTrue();
        other.Name.Should().Be("Blah");
        other.ToXme().OuterXml.Should().Be(xme.OuterXml);
    }
}
