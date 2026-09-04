using FluentAssertions;
using PdfSharp.Drawing;
using Tharga.Reporter.Fonts;
using Xunit;

namespace Tharga.Reporter.Tests.Fonts;

public class ReporterFontResolver_Tests
{
    private const string UnknownFamily = "NoSuchFontFamilyAnywhere";

    [Fact]
    public void An_unknown_family_falls_back_to_an_embedded_face()
    {
        //Arrange
        var resolver = new ReporterFontResolver();

        //Act
        var info = resolver.ResolveTypeface(UnknownFamily, false, false);
        var data = resolver.GetFont(info.FaceName);

        //Assert
        info.Should().NotBeNull();
        data.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void An_empty_family_name_still_resolves()
    {
        //Arrange
        var resolver = new ReporterFontResolver();

        //Act
        var info = resolver.ResolveTypeface(string.Empty, false, false);
        var data = resolver.GetFont(info.FaceName);

        //Assert
        data.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void A_registered_font_takes_precedence_over_the_fallback()
    {
        //Arrange
        var resolver = new ReporterFontResolver();
        var fontData = resolver.GetFont(resolver.ResolveTypeface(UnknownFamily, false, false).FaceName);
        ReporterFontResolver.Register("MyRegisteredFamily", false, false, fontData);

        //Act
        var info = resolver.ResolveTypeface("MyRegisteredFamily", false, false);

        //Assert
        resolver.GetFont(info.FaceName).Should().BeSameAs(fontData);
    }

    [Fact]
    public void Bold_and_italic_resolve_to_usable_font_data()
    {
        //Arrange
        var resolver = new ReporterFontResolver();

        //Act
        var bold = resolver.GetFont(resolver.ResolveTypeface(UnknownFamily, true, false).FaceName);
        var italic = resolver.GetFont(resolver.ResolveTypeface(UnknownFamily, false, true).FaceName);

        //Assert
        bold.Should().NotBeNullOrEmpty();
        italic.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Text_can_be_measured_without_any_platform_graphics_library()
    {
        //Arrange
        ReporterFontResolver.EnsureInstalled();
        var document = new PdfSharp.Pdf.PdfDocument();
        var graphics = XGraphics.FromPdfPage(document.AddPage());

        //Act
        var size = graphics.MeasureString("Tharga", new XFont("Verdana", 10, XFontStyleEx.Regular));

        //Assert
        size.Width.Should().BeGreaterThan(0);
        size.Height.Should().BeGreaterThan(0);
    }
}
