using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Tharga.Reporter.Engine.Interface;

namespace Tharga.Reporter.Engine.Entity.Area
{
    public class ReferencePoint : Element.Element, IElementContainer
    {
        public enum Stack { None, Vertical }
        
        private List<Element.Element> _elementList = new List<Element.Element>();
        private readonly Stack _stack;

        public List<Element.Element> ElementList { get { return _elementList; } set { _elementList = value; } }

        public ReferencePoint()
        {

        }

        public ReferencePoint(string left, string top = null, Stack stack = Stack.None)
            : base(new UnitRectangle(left != null ? UnitValue.Parse(left) : null, 
                top != null ? UnitValue.Parse(top) : null, null, null))
        {
            _stack = stack;
        }

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
            out XRect elementBounds, bool  background, bool debug)
        {
            var bounds = GetBounds(parentBounds);

            RenderChildren(page, documentData, bounds, background, debug);

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
                if (_stack == Stack.Vertical && element.Top == null)
                    element.Top = stackTop;

                XRect elmBnd;
                element.Render(page, bounds, documentData, out elmBnd, background, debug);

                stackTop.Value += elmBnd.Height;
            }
        }

        protected internal override XmlElement AppendXml(ref XmlElement xmePane)
        {
            throw new NotImplementedException();
        }
    }
}