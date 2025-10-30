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

        var first = template.SectionList.First();
        first.Header.Should().NotBeNull();
        first.Header.ElementList.Count.Should().Be(0);
        first.Header.Height.Should().NotBe(0);
        first.Header.Height.Unit.Should().Be(UnitValue.EUnit.Point);

        first.Footer.Should().NotBeNull();
        first.Footer.ElementList.Count.Should().Be(0);
        first.Footer.Height.Value.Should().Be(0);
        first.Footer.Height.Unit.Should().Be(UnitValue.EUnit.Point);

        first.Pane.Should().NotBeNull();
        first.Pane.ElementList.Count.Should().Be(0);

        first.Margin.Should().NotBeNull();
        first.Margin.Left.Value.Value.Should().Be(0);
        first.Margin.Left.Value.Unit.Should().Be(UnitValue.EUnit.Point);
        first.Margin.Right.Value.Value.Should().Be(0);
        first.Margin.Right.Value.Unit.Should().Be(UnitValue.EUnit.Point);
        first.Margin.Top.Value.Value.Should().Be(0);
        first.Margin.Top.Value.Unit.Should().Be(UnitValue.EUnit.Point);
        first.Margin.Bottom.Value.Value.Should().Be(0);
        first.Margin.Bottom.Value.Unit.Should().Be(0);
        first.Margin.Height.Should().BeNull();
        first.Margin.Width.Should().BeNull();
    }

    //[Test]
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