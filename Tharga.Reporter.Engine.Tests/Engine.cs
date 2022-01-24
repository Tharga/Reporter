using System;
using System.Linq;
using FluentAssertions;
using Tharga.Reporter.Engine;
using Tharga.Reporter.Engine.Entity;
using Xunit;

namespace Tharga.Reporter.Test
{
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

            //Assert.IsNotNull(template.SectionList.First().Header);
            template.SectionList.First().Header.ElementList.Count.Should().Be(0);
            //template.SectionList.First().Header.Height.Should().Be(0);
            UnitValue.EUnit.Point.Should().Be(template.SectionList.First().Header.Height.Unit);

            //Assert.IsNotNull(template.SectionList.First().Footer);
            //0.Should().Be(template.SectionList.First().Footer.ElementList.Count);
            //0.Should().Be(template.SectionList.First().Footer.Height.Value);
            UnitValue.EUnit.Point.Should().Be(template.SectionList.First().Footer.Height.Unit);

            //Assert.IsNotNull(template.SectionList.First().Pane);
            //0.Should().Be(template.SectionList.First().Pane.ElementList.Count);

            //Assert.IsNotNull(template.SectionList.First().Margin);
            //0.Should().Be(template.SectionList.First().Margin.Left.Value.Value);
            //UnitValue.EUnit.Point, template.SectionList.First().Margin.Left.Value.Unit);
            //0.Should().Be(template.SectionList.First().Margin.Right.Value.Value);
            //UnitValue.EUnit.Point, template.SectionList.First().Margin.Right.Value.Unit);
            //0.Should().Be(template.SectionList.First().Margin.Top.Value.Value);
            //UnitValue.EUnit.Point, template.SectionList.First().Margin.Top.Value.Unit);
            //0.Should().Be(template.SectionList.First().Margin.Bottom.Value.Value);
            //UnitValue.EUnit.Point, template.SectionList.First().Margin.Bottom.Value.Unit);
            //Assert.IsNull(template.SectionList.First().Margin.Height);
            //Assert.IsNull(template.SectionList.First().Margin.Width);
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
}