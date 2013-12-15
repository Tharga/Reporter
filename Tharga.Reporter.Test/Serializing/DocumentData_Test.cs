using System.Collections.Generic;
using System.Linq;
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
            var row = new Dictionary<string, string>();
            row.Add("Row1A", "RowData1A");
            row.Add("Row1B", "RowData1B");
            row.Add("Row1C", "RowData1C");
            t.AddRow(row);

            var row2 = new Dictionary<string, string>();
            row2.Add("Row2A", "RowData2A");
            row2.Add("Row2B", "RowData2A");
            row2.Add("Row2C", "RowData2A");
            t.AddRow(row2);
            
            documentData.Add(t);
            var xml = documentData.ToXml();

            //Act
            var other = DocumentData.Load(xml);

            //Assert
            Assert.AreEqual(documentData.Get("A"), other.Get("A"));
            Assert.AreEqual(documentData.GetDataTable("TableA").Rows.Count, other.GetDataTable("TableA").Rows.Count);
            Assert.AreEqual(documentData.GetDataTable("TableA").Rows[0].First().Key, other.GetDataTable("TableA").Rows[0].First().Key);
            Assert.AreEqual(documentData.GetDataTable("TableA").Rows[0].First().Value, other.GetDataTable("TableA").Rows[0].First().Value);
            Assert.AreEqual(documentData.GetDataTable("TableA").Rows[1].First().Key, other.GetDataTable("TableA").Rows[1].First().Key);
            Assert.AreEqual(documentData.GetDataTable("TableA").Rows[1].First().Value, other.GetDataTable("TableA").Rows[1].First().Value);
            Assert.AreEqual(xml.OuterXml, other.ToXml().OuterXml);
        }
    }
}
