using System.Drawing;
using AwesomeAssertions;
using PdfSharp;
using Tharga.Reporter.Entity;
using Tharga.Reporter.Entity.Element;
using Tharga.Reporter.Tests.Rendering;
using Xunit;
using Font = Tharga.Reporter.Entity.Font;

namespace Tharga.Reporter.Tests.Serializing;

public class TextBox_Tests
{
    [Fact]
    public void Default_Text()
    {
        //Arrange
        var text = new TextBox();
        var xme = text.ToXme();

        //Act
        var other = TextBox.Load(xme);

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
        other.Value.Should().Be(text.Value);
        other.IsBackground.Should().Be(text.IsBackground);
        other.Name.Should().Be(text.Name);
        other.ToXme().OuterXml.Should().Be(xme.OuterXml);
    }

    [Fact]
    public void Text_with_fontclass()
    {
        //Arrange
        var text = new TextBox
        {
            FontClass = "Yahoo"
        };
        var xme = text.ToXme();

        //Act
        var other = TextBox.Load(xme);

        //Assert
        other.FontClass.Should().Be("Yahoo");
        other.Font.FontName.Should().Be(text.Font.FontName);
        other.ToXme().OuterXml.Should().Be(xme.OuterXml);
    }

    [Fact]
    public void Text_with_all_propreties_set()
    {
        //Arrange
        var text = new TextBox
        {
            IsBackground = true,
            Name = "Rea Padda",
            Right = UnitValue.Parse("1px"),
            Width = UnitValue.Parse("10cm"),
            Bottom = UnitValue.Parse("2px"),
            Top = UnitValue.Parse("3px"),
            HideValue = "ABC",
            Value = "Bob Loblaw",
            Font = new Font
            {
                FontName = "Verdana",
                Color = Color.MistyRose,
                Size = 13
            }
        };
        var xme = text.ToXme();

        //Act
        var other = TextBox.Load(xme);

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
        other.Value.Should().Be("Bob Loblaw");
        other.IsBackground.Should().BeTrue();
        other.Name.Should().Be("Rea Padda");
        other.ToXme().OuterXml.Should().Be(xme.OuterXml);
    }

    [Fact]
    public void When_using_one_template_and_rendering_several_times_with_different_data()
    {
        //Arrange
        var section = new Section();
        section.Pane.ElementList.Add(new TextBox { Value = "{Data}", Width = "2cm", Height = "2cm" });
        var template = new Template(section);
        var shortData = new DocumentData();
        shortData.Add("Data", "AAA BBB CCC DDD EEE FFF GGG HHH III JJJ KKK LLL MMM NNN OOO PPP QQQ RRR SSS TTT");
        var longData = new DocumentData();
        longData.Add("Data", "AAA BBB CCC DDD EEE FFF GGG HHH III JJJ KKK LLL MMM NNN OOO PPP QQQ RRR SSS TTT UUU VVV XXX YYY ZZZ AAA BBB CCC DDD EEE FFF GGG HHH III JJJ KKK LLL MMM NNN OOO");

        //Act
        var first = Render(template, shortData);
        var second = Render(template, longData);
        var third = Render(template, shortData);

        //Assert
        first.Should().BeGreaterThan(1);
        second.Should().BeGreaterThan(first);
        third.Should().Be(first);
    }

    private static int Render(Template template, DocumentData documentData)
    {
        var graphicsFactory = new PageCountingGraphicsFactory();
        var pdf = new Renderer(graphicsFactory, template, documentData).GetPdfBinary(PageSize.A4);
        pdf.Should().NotBeEmpty();
        return graphicsFactory.RenderedPageCount;
    }
}
