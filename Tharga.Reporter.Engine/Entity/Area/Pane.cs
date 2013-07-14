using System;
using System.Linq;
using System.Xml;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Tharga.Reporter.Engine.Entity.Element;
using Tharga.Reporter.Engine.Helper;
using Tharga.Reporter.Engine.Interface;
using Rectangle = Tharga.Reporter.Engine.Entity.Element.Rectangle;

namespace Tharga.Reporter.Engine.Entity.Area
{
    public class PageNumberInfo
    {
        public readonly int PageNumber;

        public PageNumberInfo(int pageNumber)
        {
            PageNumber = pageNumber;
        }
    }

    public class Pane : IElementContainer
    {
        protected const double HeightEpsilon = 0.01;

        private readonly ElementList _elementList = new ElementList();

        public ElementList ElementList { get { return _elementList; } }

        internal Pane()
        {

        }

        //internal Pane(XmlElement xmlPane)
        //{
        //    foreach(XmlElement xmlElement in xmlPane.ChildNodes)
        //    {
        //        switch(xmlElement.Name)
        //        {
        //            case "Text":
        //                ElementList.Add(new Text(xmlElement));
        //                break;
        //            case "Rectangle":
        //                ElementList.Add(new Rectangle(xmlElement));
        //                break;
        //            default:
        //                throw new ArgumentOutOfRangeException(string.Format("Cannot parse {0} as an element.", xmlElement.Name));
        //        }
        //    }
        //}

        internal void ClearRenderPointers()
        {
            foreach (var element in _elementList.Where(x => x is MultiPageElement))
                ((MultiPageElement)element).ClearRenderPointer();
        }

        internal bool Render(PdfPage page, XRect bounds, DocumentData documentData, bool background, bool debug, PageNumberInfo pageNumberInfo)
        {
            var needAnotherPage = false;
            foreach (var element in _elementList)
            {
                XRect bnd;
                if (element is MultiPageElement)
                {
                    var needMore = ((MultiPageElement)element).Render(page, bounds, documentData, out bnd, background, debug, pageNumberInfo);
                    if (needMore)
                        needAnotherPage = true;
                }
                else if (element is SinglePageElement)
                {
                    ((SinglePageElement)element).Render(page, bounds, documentData, out bnd, background, debug, pageNumberInfo);
                }
            }
            return needAnotherPage;
        }

        internal virtual XmlElement AppendXml(ref XmlElement xmeSection)
        {
            if (xmeSection == null) throw new ArgumentNullException("xmeSection");
            if (xmeSection.OwnerDocument == null) throw new ArgumentNullException("xmeSection", "xmeSection has no owner document.");

            var xmePane = xmeSection.OwnerDocument.CreateElement(GetType().ToShortTypeName());
            xmeSection.AppendChild(xmePane);

            foreach(var element in ElementList)
                element.AppendXml(ref xmePane);

            return xmePane;
        }
    }
}