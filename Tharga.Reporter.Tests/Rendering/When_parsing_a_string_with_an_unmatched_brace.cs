using AwesomeAssertions;
using PdfSharp;
using Tharga.Reporter.Entity;
using Tharga.Reporter.Entity.Element;
using Tharga.Reporter.Extensions;
using Xunit;

namespace Tharga.Reporter.Tests.Rendering;

public class When_parsing_a_string_with_an_unmatched_brace
{
    [Fact]
    public void A_brace_with_no_closing_brace_is_kept_as_text()
    {
        //Arrange
        var input = "Price {approx";

        //Act
        var result = input.ParseValue(new DocumentData(), null);

        //Assert
        result.Should().Be(input);
    }

    [Fact]
    public void Placeholders_before_an_unmatched_brace_are_still_replaced()
    {
        //Arrange
        var documentData = new DocumentData();
        documentData.Add("Amount", "100");

        //Act
        var result = "{Amount} or {approx".ParseValue(documentData, null);

        //Assert
        result.Should().Be("100 or {approx");
    }

    [Fact]
    public void A_table_cell_with_no_closing_brace_is_kept_as_text()
    {
        //Arrange
        var row = new Dictionary<string, string> { { "Amount", "100" } };

        //Act
        var result = "{Amount} or {approx".ParseValue(row);

        //Assert
        result.Should().Be("100 or {approx");
    }

    [Fact]
    public void Rendering_a_text_with_no_closing_brace_does_not_throw()
    {
        //Arrange
        var section = new Section();
        section.Pane.ElementList.Add(new Text { Value = "Price {approx" });
        var renderer = new Renderer(new Template(section));

        //Act
        var act = () => renderer.GetPdfBinary(PageSize.A4);

        //Assert
        act.Should().NotThrow();
    }
}
