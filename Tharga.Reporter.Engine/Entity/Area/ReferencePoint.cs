using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Tharga.Reporter.Engine.Entity.Element;
using Tharga.Reporter.Engine.Interface;

namespace Tharga.Reporter.Engine.Entity.Area
{
    public class ReferencePoint : SinglePageElement, IElementContainer
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

        protected internal override void Render(PdfPage page, XRect parentBounds, DocumentData documentData, 
            out XRect elementBounds, bool includeBackground, bool debug)
        {
            var bounds = GetBounds(parentBounds);

            RenderChildren(page, documentData, bounds, includeBackground, debug);

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
        }

        private void RenderChildren(PdfPage page, DocumentData documentData, XRect bounds, bool background, bool debug)
        {
            var stackTop = UnitValue.Create();
            foreach (var element in _elementList)
            {
                if (Stack == StackMethod.Vertical && element.Top == null)
                    element.Top = stackTop;

                XRect elmBnd;
                
                if ( element is MultiPageElement)
                    throw new NotImplementedException("Rendering multi page elements for reference points have not yet been implemented.");

                ((SinglePageElement)element).Render(page, bounds, documentData, out elmBnd, background, debug);

                stackTop.Value += elmBnd.Height;
            }
        }

        protected internal override XmlElement AppendXml(ref XmlElement xmePane)
        {
            throw new NotImplementedException();
        }
    }
}