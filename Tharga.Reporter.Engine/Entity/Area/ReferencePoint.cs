using System;
using System.Drawing;
using System.Linq;
using System.Xml;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Tharga.Reporter.Engine.Entity.Element;
using Tharga.Reporter.Engine.Interface;

namespace Tharga.Reporter.Engine.Entity.Area
{
    public class ReferencePoint : MultiPageElement, IElementContainer
    {
        public enum StackMethod { None, Vertical }
        
        private readonly ElementList _elementList = new ElementList();

        public StackMethod Stack { get; set; }
        public ElementList ElementList { get { return _elementList; } } 

        public ReferencePoint()
        {

        }

        //[Obsolete("Use default constructor and property setters instead.")]
        //public ReferencePoint(string left, string top = null, StackMethod stack = StackMethod.None)
        //    : base(new UnitRectangle(left != null ? UnitValue.Parse(left) : null, top != null ? UnitValue.Parse(top) : null, null, null))
        //{
        //    Stack = stack;
        //}

        public override UnitValue Bottom
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override UnitValue Height
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override UnitValue Width
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override UnitValue Right
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        protected internal override void ClearRenderPointer()
        {
            foreach (var element in _elementList.Where(x => x is MultiPageElement))
                ((MultiPageElement)element).ClearRenderPointer();
        }

        protected internal override bool Render(PdfPage page, XRect parentBounds, DocumentData documentData,
            out XRect elementBounds, bool includeBackground, bool debug, PageNumberInfo pageNumberInfo)
        {
            var bounds = GetBounds(parentBounds);

            var needMorePages = RenderChildren(page, documentData, bounds, includeBackground, debug, pageNumberInfo);

            if (debug)
            {
                using (var gfx = XGraphics.FromPdfPage(page))
                {
                    var pen = new XPen(XColor.FromArgb(Color.Blue), 0.1);
                    const int radius = 10;
                    gfx.DrawEllipse(pen, bounds.Left - radius, bounds.Top - radius, radius*2, radius*2);
                    gfx.DrawLine(pen, bounds.Left - radius - 2, bounds.Top, bounds.Left + radius + 2, bounds.Top);
                    gfx.DrawLine(pen, bounds.Left, bounds.Top - radius - 2, bounds.Left, bounds.Top + radius + 2);
                }
            }

            //TODO: Change the width and height to the actual area used
            elementBounds = new XRect(bounds.Left, bounds.Right, 0, 0);

            return needMorePages;
        }

        private bool RenderChildren(PdfPage page, DocumentData documentData, XRect bounds, bool background, bool debug, PageNumberInfo pageNumberInfo)
        {
            var needMorePages = false;

            var stackTop = UnitValue.Create();
            foreach (var element in _elementList)
            {
                //When using vertical stacking it should not be allowed to set the top value. Or can it be set as a offset?

                var resetLocation = false;
                if (Stack == StackMethod.Vertical)
                {
                    if (element.Top == null)
                    {
                        resetLocation = true;
                        element.Top = stackTop;
                    }
                    //else
                    //{
                    //    System.Diagnostics.Debug.WriteLine("Element {0} has the top set to {1} on page {2}.", element.Name, element.Top.ToString(), pageNumberInfo.PageNumber);
                    //}
                }

                var elmBnd = new XRect();                
                if (element is SinglePageElement)
                    ((SinglePageElement)element).Render(page, bounds, documentData, out elmBnd, background, debug, pageNumberInfo);
                else if (element is MultiPageElement)
                {
                    if (((MultiPageElement) element).Render(page, bounds, documentData, out elmBnd, background, debug, pageNumberInfo))
                        needMorePages = true;
                }

                stackTop.Value += elmBnd.Height;

                if (resetLocation)
                    element.Top = null;
            }
            return needMorePages;
        }

        protected internal override XmlElement AppendXml(ref XmlElement xmePane)
        {
            throw new NotImplementedException();
        }
    }
}