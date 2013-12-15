using NUnit.Framework;

namespace Tharga.Reporter.Tests.Serializing
{
    [TestFixture]
    class DocumentData
    {
        [Test]
        public void Default_Document()
        {
            //Arrange
            var documentData = new Engine.Entity.DocumentData();
            var xml = documentData.ToXml();

            //Act
            var other = Engine.Entity.DocumentData.Load(xml);

            //Assert
            Assert.AreEqual(documentData.Get("A"), other.Get("A"));
            Assert.AreEqual(documentData.GetDataTable("TableA").Rows.Count, other.GetDataTable("TableA").Rows.Count);
            Assert.AreEqual(xml, other.ToXml());
        }

        [Test]
        public void Document_with_data()
        {
            //Arrange
            var documentData = new Engine.Entity.DocumentData();
            documentData.Add("A","DataA");
            var xml = documentData.ToXml();

            //Act
            var other = Engine.Entity.DocumentData.Load(xml);

            //Assert
            Assert.AreEqual(documentData.Get("A"), other.Get("A"));
            Assert.AreEqual(documentData.GetDataTable("TableA").Rows.Count, other.GetDataTable("TableA").Rows.Count);
            Assert.AreEqual(xml, other.ToXml());
        }

        [Test]
        [Ignore]
        public void Document_with_table()
        {
            //Arrange
            var documentData = new Engine.Entity.DocumentData();
            var row = documentData.GetDataTable("TableA").AddRow();
            row.Add("RowA","RowDataA");
            var xml = documentData.ToXml();

            //Act
            var other = Engine.Entity.DocumentData.Load(xml);

            //Assert
            Assert.AreEqual(documentData.Get("A"), other.Get("A"));
            Assert.AreEqual(documentData.GetDataTable("TableA").Rows.Count, other.GetDataTable("TableA").Rows.Count);
            Assert.AreEqual(xml, other.ToXml());
        }
    }
}
