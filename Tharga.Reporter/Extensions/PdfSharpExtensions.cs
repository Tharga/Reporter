using PdfSharp;

namespace Tharga.Reporter.Extensions;

internal static class PdfSharpExtensions
{
    public static PageSize ToPageSize(this Renderer.PageSize pageSize)
    {
        PageSize result;
        if (!Enum.TryParse(pageSize.ToString(), true, out result))
        {
            throw new InvalidOperationException(string.Format("Unable to parse page size {0} to PdfSharp version of page size."));
        }

        return result;
    }
}