using System.Collections;

namespace Tharga.Reporter.Entity
{
    public class SectionList : IEnumerable<Section>
    {
        private readonly List<Section> _sections = new List<Section>();

        internal SectionList()
        {
            
        }

        public int Count { get { return _sections.Count; } }

        public void Add(Section section)
        {
            _sections.Add(section);
        }

        public IEnumerator<Section> GetEnumerator()
        {
            return _sections.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}