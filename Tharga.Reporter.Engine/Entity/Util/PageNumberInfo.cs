using System.Globalization;

namespace Tharga.Reporter.Engine.Entity.Area
{
    public class PageNumberInfo
    {
        public readonly int PageNumber;
        public static int? TotalPages;

        public PageNumberInfo(int pageNumber)
        {
            PageNumber = pageNumber;
        }

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
}