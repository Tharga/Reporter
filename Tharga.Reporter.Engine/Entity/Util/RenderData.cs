using PdfSharp.Drawing;
using Tharga.Reporter.Engine.Entity.Area;
using Tharga.Reporter.Engine.Interface;

namespace Tharga.Reporter.Engine.Entity.Util
{
    class RenderData : IRenderData
    {
        private readonly IGraphics _graphics;
        private readonly DocumentData _documentData;
        private readonly bool _includeBackground;
        private readonly IDebugData _debugData;

        public RenderData(IGraphics graphics, XRect parentBounds, Section section, DocumentData documentData, PageNumberInfo pageNumberInfo, IDebugData debugData, bool includeBackground)
        {
            //_gfx = gfx;
            ParentBounds = parentBounds;
            Section = section;
            _graphics = graphics;
            _documentData = documentData;
            PageNumberInfo = pageNumberInfo;
            _debugData = debugData;
            _includeBackground = includeBackground;
        }

        public XRect ParentBounds { get; private set; }
        public XRect ElementBounds { get; set; }
        public bool IncludeBackground { get { return _includeBackground; } }
        public IGraphics Graphics { get { return _graphics; } }
        public Section Section { get; private set; }
        public DocumentData DocumentData { get { return _documentData; } }
        public PageNumberInfo PageNumberInfo { get; private set; }
        public IDebugData DebugData { get { return _debugData; } }
    }
}