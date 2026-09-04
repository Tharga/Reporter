using System.Collections.Concurrent;
using PdfSharp.Fonts;
using PdfSharp.WPFonts;

namespace Tharga.Reporter.Fonts;

/// <summary>
/// Resolves fonts without any platform graphics library, so rendering works on Windows, Linux and macOS alike.
/// </summary>
/// <remarks>
/// Resolution order is: fonts registered through <see cref="Register"/>, then fonts installed on the host,
/// then an embedded fallback face. The fallback means a host with no fonts installed — a bare container, for
/// instance — still renders text instead of throwing.
/// </remarks>
public class ReporterFontResolver : IFontResolver
{
    private const string FallbackFaceName = "$fallback";
    private const string FallbackBoldFaceName = "$fallback-bold";

    private static readonly ConcurrentDictionary<string, byte[]> RegisteredFonts = new(StringComparer.OrdinalIgnoreCase);
    private static readonly ConcurrentDictionary<string, byte[]> FontDataCache = new(StringComparer.Ordinal);

    /// <summary>
    /// Registers a font face, taking precedence over any font installed on the host.
    /// </summary>
    /// <param name="familyName">The family name templates refer to, for example "Verdana".</param>
    /// <param name="isBold">Whether the supplied face is bold.</param>
    /// <param name="isItalic">Whether the supplied face is italic.</param>
    /// <param name="fontData">The raw TrueType font file.</param>
    public static void Register(string familyName, bool isBold, bool isItalic, byte[] fontData)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(familyName);
        ArgumentNullException.ThrowIfNull(fontData);

        RegisteredFonts[BuildFaceName(familyName, isBold, isItalic)] = fontData;
    }

    /// <summary>
    /// Installs this resolver unless the application has already assigned one of its own.
    /// </summary>
    public static void EnsureInstalled()
    {
        GlobalFontSettings.FontResolver ??= new ReporterFontResolver();
    }

    public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        var faceName = BuildFaceName(familyName, isBold, isItalic);

        if (RegisteredFonts.ContainsKey(faceName))
        {
            return new FontResolverInfo(faceName);
        }

        if (SystemFontIndex.TryGetFontFile(familyName, isBold, isItalic, out var path))
        {
            FontDataCache.GetOrAdd(faceName, _ => File.ReadAllBytes(path));
            return new FontResolverInfo(faceName);
        }

        return new FontResolverInfo(isBold ? FallbackBoldFaceName : FallbackFaceName, isBold, isItalic);
    }

    public byte[] GetFont(string faceName)
    {
        if (RegisteredFonts.TryGetValue(faceName, out var registered)) return registered;
        if (FontDataCache.TryGetValue(faceName, out var cached)) return cached;

        return faceName == FallbackBoldFaceName ? FontDataHelper.SegoeWPBold : FontDataHelper.SegoeWP;
    }

    private static string BuildFaceName(string familyName, bool isBold, bool isItalic)
    {
        return $"{(familyName ?? string.Empty).Trim().ToLowerInvariant()}|{(isBold ? 1 : 0)}|{(isItalic ? 1 : 0)}";
    }
}
