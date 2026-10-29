using AwesomeAssertions;
using PdfSharp;
using Tharga.Reporter.Entity;
using Tharga.Reporter.Entity.Element;
using Xunit;

namespace Tharga.Reporter.Tests.Rendering;

public class Rendering_one_template_from_several_renderers
{
    private const string TableName = "Lines";
    private const int FinalDocument = 1;

    [Fact]
    public void A_render_in_between_does_not_change_the_rows_or_page_count_of_a_table()
    {
        //Arrange
        var template = CreateTableTemplate();
        var expected = RenderAlone(CreateTableTemplate(), CreateRows(150));

        //Act
        var actual = RenderWithAnotherRenderInBetween(template, CreateRows(150), CreateRows(2));

        //Assert
        actual.Should().Equal(expected);
    }

    [Fact]
    public void A_render_in_between_does_not_change_the_lines_of_a_text_box()
    {
        //Arrange
        var template = CreateTextBoxTemplate();
        var longText = CreateText(string.Join(" ", Enumerable.Range(0, 3000).Select(x => $"word{x}")));
        var expected = RenderAlone(CreateTextBoxTemplate(), longText);

        //Act
        var actual = RenderWithAnotherRenderInBetween(template, longText, CreateText("Short"));

        //Assert
        actual.Should().Equal(expected);
    }

    [Fact]
    public void A_vertical_stack_does_not_position_its_children_by_changing_the_template()
    {
        //Arrange
        var first = new Text { Value = "First" };
        var second = new Text { Value = "Second" };
        var referencePoint = new ReferencePoint { Stack = ReferencePoint.StackMethod.Vertical };
        referencePoint.ElementList.Add(first);
        referencePoint.ElementList.Add(second);
        var section = new Section();
        section.Pane.ElementList.Add(referencePoint);

        var topsWhileDrawing = new List<UnitValue?>();
        var graphics = new RecordingGraphicsFactory { Drawn = _ => topsWhileDrawing.Add(first.Top ?? second.Top) };

        //Act
        new Renderer(graphics, new Template(section)).GetPdfBinary(PageSize.A4);

        //Assert
        topsWhileDrawing.Should().NotBeEmpty().And.AllSatisfy(x => x.Should().BeNull());
        var drawn = graphics.Strings.ToDictionary(x => x.Text, x => x.Location.Y);
        drawn["Second"].Should().BeGreaterThan(drawn["First"]);
    }

    [Fact]
    public async Task Renders_running_at_the_same_time_give_the_same_result_as_one_at_a_time()
    {
        //Arrange
        var template = CreateTableTemplate();
        var data = new[] { 150, 2, 90, 7 };
        var expected = data.ToDictionary(x => x, x => RenderAlone(CreateTableTemplate(), CreateRows(x)));

        //Act
        var renders = Enumerable.Range(0, 32)
            .Select(x => data[x % data.Length])
            .Select(rows => Task.Run(() => (Rows: rows, Result: RenderAlone(template, CreateRows(rows)))));
        var results = await Task.WhenAll(renders);

        //Assert
        results.Should().AllSatisfy(x => x.Result.Should().Equal(expected[x.Rows]));
    }

    private static IReadOnlyList<(int Page, string Text, double X, double Y)> RenderAlone(Template template, DocumentData data)
    {
        var graphics = new RecordingGraphicsFactory();
        new Renderer(graphics, template, data).GetPdfBinary(PageSize.A4);
        return Summarize(graphics);
    }

    private static IReadOnlyList<(int Page, string Text, double X, double Y)> RenderWithAnotherRenderInBetween(Template template, DocumentData data, DocumentData otherData)
    {
        var graphics = new RecordingGraphicsFactory
        {
            DocumentStarted = document =>
            {
                if (document == FinalDocument)
                {
                    new Renderer(template, otherData).GetPdfBinary(PageSize.A4);
                }
            }
        };

        new Renderer(graphics, template, data).GetPdfBinary(PageSize.A4);
        return Summarize(graphics);
    }

    private static IReadOnlyList<(int Page, string Text, double X, double Y)> Summarize(RecordingGraphicsFactory graphics)
    {
        return graphics.Strings.Select(x => (x.Page, x.Text, Math.Round(x.Location.X, 3), Math.Round(x.Location.Y, 3))).ToArray();
    }

    private static Template CreateTableTemplate()
    {
        var table = new Table { Name = TableName };
        table.AddColumn("{Item}", "Item", "50mm", Table.WidthMode.Specific);

        var section = new Section();
        section.Pane.ElementList.Add(table);
        section.Footer.Height = "1cm";
        section.Footer.ElementList.Add(new Text { Value = "Page {PageNumber} of {TotalPages}" });
        return new Template(section);
    }

    private static Template CreateTextBoxTemplate()
    {
        var section = new Section();
        section.Pane.ElementList.Add(new TextBox { Value = "{Body}" });
        section.Footer.Height = "1cm";
        section.Footer.ElementList.Add(new Text { Value = "Page {PageNumber} of {TotalPages}" });
        return new Template(section);
    }

    private static DocumentData CreateRows(int count)
    {
        var lines = new DocumentDataTable(TableName);
        for (var i = 0; i < count; i++)
        {
            lines.AddRow(new Dictionary<string, string> { { "Item", $"Row {i}" } });
        }

        var data = new DocumentData();
        data.Add(lines);
        return data;
    }

    private static DocumentData CreateText(string body)
    {
        var data = new DocumentData();
        data.Add("Body", body);
        return data;
    }
}
