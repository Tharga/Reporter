using System.Xml;
using PdfSharp.Drawing;
using Tharga.Reporter.Engine.Helper;

namespace Tharga.Reporter.Engine.Entity.Element
{
    //TODO: Rename to AreaElement
    public abstract class Element
    {
        private UnitValue? _left;
        private UnitValue? _top;
        private string _name;
        private bool? _isBackground;

        public virtual UnitValue? Top { get { return _top; } set { _top = value; } }
        public virtual UnitValue? Left { get { return _left; } set { _left = value; } }
        public virtual string Name { get { return _name ?? string.Empty; } set { _name = value; } }
        public virtual bool IsBackground { get { return _isBackground ?? false; } set { _isBackground = value; } }

        internal virtual XmlElement ToXme()
        {
            var xmd = new XmlDocument();
            var xme = xmd.CreateElement(GetType().ToShortTypeName());

            if (_name != null)
                xme.SetAttribute("Name", _name);

            if (_isBackground != null)
                xme.SetAttribute("IsBackground", _isBackground.ToString());

            return xme;
        }

        protected void AppendData(XmlElement xme)
        {
            _name = GetString(xme, "Name");
            _isBackground = GetBool(xme, "IsBackground");
        }

        protected abstract XRect GetBounds(XRect parentBounds);

        protected string GetString(XmlElement xme, string name)
        {
            var val = xme.Attributes[name];
            if (val != null)
                return val.Value;
            return null;
        }

        protected bool? GetBool(XmlElement xme, string name)
        {
            var val = xme.Attributes[name];
            if (val != null)
                return bool.Parse(val.Value);
            return null;
        }

        protected UnitValue? GetValue(XmlElement xme, string name)
        {
            var val = xme.Attributes[name];
            if (val != null)
                return UnitValue.Parse(val.Value);
            return null;
        }
    }
}