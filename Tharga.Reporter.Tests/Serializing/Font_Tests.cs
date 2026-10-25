using System.Drawing;
using AwesomeAssertions;
using Xunit;
using Font = Tharga.Reporter.Entity.Font;

namespace Tharga.Reporter.Tests.Serializing;

public class Font_Tests
{
    [Fact]
    public void Default_Font()
    {
        //Arrange
        var font = new Font();
        var xme = font.ToXme();

        //Act
        var other = Font.Load(xme);

        //Assert
        other.Color.Should().Be(font.Color);
        other.FontName.Should().Be(font.FontName);
        other.Size.Should().Be(font.Size);
        other.Bold.Should().BeFalse();
        other.Italic.Should().BeFalse();
        other.Strikeout.Should().BeFalse();
        other.Underline.Should().BeFalse();
        other.ToXme().OuterXml.Should().Be(xme.OuterXml);
    }

    [Fact]
    public void Font_with_all_properties_set()
    {
        //Arrange
        var font = new Font
        {
            Size = 12,
            Color = Color.Orange,
            FontName = "Some Name",
            Bold = true,
            Italic = true
        };
        var xme = font.ToXme();

        //Act
        var other = Font.Load(xme);

        //Assert
        other.Color.ToArgb().Should().Be(Color.Orange.ToArgb());
        other.FontName.Should().Be("Some Name");
        other.Size.Should().Be(12);
        other.Bold.Should().BeTrue();
        other.Italic.Should().BeTrue();
        other.Strikeout.Should().BeFalse();
        other.Underline.Should().BeFalse();
        other.ToXme().OuterXml.Should().Be(xme.OuterXml);
    }

    [Fact]
    public void Font_with_Strikeout_set()
    {
        //Arrange
        var font = new Font
        {
            Strikeout = true
        };
        var xme = font.ToXme();

        //Act
        var other = Font.Load(xme);

        //Assert
        other.Strikeout.Should().BeTrue();
        other.Underline.Should().BeFalse();
        other.ToXme().OuterXml.Should().Be(xme.OuterXml);
    }

    [Fact]
    public void Font_with_Underline_set()
    {
        //Arrange
        var font = new Font
        {
            Underline = true
        };
        var xme = font.ToXme();

        //Act
        var other = Font.Load(xme);

        //Assert
        other.Underline.Should().BeTrue();
        other.Strikeout.Should().BeFalse();
        other.ToXme().OuterXml.Should().Be(xme.OuterXml);
    }

    [Fact]
    public void Font_cannot_use_bold_with_underline()
    {
        //Act
        var act = () => new Font { Bold = true, Underline = true };

        //Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Font_cannot_use_italic_with_underline()
    {
        //Act
        var act = () => new Font { Italic = true, Underline = true };

        //Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Font_cannot_use_strikeout_with_underline()
    {
        //Act
        var act = () => new Font { Strikeout = true, Underline = true };

        //Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Font_cannot_use_bold_with_strikeout()
    {
        //Act
        var act = () => new Font { Bold = true, Strikeout = true };

        //Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Font_cannot_use_italic_with_strikeout()
    {
        //Act
        var act = () => new Font { Italic = true, Strikeout = true };

        //Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Font_can_use_bold_with_italic()
    {
        //Act
        var act = () => new Font { Bold = true, Italic = true };

        //Assert
        act.Should().NotThrow();
    }
}
