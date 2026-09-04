using FluentAssertions;
using Tharga.Reporter.Entity;
using Tharga.Reporter.Entity.Element;
using Xunit;

namespace Tharga.Reporter.Tests.Rendering;

public class BarCode_Samples
{
    private static readonly string[] SampleCodes = ["ABC123", "S1309799801", "MEMBER0042"];

    private readonly ITestOutputHelper _output;

    public BarCode_Samples(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Write_scannable_samples_for_manual_verification()
    {
        //Arrange
        Directory.CreateDirectory(BarCodeSampleWriter.OutputDirectory);

        //Act
        var pngs = SampleCodes.Select(BarCodeSampleWriter.WritePng).ToArray();
        var pdf = WriteSamplePdf();

        //Assert
        pngs.Should().OnlyContain(x => new FileInfo(x).Length > 0);
        new FileInfo(pdf).Length.Should().BeGreaterThan(0);

        _output.WriteLine($"Samples written to {BarCodeSampleWriter.OutputDirectory}");
        foreach (var path in pngs.Append(pdf))
        {
            _output.WriteLine(path);
        }
    }

    private static string WriteSamplePdf()
    {
        var sections = SampleCodes.Select(BuildCardSection).ToArray();

        var template = new Template(sections[0]);
        foreach (var section in sections.Skip(1))
        {
            template.SectionList.Add(section);
        }

        var pdf = new Renderer(template).GetPdfBinary(PageFormat.PlasticCard);

        var path = Path.Combine(BarCodeSampleWriter.OutputDirectory, "barcode-samples.pdf");
        File.WriteAllBytes(path, pdf);

        return path;
    }

    private static Section BuildCardSection(string code)
    {
        var section = new Section();
        section.Pane.ElementList.Add(new BarCode { Code = code, Top = "8mm", Left = "8mm", Bottom = "22mm", Right = "8mm" });
        section.Pane.ElementList.Add(new Text { Value = code, Top = "36mm", Left = "8mm", Right = "8mm" });

        return section;
    }
}
