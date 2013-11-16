using System.Collections.Generic;

namespace Tharga.Reporter.Engine.Entity
{
    public class DocumentDataTable
    {
        private List<Dictionary<string, string>> _data = new List<Dictionary<string, string>>();

        public string TableName { get; private set; }
        public List<Dictionary<string,string>> Rows
        {
            get { return _data; }
        }

        internal DocumentDataTable(string tableName)
        {
            TableName = tableName;
        }

        public Dictionary<string, string> AddRow()
        {
            var row = new Dictionary<string, string>();
            _data.Add(row);
            return row;
        }
    }
}