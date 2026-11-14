using AwesomeAssertions;
using PdfSharp;
using Tharga.Reporter.Entity;
using Tharga.Reporter.Entity.Element;
using Xunit;

namespace Tharga.Reporter.Tests.Rendering;

public class Rendering_multi_page_elements_in_a_header_or_footer
{
    private const string HeaderText = "Header text";
    private const string TableName = "Totals";
    private const string TotalText = "Total 123";

    [Fact]
    public void A_text_box_in_the_header_is_drawn_on_every_page()
    {
        //Arrange
        var section = CreateSectionWithSeveralPages();
        section.Header.Height = "20mm";
        section.Header.ElementList.Add(new TextBox { Value = HeaderText, Left = "0", Top = "0", Width = "100mm", Height = "10mm" });
        var graphics = new RecordingGraphicsFactory();

        //Act
        new Renderer(graphics, new Template(section)).GetPdfBinary(PageSize.A4);

        //Assert
        var pages = graphics.Strings.Select(x => x.Page).Distinct().Count();
        pages.Should().BeGreaterThan(1);
        graphics.Strings.Where(x => x.Text.Trim() == HeaderText).Select(x => x.Page).Should().BeEquivalentTo(Enumerable.Range(0, pages));
    }

    [Fact]
    public void A_table_in_the_footer_is_drawn_on_every_page()
    {
        //Arrange
        var section = CreateSectionWithSeveralPages();
        section.Footer.Height = "20mm";
        section.Footer.ElementList.Add(CreateTotalsTable());
        var graphics = new RecordingGraphicsFactory();

        //Act
        new Renderer(graphics, new Template(section), CreateTotalsData()).GetPdfBinary(PageSize.A4);

        //Assert
        var pages = graphics.Strings.Select(x => x.Page).Distinct().Count();
        pages.Should().BeGreaterThan(1);
        graphics.Strings.Where(x => x.Text == TotalText).Select(x => x.Page).Should().BeEquivalentTo(Enumerable.Range(0, pages));
    }

    [Fact]
    public void A_table_in_the_footer_of_a_document_with_one_page_is_drawn()
    {
        //Arrange
        var section = new Section();
        section.Footer.Height = "20mm";
        section.Footer.ElementList.Add(CreateTotalsTable());
        var graphics = new RecordingGraphicsFactory();

        //Act
        new Renderer(graphics, new Template(section), CreateTotalsData()).GetPdfBinary(PageSize.A4);

        //Assert
        graphics.Strings.Should().Contain(x => x.Text == TotalText);
    }

    [Fact]
    public void A_text_box_in_a_reference_point_in_the_header_is_drawn()
    {
        //Arrange
        var section = new Section();
        section.Header.Height = "20mm";
        var referencePoint = new ReferencePoint();
        referencePoint.ElementList.Add(new TextBox { Value = HeaderText, Width = "100mm", Height = "10mm" });
        section.Header.ElementList.Add(referencePoint);
        var graphics = new RecordingGraphicsFactory();

        //Act
        new Renderer(graphics, new Template(section)).GetPdfBinary(PageSize.A4);

        //Assert
        graphics.Strings.Should().Contain(x => x.Text.Trim() == HeaderText);
    }

    private static Section CreateSectionWithSeveralPages()
    {
        var section = new Section();
        var longText = string.Join(" ", Enumerable.Repeat("word", 4000));
        section.Pane.ElementList.Add(new TextBox { Value = longText, Left = "0", Top = "0", Width = "150mm", Height = "200mm" });
        return section;
    }

    private static Table CreateTotalsTable()
    {
        var table = new Table { Name = TableName, Left = "0", Top = "0", Width = "100mm", Height = "10mm" };
        table.AddColumn("{Total}", "Total", "40mm", Table.WidthMode.Specific);
        return table;
    }

    private static DocumentData CreateTotalsData()
    {
        var totals = new DocumentDataTable(TableName);
        totals.AddRow(new Dictionary<string, string> { { "Total", TotalText } });
        var data = new DocumentData();
        data.Add(totals);
        return data;
    }
}
