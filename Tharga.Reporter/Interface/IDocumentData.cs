using Tharga.Reporter.Entity;

namespace Tharga.Reporter.Interface;

public interface IDocumentData
{
    string Get(string dataName);
    DocumentDataTable GetDataTable(string name);
}