using System;
using Tharga.Reporter.Engine.Helper;

namespace Tharga.Reporter.Engine.Entity
{
    public class UnitValue : IEquatable<UnitValue>
    {
        public enum EUnit { Point, Inch, Millimeter, Centimeter, Percentage };

        #region Constructors

        private UnitValue(double value, EUnit unit)
        {
            Value = value;
            Unit = unit;
        }

        private UnitValue(string s)
        {
            //Cut numbers to the left, from unit to the riht
            var regex = new System.Text.RegularExpressions.Regex("([-]|[.,]|[-.,]|[0-9])[0-9]*[.,]*[0-9]*");
            var collection = regex.Matches(s);

            if (collection.Count != 1) throw new InvalidOperationException(string.Format("Cannot parse {0}.", s));
            if (collection[0].Index != 0) throw new InvalidOperationException(string.Format("Cannot parse {0}.", s));

            //Get the value part
            var value = s.Substring(0, collection[0].Length);
            double d;
            value = value.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            if (!double.TryParse(value, out d)) throw new InvalidOperationException(string.Format("Cannot parse {0} to a double.", value));
            Value = d;

            //Get the unit part
            Unit = s.Substring(collection[0].Length).ToUnit();
        }


        #endregion
        #region Factory


        public static bool TryParse(string s, out UnitValue result)
        {
            result = null;
            try
            {
                result = new UnitValue(s);
                return true;
            }
            catch (InvalidOperationException exp)
            {
                System.Diagnostics.Trace.TraceWarning(exp.Message);
                return false;
            }
        }

        public static UnitValue Parse(string s)
        {
            return new UnitValue(s);
        }

        internal static UnitValue Create()
        {
            return new UnitValue("0");
        }


        #endregion

        public bool Equals(UnitValue other)
        {
            if (Unit == EUnit.Percentage && other.Unit != EUnit.Percentage) throw new InvalidOperationException("Cannot compare UnitValues when the unit is in percentage, if not both values are in percentage.");
            if (Unit != EUnit.Percentage && other.Unit == EUnit.Percentage) throw new InvalidOperationException("Cannot compare UnitValues when the unit is in percentage, if not both values are in percentage.");

            var abs = Unit == other.Unit ? Math.Abs(Value - other.Value) : Math.Abs(GetXUnitValue(0) - other.GetXUnitValue(0));
            return abs < 0.0001;
        }

        new public string ToString()
        {
            return string.Format("{0}{1}", Value.ToString("0.####").Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, "."), Unit.ToShortString());
        }

        internal double Value { get; set; }
        internal EUnit Unit { get; set; }

        internal double GetXUnitValue(double totalValue)
        {
            PdfSharp.Drawing.XUnit value;
            switch (Unit)
            {
                case EUnit.Millimeter:
                    value = new PdfSharp.Drawing.XUnit(Value, PdfSharp.Drawing.XGraphicsUnit.Millimeter);
                    break;
                case EUnit.Centimeter:
                    value = new PdfSharp.Drawing.XUnit(Value, PdfSharp.Drawing.XGraphicsUnit.Centimeter);
                    break;
                case EUnit.Inch:
                    value = new PdfSharp.Drawing.XUnit(Value, PdfSharp.Drawing.XGraphicsUnit.Inch);
                    break;
                case EUnit.Percentage:
                    //Calculate the actual value, using provided total value
                    return Value / 100 * totalValue;
                case EUnit.Point:
                    return Value;
                default:
                    throw new InvalidOperationException(String.Format("Unknown unit {0}", Unit));
            }

            value.ConvertType(PdfSharp.Drawing.XGraphicsUnit.Point);
            return value.Value;
        }

        #region Operators


        public static UnitValue operator -(UnitValue a, UnitValue b)
        {
            if (a.Unit == EUnit.Percentage && b.Unit != EUnit.Percentage) throw new InvalidOperationException("Cannot use operators when the unit is in percentage, if not both values are in percentage.");
            if (a.Unit != EUnit.Percentage && b.Unit == EUnit.Percentage) throw new InvalidOperationException("Cannot use operators when the unit is in percentage, if not both values are in percentage.");

            if (a.Unit == b.Unit)
                return new UnitValue(a.Value - b.Value, a.Unit);
            return new UnitValue(a.GetXUnitValue(0) - b.GetXUnitValue(0), EUnit.Point);
        }

        public static UnitValue operator +(UnitValue a, UnitValue b)
        {
            if (a.Unit == EUnit.Percentage && b.Unit != EUnit.Percentage) throw new InvalidOperationException("Cannot use operators when the unit is in percentage, if not both values are in percentage.");
            if (a.Unit != EUnit.Percentage && b.Unit == EUnit.Percentage) throw new InvalidOperationException("Cannot use operators when the unit is in percentage, if not both values are in percentage.");

            if (a.Unit == b.Unit)
                return new UnitValue(a.Value + b.Value, a.Unit);
            return new UnitValue(a.GetXUnitValue(0) + b.GetXUnitValue(0), EUnit.Point);
        }

        public static bool operator ==(UnitValue a, UnitValue b)
        {
            if (((object)a) == ((object)b)) return true;
            if (((object)a) == null || ((object)b) == null) return false;

            return a.Equals(b);
        }

        public static bool operator !=(UnitValue a, UnitValue b)
        {
            if (((object)a) == ((object)b)) return false;
            if (((object)a) == null || ((object)b) == null) return true;
            return !a.Equals(b);
        }


        #endregion
    }
}