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
        private const StackMethod _defaultStack = StackMethod.None;

        public enum StackMethod { None, Vertical }

        private ElementList _elementList; // = new ElementList();
        private StackMethod? _stack;

        public StackMethod Stack { get { return _stack ?? _defaultStack; } set { _stack = value; } }
        public ElementList ElementList { get { return _elementList ?? (_elementList = new ElementList()); } set { _elementList = value; } } 

        //TODO: Overriding theese is not a good idea. They should not even be here in the first place
        //public override UnitValue? Bottom
        //{
        //    get { throw new NotSupportedException(); }
        //    set { throw new NotSupportedException(); }
        //}

        //public override UnitValue? Height
        //{
        //    get { throw new NotSupportedException(); }
        //    set { throw new NotSupportedException(); }
        //}

        //public override UnitValue? Width
        //{
        //    get { throw new NotSupportedException(); }
        //    set { throw new NotSupportedException(); }
        //}

        //public override UnitValue? Right
        //{
        //    get { throw new NotSupportedException(); }
        //    set { throw new NotSupportedException(); }
        //}

        protected internal override void ClearRenderPointer()
        {
            foreach (var element in _elementList.Where(x => x is MultiPageAreaElement))
                ((MultiPageAreaElement)element).ClearRenderPointer();
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

            var stackTop = new UnitValue();
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
                    //    System.Diagnostics.Debug.WriteLine("AreaElement {0} has the top set to {1} on page {2}.", element.Name, element.Top.ToString(), pageNumberInfo.PageNumber);
                    //}
                }

                var elmBnd = new XRect();                
                if (element is SinglePageAreaElement)
                    ((SinglePageAreaElement)element).Render(page, bounds, documentData, out elmBnd, background, debug, pageNumberInfo);
                else if (element is MultiPageAreaElement)
                {
                    if (((MultiPageAreaElement) element).Render(page, bounds, documentData, out elmBnd, background, debug, pageNumberInfo))
                        needMorePages = true;
                }

                stackTop = new UnitValue(stackTop.Value + elmBnd.Height, stackTop.Unit);
                //stackTop.Value += elmBnd.Height;

                if (resetLocation)
                    element.Top = null;
            }
            return needMorePages;
        }

        protected override XRect GetBounds(XRect parentBounds)
        {
            //if (_relativeAlignment == null) throw new InvalidOperationException("No relative alignment for the Area.");
            //var relativeAlignment = _relativeAlignment;
            var relativeAlignment = new UnitRectangle(Left.Value, Top.Value, "0", "0");

            var left = parentBounds.X + relativeAlignment.GetLeft(parentBounds.Width);
            //var right = parentBounds.Right - relativeAlignment.GetRight(parentBounds.Width);
            var width = relativeAlignment.GetWidht(parentBounds.Width);

            var top = parentBounds.Y + relativeAlignment.GetTop(parentBounds.Height);
            //var bottom = parentBounds.Bottom - relativeAlignment.GetBottom(parentBounds.Height);
            var height = relativeAlignment.GetHeight(parentBounds.Height);

            if (height < 0)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Height is adjusted from {0} to 0.", height));
                height = 0;
            }

            return new XRect(left, top, width, height);
        }

        //TODO: Write tests for this
        internal override XmlElement ToXme()
        {
            var xme = base.ToXme();

            if (Left != null)
                xme.SetAttribute("Left", Left.Value.ToString());

            if (Top != null)
                xme.SetAttribute("Top", Top.Value.ToString());

            if (_stack != null)
                xme.SetAttribute("Stack", _stack.ToString());

            foreach (var element in ElementList)
            {
                var xmeElement = element.ToXme();
                var importedElement = xme.OwnerDocument.ImportNode(xmeElement, true);
                xme.AppendChild(importedElement);
            }

            return xme;
        }

        public static ReferencePoint Load(XmlElement xme)
        {
            var referencePoint = new ReferencePoint();
            referencePoint.AppendData(xme);

            referencePoint.Left = referencePoint.GetValue(xme, "Left");
            referencePoint.Top = referencePoint.GetValue(xme, "Top");

            var xmeStack = xme.Attributes["Stack"];
            if (xmeStack != null)
                referencePoint.Stack = (StackMethod)Enum.Parse(typeof(StackMethod), xmeStack.Value);

            //TODO: Add range
            var elements = Pane.GetElements(xme);
            foreach (var element in elements)
            {
                referencePoint.ElementList.Add(element);
            }

            return referencePoint;
        }
    }
}