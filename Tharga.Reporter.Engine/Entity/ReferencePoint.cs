using System;
using System.Drawing;
using System.Linq;
using System.Xml;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Tharga.Reporter.Engine.Entity.Element;

namespace Tharga.Reporter.Engine.Entity.Area
{
    public class ReferencePoint : MultiPageElement
    {
        private const StackMethod _defaultStack = StackMethod.None;

        public enum StackMethod { None, Vertical }

        private ElementList _elementList;
        private StackMethod? _stack;

        public StackMethod Stack { get { return _stack ?? _defaultStack; } set { _stack = value; } }
        public ElementList ElementList { get { return _elementList ?? (_elementList = new ElementList()); } set { _elementList = value; } } 

        //protected internal override void ClearRenderPointer()
        //{
        //    foreach (var element in _elementList.Where(x => x is MultiPageAreaElement))
        //        ((MultiPageAreaElement)element).ClearRenderPointer();
        //}

        internal override bool Render(PdfPage page, XRect parentBounds, DocumentData documentData,
            out XRect elementBounds, bool includeBackground, bool debug, PageNumberInfo pageNumberInfo, Section section)
        {
            var bounds = GetBounds(parentBounds);

            var needMorePages = RenderChildren(page, documentData, bounds, includeBackground, debug, pageNumberInfo,section);

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

        //TODO: Make sure there is no output here
        internal override int PreRender(IRenderData renderData)
        {
            var bounds = GetBounds(renderData.ParentBounds);

            var rd = new RenderData(renderData.Gfx, bounds, renderData.Section, renderData.DocumentData, renderData.PageNumberInfo, renderData.Debug);
            var pageCount = PreRenderChildren(rd);

            //TODO: Change the width and height to the actual area used
            renderData.ElementBounds = new XRect(bounds.Left, bounds.Right, 0, 0);

            return pageCount;
        }

        internal override void Render(IRenderData renderData, int page)
        {
            var bounds = GetBounds(renderData.ParentBounds);
            
            var rd = new RenderData(renderData.Gfx, bounds, renderData.Section, renderData.DocumentData, renderData.PageNumberInfo, renderData.Debug);
            RenderChildren(rd, page);

            if (renderData.Debug)
            {
                var pen = new XPen(XColor.FromArgb(Color.Blue), 0.1);
                const int radius = 10;
                renderData.Gfx.DrawEllipse(pen, bounds.Left - radius, bounds.Top - radius, radius * 2, radius * 2);
                renderData.Gfx.DrawLine(pen, bounds.Left - radius - 2, bounds.Top, bounds.Left + radius + 2, bounds.Top);
                renderData.Gfx.DrawLine(pen, bounds.Left, bounds.Top - radius - 2, bounds.Left, bounds.Top + radius + 2);
            }

            //TODO: Change the width and height to the actual area used
            renderData.ElementBounds = new XRect(bounds.Left, bounds.Right, 0, 0);
        }

        private void RenderChildren(IRenderData renderData, int page)
        {
            var stackTop = new UnitValue();
            foreach (var element in _elementList)
            {
                var resetLocation = false;
                if (Stack == StackMethod.Vertical)
                {
                    if (element.Top == null)
                    {
                        resetLocation = true;
                        element.Top = stackTop;
                    }
                }

                if (element is SinglePageAreaElement)
                    ((SinglePageAreaElement)element).Render(renderData);
                else if (element is MultiPageAreaElement)
                    ((MultiPageAreaElement)element).Render(renderData, page);
                else if (element is MultiPageElement)
                    ((MultiPageElement)element).Render(renderData, page);
                else
                    throw new ArgumentOutOfRangeException(string.Format("Unknown type {0}.", element.GetType()));

                stackTop = new UnitValue(stackTop.Value + renderData.ElementBounds.Height, stackTop.Unit);

                if (resetLocation)
                    element.Top = null;
            }
        }

        private int PreRenderChildren(IRenderData renderData)
        {
            var maxPageCount = 1;
            var elementsToRender = _elementList.Where(x => x is MultiPageAreaElement || x is MultiPageElement);

            var stackTop = new UnitValue();
            foreach (var element in elementsToRender)
            {
                var resetLocation = false;
                if (Stack == StackMethod.Vertical)
                {
                    if (element.Top == null)
                    {
                        resetLocation = true;
                        element.Top = stackTop;
                    }
                }

                int pageCount;
                if (element is MultiPageAreaElement)
                    pageCount = ((MultiPageAreaElement) element).PreRender(renderData);
                else if (element is MultiPageElement)
                    pageCount = ((MultiPageElement) element).PreRender(renderData);
                else
                    throw new ArgumentOutOfRangeException(string.Format("Unknown type {0}.", element.GetType()));

                stackTop = new UnitValue(stackTop.Value + renderData.ElementBounds.Height, stackTop.Unit);

                if (resetLocation)
                    element.Top = null;

                if (pageCount > maxPageCount)
                    maxPageCount = pageCount;
            }
            return maxPageCount;
        }

        private bool RenderChildren(PdfPage page, DocumentData documentData, XRect bounds, bool background, bool debug, PageNumberInfo pageNumberInfo, Section section)
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
                    ((SinglePageAreaElement)element).Render(page, bounds, documentData, out elmBnd, background, debug, pageNumberInfo, section);
                else if (element is MultiPageAreaElement)
                {
                    if (((MultiPageAreaElement) element).Render(page, bounds, documentData, out elmBnd, background, debug, pageNumberInfo,section))
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

        //public override int PageCount(IRenderData renderData)
        //{
        //    var multi = ElementList.Where(x => x is MultiPageAreaElement || x is MultiPageElement);
        //    return multi.Max(x => x.PageCount(renderData));
        //}
    }
}