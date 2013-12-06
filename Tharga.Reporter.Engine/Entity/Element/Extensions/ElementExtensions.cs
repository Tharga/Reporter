using System;
using System.Drawing;

namespace Tharga.Reporter.Engine.Entity.Element
{
    internal static class ElementExtensions
    {
        internal static Color ToColor(this string value)
        {
            var rs = value.Substring(0, 2);
            var gs = value.Substring(2, 2);
            var bs = value.Substring(4, 2);

            var r = Int32.Parse(rs, System.Globalization.NumberStyles.HexNumber);
            var g = Int32.Parse(gs, System.Globalization.NumberStyles.HexNumber);
            var b = Int32.Parse(bs, System.Globalization.NumberStyles.HexNumber);

            var color = Color.FromArgb(r, g, b);
            return color;
        }
    }
}