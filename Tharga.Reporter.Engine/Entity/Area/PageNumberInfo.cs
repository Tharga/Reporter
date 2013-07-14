using System.Globalization;

namespace Tharga.Reporter.Engine.Entity.Area
{
    public class PageNumberInfo
    {
        private readonly int _pageNumber;
        public static int? TotalPages;

        public PageNumberInfo(int pageNumber)
        {
            _pageNumber = pageNumber;
        }

        public string GetPageNumberInfo(string dataName)
        {
            switch (dataName)
            {
                case "PageNumber":
                    return _pageNumber.ToString(CultureInfo.CurrentCulture);
                case "TotalPages":
                    return TotalPages == null ? "N/A" : TotalPages.Value.ToString(CultureInfo.CurrentCulture);
                default:
                    return null;
            }
        }
    }
}