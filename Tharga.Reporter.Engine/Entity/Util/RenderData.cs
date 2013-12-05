using PdfSharp.Drawing;
using Tharga.Reporter.Engine.Entity.Area;

namespace Tharga.Reporter.Engine.Entity.Element
{
    class RenderData : IRenderData
    {
        private readonly XGraphics _gfx;
        private readonly DocumentData _documentData;
        private readonly bool _debug;

        public RenderData(XGraphics gfx, XRect parentBounds, Section section, DocumentData documentData, PageNumberInfo pageNumberInfo, bool debug)
        {
            _gfx = gfx;
            ParentBounds = parentBounds;
            Section = section;
            _documentData = documentData;
            PageNumberInfo = pageNumberInfo;
            _debug = debug;
        }

        public XRect ParentBounds { get; private set; }
        public XRect ElementBounds { get; set; }
        public bool IncludeBackground { get; private set; }
        public XGraphics Gfx { get { return _gfx; } }
        public Section Section { get; private set; }
        public DocumentData DocumentData { get { return _documentData; } }
        public PageNumberInfo PageNumberInfo { get; private set; }
        public bool Debug { get { return _debug; } }
    }
}