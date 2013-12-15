using System;
using System.Collections.Generic;
using System.Xml;

namespace Tharga.Reporter.Engine.Entity
{
    public class DocumentData
    {
        private readonly Dictionary<string, string> _data = new Dictionary<string, string>();
        private readonly List<DocumentDataTable> _dataTable = new List<DocumentDataTable>();

        public void Add(string key, string value)
        {
            _data.Add(key, value);
        }

        internal string Get(string key)
        {
            return !_data.ContainsKey(key) ? null : _data[key];
        }

        public DocumentDataTable GetDataTable(string tableName)
        {
            var table = _dataTable.Find(itm => itm.TableName == tableName);
            if (table == null)
            {
                table = new DocumentDataTable(tableName);
                _dataTable.Add(table);
            }
            return table;

            //0-n tables per document
            //0-n rows
            //0-1 columns

            //var tbl = new DataTable("Table Name");
            //var dr = tbl.AddRow();
            //dr.AddData("ColumnName", "value");
            //dr.AddData("ColumnName", "value");
        }

        public XmlDocument ToXml()
        {
            var xmd = new XmlDocument();
            var xmeList = xmd.CreateElement("Data");
            xmd.AppendChild(xmeList);

            foreach (var item in _data)
            {
                var xme = xmd.CreateElement(item.Key);
                xme.InnerText = item.Value;
                xmeList.AppendChild(xme);
            }

            return xmd;
        }

        public static DocumentData Load(XmlDocument xmd)
        {
            var documentData = new DocumentData();

            var nodes = xmd.SelectNodes("Data");
            foreach (XmlNode node in nodes[0])
            {
                documentData.Add(node.Name, node.InnerText);
            }

            return documentData;
        }
    }
}