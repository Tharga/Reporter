using System.Collections;

namespace Tharga.Reporter.Entity;

public class SectionList : IEnumerable<Section>
{
    private readonly List<Section> _sections = new();

    internal SectionList()
    {
    }

    public int Count => _sections.Count;

    public IEnumerator<Section> GetEnumerator()
    {
        return _sections.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(Section section)
    {
        _sections.Add(section);
    }
}