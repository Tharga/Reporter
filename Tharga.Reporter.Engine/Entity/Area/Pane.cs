using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Tharga.Reporter.Engine.Entity.Element;
using Tharga.Reporter.Engine.Interface;

namespace Tharga.Reporter.Engine.Entity.Area
{
    public class Pane : IElementContainer
    {
        //protected const double HeightEpsilon = 0.01;

        private readonly ElementList _elementList = new ElementList();

        public ElementList ElementList { get { return _elementList; } }

        internal Pane()
        {

        }

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

        public virtual XmlElement ToXme()
        {
            var xmd = new XmlDocument();
            var header = xmd.CreateElement("Pane");

            var elms = GetElements(xmd);
            foreach (var elm in elms)
                header.AppendChild(elm);

            return header;
        }

        protected IEnumerable<XmlNode> GetElements(XmlDocument xmd)
        {
            foreach (var element in ElementList)
            {
                var xmeElement = element.ToXme();
                var importedElement = xmd.ImportNode(xmeElement, true);
                yield return importedElement;
            }
        }

        //internal virtual XmlElement AppendXml(ref XmlElement xmeSection)
        //{
        //    if (xmeSection == null) throw new ArgumentNullException("xmeSection");
        //    if (xmeSection.OwnerDocument == null) throw new ArgumentNullException("xmeSection", "xmeSection has no owner document.");

        //    var xmePane = xmeSection.OwnerDocument.CreateElement(GetType().ToShortTypeName());
        //    xmeSection.AppendChild(xmePane);

        //    foreach(var element in ElementList)
        //        element.AppendXml(ref xmePane);

        //    return xmePane;
        //}

        public static Pane Load(XmlElement xme)
        {
            var pane = new Pane();

            var elms = pane.GetElements(xme);
            pane.ElementList.AddRange(elms);
            //foreach(XmlElement xmlElement in xme.ChildNodes)
            //{
            //    Element.Element element;
            //    switch (xmlElement.Name)
            //    {
            //        case "Line":
            //            element = Line.Load(xmlElement);                        
            //            break;
            //        default:
            //            throw new ArgumentOutOfRangeException(string.Format("Cannot parse element {0} as a subelement of pane.", xmlElement.Name));
            //    }
            //    //pane.ElementList.Add(element);
            //}

            return pane;
        }

        public IEnumerable<Element.Element> GetElements(XmlElement xme)
        {
            foreach (XmlElement xmlElement in xme.ChildNodes)
            {
                Element.Element element;
                switch (xmlElement.Name)
                {
                    case "Line":
                        element = Line.Load(xmlElement);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(string.Format("Cannot parse element {0} as a subelement of pane.", xmlElement.Name));
                }
                //pane.ElementList.Add(element);
                yield return element;
            }
        }
    }
}