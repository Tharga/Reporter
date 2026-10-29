using System.Drawing;
using AwesomeAssertions;
using PdfSharp;
using PdfSharp.Drawing;
using Tharga.Reporter.Entity;
using Tharga.Reporter.Entity.Element;
using Xunit;
using Font = Tharga.Reporter.Entity.Font;

namespace Tharga.Reporter.Tests.Rendering;

public class Rendering_font_classes
{
    private const string TableName = "Lines";
    private const string Heading = "Heading";

    [Fact]
    public void A_text_with_a_font_class_is_drawn_with_the_font_of_that_class()
    {
        //Arrange
        var section = CreateSection();
        section.Pane.ElementList.Add(new Text { Value = "Hello", FontClass = Heading });

        //Act
        var drawn = Render(section).Strings.Single();

        //Assert
        AssertHeadingFont(drawn);
    }

    [Fact]
    public void A_text_box_with_a_font_class_is_drawn_with_the_font_of_that_class()
    {
        //Arrange
        var section = CreateSection();
        section.Pane.ElementList.Add(new TextBox { Value = "Hello", FontClass = Heading });

        //Act
        var drawn = Render(section).Strings.Single();

        //Assert
        AssertHeadingFont(drawn);
    }

    [Fact]
    public void A_table_draws_its_header_and_content_with_the_fonts_of_their_classes()
    {
        //Arrange
        var section = CreateSection();
        section.FontClasses["Content"] = new Font { Size = 8, Italic = true, Color = Color.Green };
        var table = new Table { Name = TableName, HeaderFontClass = Heading, ContentFontClass = "Content" };
        table.AddColumn("{Item}", "Item", "50mm", Table.WidthMode.Specific);
        section.Pane.ElementList.Add(table);

        var lines = new DocumentDataTable(TableName);
        lines.AddRow(new Dictionary<string, string> { { "Item", "Widget" } });
        var data = new DocumentData();
        data.Add(lines);

        //Act
        var drawn = Render(section, data).Strings.ToDictionary(x => x.Text);

        //Assert
        AssertHeadingFont(drawn["Item"]);
        drawn["Widget"].Font.Size.Should().Be(8);
        drawn["Widget"].Font.Style.Should().Be(XFontStyleEx.Italic);
        ((XSolidBrush)drawn["Widget"].Brush).Color.G.Should().Be(Color.Green.G);
    }

    [Fact]
    public void A_font_class_that_is_not_defined_falls_back_to_the_section_default_font()
    {
        //Arrange
        var section = CreateSection();
        section.DefaultFont = new Font { Size = 14 };
        section.Pane.ElementList.Add(new Text { Value = "Hello", FontClass = "Missing" });

        //Act
        var drawn = Render(section).Strings.Single();

        //Assert
        drawn.Font.Size.Should().Be(14);
    }

    private static Section CreateSection()
    {
        var section = new Section();
        section.FontClasses[Heading] = new Font { Size = 20, Bold = true, Color = Color.Red };
        return section;
    }

    private static void AssertHeadingFont(DrawnString drawn)
    {
        drawn.Font.Size.Should().Be(20);
        drawn.Font.Style.Should().Be(XFontStyleEx.Bold);
        ((XSolidBrush)drawn.Brush).Color.R.Should().Be(Color.Red.R);
        ((XSolidBrush)drawn.Brush).Color.G.Should().Be(Color.Red.G);
    }

    private static RecordingGraphicsFactory Render(Section section, DocumentData data = null)
    {
        var graphics = new RecordingGraphicsFactory();
        new Renderer(graphics, new Template(section), data).GetPdfBinary(PageSize.A4);
        return graphics;
    }
}
