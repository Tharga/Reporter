using NUnit.Framework;
using Tharga.Reporter.Engine.Entity;

namespace Tharga.Reporter.Tests.Serializing
{
    [TestFixture]
    class DocumentData_Test
    {
        [Test]
        public void Default_Document()
        {
            //Arrange
            var documentData = new DocumentData();
            var xml = documentData.ToXml();

            //Act
            var other = DocumentData.Load(xml);

            //Assert
            Assert.AreEqual(documentData.Get("A"), other.Get("A"));
            Assert.AreEqual(documentData.GetDataTable("TableA"), other.GetDataTable("TableA"));
            Assert.AreEqual(xml.OuterXml, other.ToXml().OuterXml);
        }

        [Test]
        public void Document_with_data()
        {
            //Arrange
            var documentData = new DocumentData();
            documentData.Add("A","DataA");
            var xml = documentData.ToXml();

            //Act
            var other = DocumentData.Load(xml);

            //Assert
            Assert.AreEqual(documentData.Get("A"), other.Get("A"));            
            Assert.AreEqual(documentData.GetDataTable("TableA"), other.GetDataTable("TableA"));
            Assert.AreEqual(xml.OuterXml, other.ToXml().OuterXml);
        }

        [Test]
        public void Document_with_table()
        {
            //Arrange
            var documentData = new DocumentData();
            var t = new DocumentDataTable("TableA");
            t.AddRow("RowA", "RowDataA");
            documentData.Add(t);
            var xml = documentData.ToXml();

            //Act
            var other = DocumentData.Load(xml);

            //Assert
            Assert.AreEqual(documentData.Get("A"), other.Get("A"));
            Assert.AreEqual(documentData.GetDataTable("TableA").Rows.Count, other.GetDataTable("TableA").Rows.Count);
            Assert.AreEqual(xml.OuterXml, other.ToXml().OuterXml);
        }
    }
}
