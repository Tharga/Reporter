using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using PdfSharp.Drawing;
using Tharga.Reporter.Extensions;

namespace Tharga.Reporter.Entity;

//TODO: Move stuff to extensions
public struct UnitValue : IEquatable<UnitValue>
{
    public enum EUnit
    {
        Point,
        Inch,
        Millimeter,
        Centimeter,
        Percentage
    }

    internal double Value { get; }

    internal EUnit Unit { get; }

    #region Constructors

    internal UnitValue(double value, EUnit unit)
    {
        Value = value;
        Unit = unit;
    }

    private UnitValue(string s)
    {
        //Cut numbers to the left, from unit to the riht
        var regex = new Regex("([-]|[.,]|[-.,]|[0-9])[0-9]*[.,]*[0-9]*");
        var collection = regex.Matches(s);

        if (collection.Count != 1) throw new InvalidOperationException(string.Format("Cannot parse {0}.", s));
        if (collection[0].Index != 0) throw new InvalidOperationException(string.Format("Cannot parse {0}.", s));

        //Get the value part
        var value = s.Substring(0, collection[0].Length);
        double d;
        value = value.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
        if (!double.TryParse(value, out d)) throw new InvalidOperationException(string.Format("Cannot parse {0} to a double.", value));
        Value = d;

        //Get the unit part
        Unit = s.Substring(collection[0].Length).ToUnit();
    }

    #endregion

    #region Factory

    public static bool TryParse(string s, out UnitValue result)
    {
        result = new UnitValue(0, EUnit.Point);
        try
        {
            result = new UnitValue(s);
            return true;
        }
        catch (InvalidOperationException exp)
        {
            Trace.TraceWarning(exp.Message);
            return false;
        }
    }

    public static UnitValue Parse(string s)
    {
        return new UnitValue(s);
    }

    #endregion

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        return obj is UnitValue && Equals((UnitValue)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (Value.GetHashCode() * 397) ^ (int)Unit;
        }
    }

    public bool Equals(UnitValue other)
    {
        return Value.Equals(other.Value) && Unit == other.Unit;
    }

    public new string ToString()
    {
        return string.Format("{0}{1}", Value.ToString("0.####").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, "."), Unit.ToShortString());
    }

    internal double GetXUnitValue(double totalValue)
    {
        XUnit value;
        switch (Unit)
        {
            case EUnit.Millimeter:
                value = new XUnit(Value, XGraphicsUnit.Millimeter);
                break;
            case EUnit.Centimeter:
                value = new XUnit(Value, XGraphicsUnit.Centimeter);
                break;
            case EUnit.Inch:
                value = new XUnit(Value, XGraphicsUnit.Inch);
                break;
            case EUnit.Percentage:
                //Calculate the actual value, using provided total value
                return Value / 100 * totalValue;
            case EUnit.Point:
                return Value;
            default:
                throw new InvalidOperationException(string.Format("Unknown unit {0}", Unit));
        }

        value.ConvertType(XGraphicsUnit.Point);
        return value.Value;
    }

    #region Operators

    public static UnitValue operator -(UnitValue a, UnitValue b)
    {
        if (a.Unit == EUnit.Percentage && b.Unit != EUnit.Percentage) throw new InvalidOperationException("Cannot use operators when the unit is in percentage, if not both values are in percentage.");
        if (a.Unit != EUnit.Percentage && b.Unit == EUnit.Percentage) throw new InvalidOperationException("Cannot use operators when the unit is in percentage, if not both values are in percentage.");

        if (a.Unit == b.Unit)
        {
            return new UnitValue(a.Value - b.Value, a.Unit);
        }

        return new UnitValue(a.GetXUnitValue(0) - b.GetXUnitValue(0), EUnit.Point);
    }

    public static UnitValue operator +(UnitValue a, UnitValue b)
    {
        if (a.Unit == EUnit.Percentage && b.Unit != EUnit.Percentage) throw new InvalidOperationException("Cannot use operators when the unit is in percentage, if not both values are in percentage.");
        if (a.Unit != EUnit.Percentage && b.Unit == EUnit.Percentage) throw new InvalidOperationException("Cannot use operators when the unit is in percentage, if not both values are in percentage.");

        if (a.Unit == b.Unit)
        {
            return new UnitValue(a.Value + b.Value, a.Unit);
        }

        return new UnitValue(a.GetXUnitValue(0) + b.GetXUnitValue(0), EUnit.Point);
    }

    public static bool operator ==(UnitValue a, UnitValue b)
    {
        if ((object)a == (object)b) return true;
        if ((object)a == null || (object)b == null) return false;
        return a.Equals(b);
    }

    public static bool operator !=(UnitValue a, UnitValue b)
    {
        if ((object)a == (object)b) return false;
        if ((object)a == null || (object)b == null) return true;
        return !a.Equals(b);
    }

    #endregion

    public static implicit operator string(UnitValue item)
    {
        return item.ToString();
    }

    public static implicit operator UnitValue(string item)
    {
        return Parse(item);
    }
}