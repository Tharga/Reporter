using System.Drawing;
using AwesomeAssertions;
using PdfSharp;
using PdfSharp.Drawing;
using Tharga.Reporter.Entity;
using Tharga.Reporter.Entity.Element;
using Tharga.Reporter.Entity.Element.Base;
using Xunit;
using Font = Tharga.Reporter.Entity.Font;
using Rectangle = Tharga.Reporter.Entity.Element.Rectangle;

namespace Tharga.Reporter.Tests.Rendering;

public class Rendering_colours_and_line_thickness
{
    private const string TableName = "Lines";

    [Fact]
    public void A_line_is_drawn_with_its_colour_and_thickness()
    {
        //Act
        var graphics = Render(new Line { Color = Color.Red, Thickness = "2mm" });

        //Assert
        var pen = graphics.Lines.Single().Pen;
        Argb(pen.Color).Should().Be(Argb(Color.Red));
        pen.Width.Should().BeApproximately(new XUnit(2, XGraphicsUnit.Millimeter).Point, 0.0001);
    }

    [Fact]
    public void A_line_with_no_thickness_set_keeps_the_width_it_has_always_been_drawn_with()
    {
        //Arrange
        var line = new Line();

        //Act
        var graphics = Render(line);

        //Assert
        graphics.Lines.Single().Pen.Width.Should().Be(1);
        line.Thickness.Should().Be(UnitValue.Parse("1px"));
    }

    [Fact]
    public void A_rectangle_is_drawn_with_its_border_colour_and_filled_with_its_background_colour()
    {
        //Act
        var graphics = Render(new Rectangle { BorderColor = Color.Green, BackgroundColor = Color.LightYellow, BorderWidth = "2px" });

        //Assert
        var rectangle = graphics.Rectangles.Single();
        Argb(rectangle.Pen.Color).Should().Be(Argb(Color.Green));
        rectangle.Pen.Width.Should().Be(2);
        Argb(rectangle.Brush).Should().Be(Argb(Color.LightYellow));
    }

    [Fact]
    public void A_rectangle_with_no_background_colour_is_outlined_in_its_border_colour()
    {
        //Act
        var graphics = Render(new Rectangle { BorderColor = Color.Green });

        //Assert
        var rectangle = graphics.Rectangles.Single();
        Argb(rectangle.Pen.Color).Should().Be(Argb(Color.Green));
        rectangle.Brush.Should().BeNull();
    }

    [Fact]
    public void A_text_is_drawn_in_its_font_colour()
    {
        //Act
        var graphics = Render(new Text { Value = "Hello", Font = new Font { Color = Color.Blue } });

        //Assert
        Argb(graphics.Strings.Single().Brush).Should().Be(Argb(Color.Blue));
    }

    [Fact]
    public void A_text_with_no_font_of_its_own_is_drawn_in_the_colour_of_the_section_default_font()
    {
        //Arrange
        var section = new Section { DefaultFont = new Font { Color = Color.Purple } };
        section.Pane.ElementList.Add(new Text { Value = "Hello" });

        //Act
        var graphics = Render(section);

        //Assert
        Argb(graphics.Strings.Single().Brush).Should().Be(Argb(Color.Purple));
    }

    [Fact]
    public void A_text_box_is_drawn_in_its_font_colour()
    {
        //Act
        var graphics = Render(new TextBox { Value = "Hello", Font = new Font { Color = Color.Blue } });

        //Assert
        Argb(graphics.Strings.Single().Brush).Should().Be(Argb(Color.Blue));
    }

    [Fact]
    public void A_table_draws_its_texts_in_their_font_colours()
    {
        //Arrange
        var table = CreateTable();
        table.HeaderFont = new Font { Color = Color.Red };
        table.ContentFont = new Font { Color = Color.Green };
        table.GroupFont = new Font { Color = Color.Blue };

        //Act
        var graphics = Render(table, CreateTableData());

        //Assert
        var brushes = graphics.Strings.ToDictionary(x => x.Text, x => Argb(x.Brush));
        brushes["Item"].Should().Be(Argb(Color.Red));
        brushes["Widget"].Should().Be(Argb(Color.Green));
        brushes["Group"].Should().Be(Argb(Color.Blue));
    }

    [Fact]
    public void A_table_draws_its_borders_and_backgrounds_in_their_colours()
    {
        //Arrange
        var table = CreateTable();
        table.HeaderBorderColor = Color.DarkGray;
        table.HeaderBackgroundColor = Color.LightGray;
        table.ContentBorderColor = Color.Orange;
        table.ContentBackgroundColor = Color.Yellow;
        table.GroupBorderColor = Color.Brown;
        table.GroupBackgroundColor = Color.Pink;

        //Act
        var graphics = Render(table, CreateTableData());

        //Assert
        graphics.Rectangles.Select(x => (Border: Argb(x.Pen?.Color), Background: Argb(x.Brush))).Should().BeEquivalentTo(new[]
        {
            (Argb(Color.Orange), Argb(Color.Yellow)),
            (Argb(Color.DarkGray), Argb(Color.LightGray)),
            (Argb(Color.Brown), Argb(Color.Pink))
        });
    }

    [Fact]
    public void A_table_area_with_only_a_background_colour_is_filled_without_a_border()
    {
        //Arrange
        var table = CreateTable();
        table.HeaderBackgroundColor = Color.LightGray;

        //Act
        var graphics = Render(table, CreateTableData());

        //Assert
        var rectangle = graphics.Rectangles.Single();
        rectangle.Pen.Should().BeNull();
        Argb(rectangle.Brush).Should().Be(Argb(Color.LightGray));
    }

    private static Table CreateTable()
    {
        var table = new Table { Name = TableName };
        table.AddColumn("{Item}", "Item", "50mm", Table.WidthMode.Specific);
        return table;
    }

    private static DocumentData CreateTableData()
    {
        var lines = new DocumentDataTable(TableName);
        lines.AddGroup("Group");
        lines.AddRow(new Dictionary<string, string> { { "Item", "Widget" } });

        var data = new DocumentData();
        data.Add(lines);
        return data;
    }

    private static RecordingGraphicsFactory Render(Element element, DocumentData data = null)
    {
        var section = new Section();
        section.Pane.ElementList.Add(element);
        return Render(section, data);
    }

    private static RecordingGraphicsFactory Render(Section section, DocumentData data = null)
    {
        var graphics = new RecordingGraphicsFactory();
        new Renderer(graphics, new Template(section), data).GetPdfBinary(PageSize.A4);
        return graphics;
    }

    private static string Argb(Color color) => $"{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
    private static string Argb(XColor? color) => color is { } x ? $"{(byte)Math.Round(x.A * 255):X2}{x.R:X2}{x.G:X2}{x.B:X2}" : null;
    private static string Argb(XBrush brush) => Argb((brush as XSolidBrush)?.Color);
}
