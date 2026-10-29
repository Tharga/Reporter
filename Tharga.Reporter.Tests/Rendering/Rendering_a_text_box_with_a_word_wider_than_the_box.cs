using AwesomeAssertions;
using PdfSharp;
using Tharga.Reporter.Entity;
using Tharga.Reporter.Entity.Element;
using Xunit;

namespace Tharga.Reporter.Tests.Rendering;

public class Rendering_a_text_box_with_a_word_wider_than_the_box
{
    private const string WideWord = "Supercalifragilistic";

    public static TheoryData<string, string[]> Texts => new()
    {
        { $"{WideWord} ab", [$"{WideWord} ", "ab "] },
        { WideWord, [$"{WideWord} "] },
        { $"{WideWord} {WideWord}", [$"{WideWord} ", $"{WideWord} "] },
        { $"ab {WideWord} cd", ["ab ", $"{WideWord} ", "cd "] },
    };

    [Theory]
    [MemberData(nameof(Texts))]
    public void Each_word_is_drawn_once(string value, string[] expectedLines)
    {
        //Arrange
        var section = new Section();
        section.Pane.ElementList.Add(new TextBox { Value = value, Width = "50" });
        var graphics = new RecordingGraphicsFactory();

        //Act
        new Renderer(graphics, new Template(section)).GetPdfBinary(PageSize.A4);

        //Assert
        graphics.Strings.Select(x => x.Text).Should().Equal(expectedLines);
    }

    [Fact]
    public void A_word_wider_than_the_box_that_fills_the_page_adds_no_extra_page()
    {
        //Arrange
        var section = new Section();
        section.Pane.ElementList.Add(new TextBox { Value = WideWord, Width = "50", Height = "15" });
        var graphics = new PageCountingGraphicsFactory();

        //Act
        new Renderer(graphics, new Template(section)).GetPdfBinary(PageSize.A4);

        //Assert
        graphics.RenderedPageCount.Should().Be(1);
    }
}
