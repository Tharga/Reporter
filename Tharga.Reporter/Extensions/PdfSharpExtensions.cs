//using PdfSharp;

//namespace Tharga.Reporter.Extensions;

//internal static class PdfSharpExtensions
//{
//    public static PageSize ToPageSize(this Renderer.PageSize pageSize)
//    {
//        if (!Enum.TryParse(pageSize.ToString(), true, out PageSize result))
//        {
//            throw new InvalidOperationException($"Unable to parse page size {pageSize} to PdfSharp version of page size.");
//        }

//        return result;
//    }
//}