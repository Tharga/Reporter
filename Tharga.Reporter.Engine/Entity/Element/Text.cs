using System.Drawing;
using System.Xml;
using Tharga.Reporter.Engine.Helper;

namespace Tharga.Reporter.Engine.Entity.Element
{
    public class Text : TextBase
    {
        public string Value { get; set; }
        public string HideValue { get; set; }

        #region Constructors



        internal Text(string value)
        {
            Value = value;
        }

        internal Text(XmlElement xmlElement)
            :base(xmlElement)
        {
            Value = xmlElement.Attributes.GetNamedItem("Value").Value;
        }

        internal Text(string value, string fontClass)
            : base(fontClass)
        {
            Value = value;
        }

        internal Text(string value, string fontClass, UnitRectangle relativeAlignment)
            : base(fontClass, relativeAlignment)
        {
            Value = value;
        }


        #endregion
        #region Factory
        

        public static Text Create(string value, string left = null, string top = null, string width = null,
            Alignment textAlignment = Alignment.Left, double fontSize = 10, string hideValue = null)
        {
            var text = new Text(value)
                           {
                               Left = left != null ? UnitValue.Parse(left) : null,
                               Top = top != null ? UnitValue.Parse(top) : null,
                               Width = width != null ? UnitValue.Parse(width) : null,
                           };
            text.TextAlignment = textAlignment;
            text.Font.Size = fontSize;
            text.HideValue = hideValue;
            return text;
        }

        public static Text Create(string value, Color fontColor, double fontSize, string left = null)
        {
            var text = new Text(value)
                           {
                               Left = left != null ? UnitValue.Parse(left) : null,
                           };
            text.Font.Color = fontColor;
            text.Font.Size = fontSize;
            return text;
        }


        #endregion

        protected override string GetValue(DocumentData documentData)
        {
            if (!string.IsNullOrEmpty(HideValue))
            {
                var result = documentData.Get(HideValue);
                if (string.IsNullOrEmpty(result))
                    return string.Empty;
            }

            return Value.ParseValue(documentData);
        }

        protected internal override XmlElement AppendXml(ref XmlElement xmePane)
        {
            var xmeElement = base.AppendXml(ref xmePane);

            xmeElement.SetAttribute("Value", Value);

            return xmeElement;
        }
    }
}