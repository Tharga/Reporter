using System.Drawing;
using System.Globalization;

namespace Tharga.Reporter.Entity.Element.Extensions;

internal static class ElementExtensions
{
    internal static Color ToColor(this string value)
    {
        var rs = value.Substring(0, 2);
        var gs = value.Substring(2, 2);
        var bs = value.Substring(4, 2);

        var r = int.Parse(rs, NumberStyles.HexNumber);
        var g = int.Parse(gs, NumberStyles.HexNumber);
        var b = int.Parse(bs, NumberStyles.HexNumber);

        var color = Color.FromArgb(r, g, b);
        return color;
    }
}