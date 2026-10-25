using System.Drawing;
using AwesomeAssertions;
using Tharga.Reporter.Entity;
using Tharga.Reporter.Entity.Element;
using Tharga.Reporter.Entity.Element.Base;
using Xunit;
using Font = Tharga.Reporter.Entity.Font;

namespace Tharga.Reporter.Tests.Serializing;

public class Text_Tests
{
    [Fact]
    public void Default_Text()
    {
        //Arrange
        var text = new Text();
        var xme = text.ToXme();

        //Act
        var other = Text.Load(xme);

        //Assert
        other.Left.Should().Be(text.Left);
        other.Right.Should().Be(text.Right);
        other.Width.Should().Be(text.Width);
        other.Top.Should().Be(text.Top);
        other.Bottom.Should().Be(text.Bottom);
        other.Height.Should().Be(text.Height);
        other.Font.FontName.Should().Be(text.Font.FontName);
        other.Font.Size.Should().Be(text.Font.Size);
        other.Font.Color.Should().Be(text.Font.Color);
        other.FontClass.Should().Be(text.FontClass);
        other.HideValue.Should().Be(text.HideValue);
        other.TextAlignment.Should().Be(text.TextAlignment);
        other.Value.Should().Be(text.Value);
        other.IsBackground.Should().Be(text.IsBackground);
        other.Name.Should().Be(text.Name);
        other.Visibility.Should().Be(text.Visibility);
        other.ToXme().OuterXml.Should().Be(xme.OuterXml);
    }

    [Fact]
    public void Text_with_fontclass()
    {
        //Arrange
        var text = new Text
        {
            FontClass = "Yahoo"
        };
        var xme = text.ToXme();

        //Act
        var other = Text.Load(xme);

        //Assert
        other.FontClass.Should().Be("Yahoo");
        other.Font.FontName.Should().Be(text.Font.FontName);
        other.ToXme().OuterXml.Should().Be(xme.OuterXml);
    }

    [Fact]
    public void Text_with_all_propreties_set()
    {
        //Arrange
        var text = new Text
        {
            IsBackground = true,
            Name = "Rea Padda",
            Right = UnitValue.Parse("1px"),
            Width = UnitValue.Parse("10cm"),
            Bottom = UnitValue.Parse("2px"),
            Top = UnitValue.Parse("3px"),
            HideValue = "ABC",
            TextAlignment = TextBase.Alignment.Right,
            Value = "Bob Loblaw",
            Font = new Font
            {
                FontName = "Verdana",
                Color = Color.MistyRose,
                Size = 13
            },
            Visibility = PageVisibility.AllButLast
        };
        var xme = text.ToXme();

        //Act
        var other = Text.Load(xme);

        //Assert
        other.Left.Should().Be(text.Left);
        other.Right.Should().Be(UnitValue.Parse("1px"));
        other.Width.Should().Be(UnitValue.Parse("10cm"));
        other.Top.Should().Be(UnitValue.Parse("3px"));
        other.Bottom.Should().Be(UnitValue.Parse("2px"));
        other.Height.Should().Be(text.Height);
        other.Font.FontName.Should().Be("Verdana");
        other.Font.Size.Should().Be(13);
        other.Font.Color.ToArgb().Should().Be(Color.MistyRose.ToArgb());
        other.HideValue.Should().Be("ABC");
        other.Visibility.Should().Be(PageVisibility.AllButLast);
        other.TextAlignment.Should().Be(TextBase.Alignment.Right);
        other.Value.Should().Be("Bob Loblaw");
        other.IsBackground.Should().BeTrue();
        other.Name.Should().Be("Rea Padda");
        other.ToXme().OuterXml.Should().Be(xme.OuterXml);
    }
}
