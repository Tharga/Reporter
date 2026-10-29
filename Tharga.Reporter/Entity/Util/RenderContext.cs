namespace Tharga.Reporter.Entity.Util;

/// <summary>
/// State worked out while rendering one document: the page counts and offsets of the sections and how the
/// multi-page elements were split into pages. It belongs to a single <see cref="Renderer"/>, so several renders of
/// one template, also at the same time, never see each other's results.
/// </summary>
internal sealed class RenderContext
{
    private const int DefaultPageCount = 1;

    private readonly Dictionary<Section, int> _sectionPageCounts = new();
    private readonly Dictionary<Section, int> _sectionPageOffsets = new();
    private readonly Dictionary<Element.Base.Element, object> _elementPages = new();

    internal int GetPageCount(Section section)
    {
        return _sectionPageCounts.GetValueOrDefault(section, DefaultPageCount);
    }

    internal void SetPageCount(Section section, int pageCount)
    {
        _sectionPageCounts[section] = pageCount;
    }

    internal int GetPageOffset(Section section)
    {
        return _sectionPageOffsets.GetValueOrDefault(section);
    }

    internal void SetPageOffset(Section section, int pageOffset)
    {
        _sectionPageOffsets[section] = pageOffset;
    }

    internal List<T> GetPages<T>(Element.Base.Element element)
    {
        return _elementPages.TryGetValue(element, out var pages) ? (List<T>)pages : null;
    }

    internal void SetPages<T>(Element.Base.Element element, List<T> pages)
    {
        _elementPages[element] = pages;
    }
}
