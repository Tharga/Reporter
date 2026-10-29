using AwesomeAssertions;
using PdfSharp;
using Tharga.Reporter.Entity;
using Tharga.Reporter.Entity.Element;
using Xunit;

namespace Tharga.Reporter.Tests.Rendering;

public class Rendering_a_table_from_a_reused_template
{
    private const string TableName = "Lines";
    private const string LongName = "A customer name that is a lot wider than the forty millimetres the column was given";

    [Fact]
    public void Rendering_leaves_the_column_widths_of_the_template_as_configured()
    {
        //Arrange
        var template = CreateTemplate();
        var before = template.ToXml().OuterXml;

        //Act
        Render(template, CreateData(LongName, "0"));
        Render(template, CreateData(LongName, "10"));

        //Assert
        template.ToXml().OuterXml.Should().Be(before);
    }

    [Fact]
    public void A_document_gets_the_same_layout_whatever_was_rendered_from_the_template_before()
    {
        //Arrange
        var template = CreateTemplate();
        Render(template, CreateData(LongName, "0"));
        Render(template, CreateData(LongName, "10"));

        //Act
        var reused = Render(template, CreateData("B", "10"));

        //Assert
        var fresh = Render(CreateTemplate(), CreateData("B", "10"));
        reused.Should().Equal(fresh);
    }

    private static Template CreateTemplate()
    {
        var table = new Table { Name = TableName };
        table.AddColumn("{Name}", "Name", "40mm", Table.WidthMode.Specific);
        table.AddColumn("{Discount}", "Discount", "30mm", Table.WidthMode.Specific, hideValue: "0");
        table.AddColumn("{Amount}", "Amount", "20mm", Table.WidthMode.Specific);

        var section = new Section();
        section.Pane.ElementList.Add(table);
        return new Template(section);
    }

    private static DocumentData CreateData(string name, string discount)
    {
        var lines = new DocumentDataTable(TableName);
        lines.AddRow(new Dictionary<string, string> { { "Name", name }, { "Discount", discount }, { "Amount", "100" } });

        var data = new DocumentData();
        data.Add(lines);
        return data;
    }

    private static IReadOnlyList<(string Text, double X, double Y)> Render(Template template, DocumentData data)
    {
        var graphics = new RecordingGraphicsFactory();
        new Renderer(graphics, template, data).GetPdfBinary(PageSize.A4);
        return graphics.Strings.Select(x => (x.Text, Math.Round(x.Location.X, 3), Math.Round(x.Location.Y, 3))).ToArray();
    }
}
