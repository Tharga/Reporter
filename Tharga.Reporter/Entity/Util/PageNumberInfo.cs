using System.Globalization;

namespace Tharga.Reporter.Entity.Util;

public class PageNumberInfo
{
    public PageNumberInfo(int pageNumber, int? totalPages)
    {
        PageNumber = pageNumber;
        TotalPages = totalPages;
    }

    public int PageNumber { get; }

    public int? TotalPages { get; }

    //TODO: Write tests for this
    public string GetPageNumberInfo(string dataName)
    {
        switch (dataName)
        {
            case "PageNumber":
                return PageNumber.ToString(CultureInfo.CurrentCulture);
            case "TotalPages":
                return TotalPages == null ? "N/A" : TotalPages.Value.ToString(CultureInfo.CurrentCulture);
            default:
                return null;
        }
    }
}