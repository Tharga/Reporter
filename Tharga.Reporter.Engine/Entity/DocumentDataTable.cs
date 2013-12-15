using System.Collections.Generic;

namespace Tharga.Reporter.Engine.Entity
{
    public class DocumentDataTable
    {
        private readonly string _tableName;
        private readonly List<Dictionary<string, string>> _data = new List<Dictionary<string, string>>();

        public string TableName { get { return _tableName; } }
        public List<Dictionary<string, string>> Rows { get { return _data; } }

        public DocumentDataTable(string tableName)
        {
            _tableName = tableName;
        }

        public void AddRow(Dictionary<string, string> row)
        {
            _data.Add(row);
        }
    }
}