using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tharga.Reporter.Engine;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Element;

namespace Tharga.Reporter.Test
{
    [TestClass]
    public class Engine
    {
        [TestMethod]
        public void Create_template_with_default_section()
        {
            //Arrange
            var section = Section.Create();

            //Act
            var template = Template.Create(section);

            //Assert
            Assert.AreEqual(1, template.SectionList.Count);

            Assert.IsNotNull(template.SectionList[0].Header);
            Assert.AreEqual(0, template.SectionList[0].Header.ElementList.Count);
            Assert.AreEqual(0, template.SectionList[0].Header.Height.Value);
            Assert.AreEqual(UnitValue.EUnit.Point, template.SectionList[0].Header.Height.Unit);

            Assert.IsNotNull(template.SectionList[0].Footer);
            Assert.AreEqual(0, template.SectionList[0].Footer.ElementList.Count);
            Assert.AreEqual(0, template.SectionList[0].Footer.Height.Value);
            Assert.AreEqual(UnitValue.EUnit.Point, template.SectionList[0].Footer.Height.Unit);

            Assert.IsNotNull(template.SectionList[0].Pane);
            Assert.AreEqual(0, template.SectionList[0].Pane.ElementList.Count);

            Assert.IsNotNull(template.SectionList[0].Margin);
            Assert.AreEqual(0, template.SectionList[0].Margin.Left.Value);
            Assert.AreEqual(UnitValue.EUnit.Point, template.SectionList[0].Margin.Left.Unit);
            Assert.AreEqual(0, template.SectionList[0].Margin.Right.Value);
            Assert.AreEqual(UnitValue.EUnit.Point, template.SectionList[0].Margin.Right.Unit);
            Assert.AreEqual(0, template.SectionList[0].Margin.Top.Value);
            Assert.AreEqual(UnitValue.EUnit.Point, template.SectionList[0].Margin.Top.Unit);
            Assert.AreEqual(0, template.SectionList[0].Margin.Bottom.Value);
            Assert.AreEqual(UnitValue.EUnit.Point, template.SectionList[0].Margin.Bottom.Unit);
            Assert.IsNull(template.SectionList[0].Margin.Height);
            Assert.IsNull(template.SectionList[0].Margin.Width);
        }

        [TestMethod]
        public void Create_pdf_document()
        {
            //Arrange
            var template = Template.Create(Section.Create());

            //Act
            var byteArray = Rendering.CreatePDFDocument(template);

            //Assert
            Assert.IsTrue(byteArray.Length > 0);
        }

        [TestMethod]
        public void Serialize_to_xml()
        {
            //Arrange
            //var section = Section.Create(); //TODO: Create a section builder that randomizes the construction of a document.
            //var text = Text.Create("Some text");
            //text.Font.FontName = "Times New Roman";
            //section.Header.ElementList.Add(text);
            //var template = Template.Create(section);

            ////Act
            //var xml = template.ToXml();
            //var otherTemplate = Template.Create(xml);
            //var otherXml = otherTemplate.ToXml();

            ////Assert
            //Assert.AreEqual(xml.InnerXml, otherXml.InnerXml);
            ////TODO: Also assert that the properties of the objects are equal
        }
    }
}
