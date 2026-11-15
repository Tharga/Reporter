using AwesomeAssertions;
using PdfSharp;
using PdfSharp.Drawing;
using Tharga.Reporter.Entity;
using Tharga.Reporter.Entity.Element;
using Xunit;
using Font = Tharga.Reporter.Entity.Font;

namespace Tharga.Reporter.Tests.Rendering;

public class Rendering_fonts_read_through_getters
{
    private const string TableName = "Lines";

    [Fact]
    public void A_font_changed_through_the_getter_of_a_text_is_drawn_and_saved()
    {
        //Arrange
        var text = new Text { Value = "Hello", Font = { Size = 20, Bold = true } };
        var section = new Section();
        section.Pane.ElementList.Add(text);

        //Act
        var drawn = Render(section).Strings.Single();

        //Assert
        drawn.Font.Size.Should().Be(20);
        drawn.Font.Style.Should().Be(XFontStyleEx.Bold);
        var other = (Text)Template.Load(new Template(section).ToXml()).SectionList.Single().Pane.ElementList.Single();
        other.Font.Size.Should().Be(20);
        other.Font.Bold.Should().BeTrue();
    }

    [Fact]
    public void A_font_changed_through_the_getter_of_a_text_box_is_drawn()
    {
        //Arrange
        var section = new Section();
        section.Pane.ElementList.Add(new TextBox { Value = "Hello", Font = { Size = 20, Bold = true } });

        //Act
        var drawn = Render(section).Strings.Single();

        //Assert
        drawn.Font.Size.Should().Be(20);
        drawn.Font.Style.Should().Be(XFontStyleEx.Bold);
    }

    [Fact]
    public void A_header_font_changed_through_the_getter_of_a_table_is_drawn()
    {
        //Arrange
        var table = CreateTable();
        table.HeaderFont.Bold = true;
        table.HeaderFont.Size = 16;

        //Act
        var drawn = Render(CreateSection(table), CreateData()).Strings.ToDictionary(x => x.Text);

        //Assert
        drawn["Item"].Font.Size.Should().Be(16);
        drawn["Item"].Font.Style.Should().Be(XFontStyleEx.Bold);
    }

    [Fact]
    public void Reading_the_getters_of_a_text_reports_the_font_that_is_drawn_and_leaves_the_output_as_it_was()
    {
        //Arrange
        var text = new Text { Value = "Hello" };
        var section = new Section { DefaultFont = new Font { Size = 14 } };
        section.Pane.ElementList.Add(text);
        var before = Render(section).Strings.Single();

        //Act
        _ = text.Font.Size;
        var after = Render(section).Strings.Single();

        //Assert
        after.Font.Size.Should().Be(before.Font.Size).And.Be(14);
        new Template(section).ToXml().OuterXml.Should().Contain("<Text Value=\"Hello\" />");
    }

    [Fact]
    public void The_header_font_of_a_table_reports_the_size_it_is_drawn_with()
    {
        //Arrange
        var table = CreateTable();
        var reported = table.HeaderFont.Size;

        //Act
        var drawn = Render(CreateSection(table), CreateData()).Strings.ToDictionary(x => x.Text);

        //Assert
        drawn["Item"].Font.Size.Should().Be(reported);
    }

    [Fact]
    public void The_group_font_of_a_table_reports_the_font_it_is_drawn_with_when_only_the_content_font_is_set()
    {
        //Arrange
        var table = CreateTable();
        table.ContentFont = new Font { Size = 14 };
        table.GroupFont.Size.Should().Be(10);

        //Act
        var drawn = Render(CreateSection(table), CreateData()).Strings.ToDictionary(x => x.Text);

        //Assert
        drawn["Widget"].Font.Size.Should().Be(14);
        drawn["Alpha"].Font.Size.Should().Be(table.GroupFont.Size);
    }

    [Fact]
    public void A_font_with_only_some_values_set_takes_the_rest_from_the_font_class_and_the_section_default()
    {
        //Arrange
        var section = new Section { DefaultFont = new Font { Size = 14 } };
        section.FontClasses["Heading"] = new Font { Bold = true };
        section.Pane.ElementList.Add(new Text { Value = "Hello", FontClass = "Heading" });
        section.Pane.ElementList.Add(new Text { Value = "Plain", Top = "1cm", Font = { Italic = true } });

        //Act
        var drawn = Render(section).Strings.ToDictionary(x => x.Text);

        //Assert
        drawn["Hello"].Font.Size.Should().Be(14);
        drawn["Hello"].Font.Style.Should().Be(XFontStyleEx.Bold);
        drawn["Plain"].Font.Size.Should().Be(14);
        drawn["Plain"].Font.Style.Should().Be(XFontStyleEx.Italic);
    }

    [Fact]
    public void Reading_the_font_does_not_stop_a_font_class_from_being_set()
    {
        //Arrange
        var text = new Text { Value = "Hello" };
        _ = text.Font.Size;

        //Act
        var act = () => text.FontClass = "Heading";

        //Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void A_font_and_a_font_class_still_cannot_both_be_set()
    {
        //Arrange
        var text = new Text { Value = "Hello", Font = { Size = 20 } };

        //Act
        var act = () => text.FontClass = "Heading";

        //Assert
        act.Should().Throw<InvalidOperationException>();
    }

    private static Table CreateTable()
    {
        var table = new Table { Name = TableName };
        table.AddColumn("{Item}", "Item", "50mm", Table.WidthMode.Specific);
        return table;
    }

    private static Section CreateSection(Table table)
    {
        var section = new Section();
        section.Pane.ElementList.Add(table);
        return section;
    }

    private static DocumentData CreateData()
    {
        var lines = new DocumentDataTable(TableName);
        lines.AddGroup("Alpha");
        lines.AddRow(new Dictionary<string, string> { { "Item", "Widget" } });
        var data = new DocumentData();
        data.Add(lines);
        return data;
    }

    private static RecordingGraphicsFactory Render(Section section, DocumentData data = null)
    {
        var graphics = new RecordingGraphicsFactory();
        new Renderer(graphics, new Template(section), data).GetPdfBinary(PageSize.A4);
        return graphics;
    }
}
