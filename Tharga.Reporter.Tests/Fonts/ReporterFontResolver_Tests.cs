using AwesomeAssertions;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
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
        var data = resolver.GetFont(Resolve(resolver, UnknownFamily).FaceName);

        //Assert
        data.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void An_empty_family_name_still_resolves()
    {
        //Arrange
        var resolver = new ReporterFontResolver();

        //Act
        var data = resolver.GetFont(Resolve(resolver, string.Empty).FaceName);

        //Assert
        data.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void A_registered_font_takes_precedence_over_the_fallback()
    {
        //Arrange
        var resolver = new ReporterFontResolver();
        var fontData = resolver.GetFont(Resolve(resolver, UnknownFamily).FaceName);
        ReporterFontResolver.Register("MyRegisteredFamily", false, false, fontData);

        //Act
        var info = Resolve(resolver, "MyRegisteredFamily");

        //Assert
        resolver.GetFont(info.FaceName).Should().BeSameAs(fontData);
    }

    [Fact]
    public void Bold_and_italic_resolve_to_usable_font_data()
    {
        //Arrange
        var resolver = new ReporterFontResolver();

        //Act
        var bold = resolver.GetFont(Resolve(resolver, UnknownFamily, isBold: true).FaceName);
        var italic = resolver.GetFont(Resolve(resolver, UnknownFamily, isItalic: true).FaceName);

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

    private static FontResolverInfo Resolve(ReporterFontResolver resolver, string familyName, bool isBold = false, bool isItalic = false)
    {
        var info = resolver.ResolveTypeface(familyName, isBold, isItalic);
        info.Should().NotBeNull();
        return info;
    }
}
