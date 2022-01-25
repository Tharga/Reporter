using FluentAssertions;
using Tharga.Reporter.Entity;
using Xunit;

namespace Tharga.Reporter.Tests;

public class Engine
{
    [Fact]
    public void Create_template_with_default_section()
    {
        //Arrange
        var section = new Section();

        //Act
        var template = new Template(section);

        //Assert
        template.SectionList.Count.Should().Be(1);

        template.SectionList.First().Header.Should().NotBeNull();
        template.SectionList.First().Header.ElementList.Count.Should().Be(0);
        template.SectionList.First().Header.Height.Should().NotBe(0);
        template.SectionList.First().Header.Height.Unit.Should().Be(UnitValue.EUnit.Point);

        template.SectionList.First().Footer.Should().NotBeNull();
        template.SectionList.First().Footer.ElementList.Count.Should().Be(0);
        template.SectionList.First().Footer.Height.Value.Should().Be(0);
        template.SectionList.First().Footer.Height.Unit.Should().Be(UnitValue.EUnit.Point);

        template.SectionList.First().Pane.Should().NotBeNull();
        template.SectionList.First().Pane.ElementList.Count.Should().Be(0);

        template.SectionList.First().Margin.Should().NotBeNull();
        template.SectionList.First().Margin.Left.Value.Value.Should().Be(0);
        template.SectionList.First().Margin.Left.Value.Unit.Should().Be(UnitValue.EUnit.Point);
        template.SectionList.First().Margin.Right.Value.Value.Should().Be(0);
        template.SectionList.First().Margin.Right.Value.Unit.Should().Be(UnitValue.EUnit.Point);
        template.SectionList.First().Margin.Top.Value.Value.Should().Be(0);
        template.SectionList.First().Margin.Top.Value.Unit.Should().Be(UnitValue.EUnit.Point);
        template.SectionList.First().Margin.Bottom.Value.Value.Should().Be(0);
        template.SectionList.First().Margin.Bottom.Value.Unit.Should().Be(0);
        template.SectionList.First().Margin.Height.Should().BeNull();
        template.SectionList.First().Margin.Width.Should().BeNull();
    }

    //[Test]
    //[Obsolete]
    //public void Create_pdf_document()
    //{
    //    ////Arrange
    //    //var template = new Template(new Section());

    //    ////Act
    //    //var byteArray = Rendering.CreatePDFDocument(template);

    //    ////Assert
    //    //Assert.IsTrue(byteArray.Length > 0);
    //}

    //[Test]
    //public void Serialize_to_xml()
    //{
    //    //Arrange
    //    //var section = Section.Create(); //TODO: Create a section builder that randomizes the construction of a document.
    //    //var text = Text.Create("Some text");
    //    //text.Font.FontName = "Times New Roman";
    //    //section.Header.ElementList.Add(text);
    //    //var template = Template.Create(section);

    //    ////Act
    //    //var xml = template.ToXml();
    //    //var otherTemplate = Template.Create(xml);
    //    //var otherXml = otherTemplate.ToXml();

    //    ////Assert
    //    xml.InnerXml, otherXml.InnerXml);
    //    ////TODO: Also assert that the properties of the objects are equal
    //}
}