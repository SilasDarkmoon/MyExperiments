using System.Globalization;
using System.Runtime.InteropServices;

namespace gameplay.math;

[StructLayout(LayoutKind.Sequential)]
public readonly struct Number : IComparable, IComparable<Number>, IComparable<double>, IConvertible, IEquatable<Number>, IEquatable<double>, IFormattable, IEquatableApprox<Number, double>, IEquatableApprox<double, double>
{
    public readonly double Value;
    public Number(double value)
    {
        Value = value;
    }
    public static implicit operator Number(double value) { return new Number(value); }
    public static implicit operator double(Number value) { return value.Value; }

    public override int GetHashCode()
    {
        return Math.Round(Value, 4).GetHashCode();
    }
    public override bool Equals(object? obj)
    {
        return obj switch
        {
            Number n => EQApprox(Value, n.Value),
            double d => EQApprox(Value, d),
            _ => false,
        };
    }
    public override string ToString()
    {
        return Value.ToString();
    }

    public const double ComparisonEpsilon = 1e-6;
    public static bool EQApprox(double a, double b, double epsilon = ComparisonEpsilon)
    {
        return Math.Abs(a - b) <= epsilon;
    }
    public static bool NEApprox(double a, double b, double epsilon = ComparisonEpsilon)
    {
        return Math.Abs(a - b) > epsilon;
    }
    public static bool LTApprox(double a, double b, double epsilon = ComparisonEpsilon)
    {
        return b - a > epsilon;
    }
    public static bool LEApprox(double a, double b, double epsilon = ComparisonEpsilon)
    {
        return b - a >= -epsilon;
    }
    public static bool GTApprox(double a, double b, double epsilon = ComparisonEpsilon)
    {
        return LTApprox(b, a, epsilon);
    }
    public static bool GEApprox(double a, double b, double epsilon = ComparisonEpsilon)
    {
        return LEApprox(b, a, epsilon);
    }
    public static int CmpApprox(double a, double b, double epsilon = ComparisonEpsilon)
    {
        if (b - a > epsilon)
        {
            return -1;
        }
        else if (a - b > epsilon)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
    public bool EQApprox(double other, double epsilon = ComparisonEpsilon)
    {
        return EQApprox(Value, other, epsilon);
    }
    public bool NEApprox(double other, double epsilon = ComparisonEpsilon)
    {
        return NEApprox(Value, other, epsilon);
    }
    public bool LTApprox(double b, double epsilon = ComparisonEpsilon)
    {
        return LTApprox(Value, b, epsilon);
    }
    public bool LEApprox(double b, double epsilon = ComparisonEpsilon)
    {
        return LEApprox(Value, b, epsilon);
    }
    public bool GTApprox(double b, double epsilon = ComparisonEpsilon)
    {
        return GTApprox(Value, b, epsilon);
    }
    public bool GEApprox(double b, double epsilon = ComparisonEpsilon)
    {
        return GEApprox(Value, b, epsilon);
    }
    public int CmpApprox(double b, double epsilon = ComparisonEpsilon)
    {
        return CmpApprox(Value, b, epsilon);
    }

    #region operators
    #region op - equal
    public static bool operator ==(Number left, Number right)
    {
        return EQApprox(left.Value, right.Value);
    }
    public static bool operator !=(Number left, Number right)
    {
        return NEApprox(left.Value, right.Value);
    }
    public static bool operator ==(Number left, double right)
    {
        return EQApprox(left.Value, right);
    }
    public static bool operator !=(Number left, double right)
    {
        return NEApprox(left.Value, right);
    }
    public static bool operator ==(double left, Number right)
    {
        return EQApprox(left, right.Value);
    }
    public static bool operator !=(double left, Number right)
    {
        return NEApprox(left, right.Value);
    }
    #endregion
    #region op - compare
    public static bool operator <(Number left, Number right)
    {
        return LTApprox(left.Value, right.Value);
    }
    public static bool operator >(Number left, Number right)
    {
        return LTApprox(right.Value, left.Value);
    }
    public static bool operator <=(Number left, Number right)
    {
        return LEApprox(left.Value, right.Value);
    }
    public static bool operator >=(Number left, Number right)
    {
        return LEApprox(right.Value, left.Value);
    }
    public static bool operator <(Number left, double right)
    {
        return LTApprox(left.Value, right);
    }
    public static bool operator >(Number left, double right)
    {
        return LTApprox(right, left.Value);
    }
    public static bool operator <=(Number left, double right)
    {
        return LEApprox(left.Value, right);
    }
    public static bool operator >=(Number left, double right)
    {
        return LEApprox(right, left.Value);
    }
    public static bool operator <(double left, Number right)
    {
        return LTApprox(left, right.Value);
    }
    public static bool operator >(double left, Number right)
    {
        return LTApprox(right.Value, left);
    }
    public static bool operator <=(double left, Number right)
    {
        return LEApprox(left, right.Value);
    }
    public static bool operator >=(double left, Number right)
    {
        return LEApprox(right.Value, left);
    }
    #endregion
    #region op - arithmetic
    public static Number operator +(Number left, Number right)
    {
        return left.Value + right.Value;
    }
    public static Number operator -(Number left, Number right)
    {
        return left.Value - right.Value;
    }
    public static Number operator *(Number left, Number right)
    {
        return left.Value * right.Value;
    }
    public static Number operator /(Number left, Number right)
    {
        return left.Value / right.Value;
    }
    public static Number operator +(Number left, double right)
    {
        return left.Value + right;
    }
    public static Number operator -(Number left, double right)
    {
        return left.Value - right;
    }
    public static Number operator *(Number left, double right)
    {
        return left.Value * right;
    }
    public static Number operator /(Number left, double right)
    {
        return left.Value / right;
    }
    public static Number operator +(double left, Number right)
    {
        return left + right.Value;
    }
    public static Number operator -(double left, Number right)
    {
        return left - right.Value;
    }
    public static Number operator *(double left, Number right)
    {
        return left * right.Value;
    }
    public static Number operator /(double left, Number right)
    {
        return left / right.Value;
    }
    public static Number operator +(Number n)
    {
        return n;
    }
    public static Number operator -(Number n)
    {
        return -n.Value;
    }
    #endregion
    #endregion

    #region Converters
    // to bool
    public static explicit operator Number(bool v)
    {
        return v ? 1 : 0;
    }
    public static explicit operator bool(Number v)
    {
        return NEApprox(v.Value, 0);
    }
    // to char
    public static implicit operator Number(char v)
    {
        return new Number(v);
    }
    public static explicit operator char(Number v)
    {
        return (char)v.Value;
    }
    // to byte
    public static implicit operator Number(byte v)
    {
        return new Number(v);
    }
    public static explicit operator byte(Number v)
    {
        return (byte)v.Value;
    }
    // to sbyte
    public static implicit operator Number(sbyte v)
    {
        return new Number(v);
    }
    public static explicit operator sbyte(Number v)
    {
        return (sbyte)v.Value;
    }
    // to float
    public static implicit operator Number(float v)
    {
        return new Number(v);
    }
    public static explicit operator float(Number v)
    {
        return (float)v.Value;
    }
    // to decimal
    public static explicit operator Number(decimal v)
    {
        return new Number((double)v);
    }
    public static explicit operator decimal(Number v)
    {
        return (decimal)v.Value;
    }
    // to short
    public static implicit operator Number(short v)
    {
        return new Number(v);
    }
    public static explicit operator short(Number v)
    {
        return (short)v.Value;
    }
    // to ushort
    public static implicit operator Number(ushort v)
    {
        return new Number(v);
    }
    public static explicit operator ushort(Number v)
    {
        return (ushort)v.Value;
    }
    // to int
    public static implicit operator Number(int v)
    {
        return new Number(v);
    }
    public static explicit operator int(Number v)
    {
        return (int)v.Value;
    }
    // to uint
    public static implicit operator Number(uint v)
    {
        return new Number(v);
    }
    public static explicit operator uint(Number v)
    {
        return (uint)v.Value;
    }
    // to long
    public static implicit operator Number(long v)
    {
        return new Number(v);
    }
    public static explicit operator long(Number v)
    {
        return (long)v.Value;
    }
    // to ulong
    public static implicit operator Number(ulong v)
    {
        return new Number(v);
    }
    public static explicit operator ulong(Number v)
    {
        return (ulong)v.Value;
    }
    #endregion

    #region Parser
    public static Number Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.AllowThousands | NumberStyles.Float, IFormatProvider? provider = null)
    {
        return double.Parse(s, style, provider);
    }
    public static Number Parse(string s, IFormatProvider? provider)
    {
        return double.Parse(s, provider);
    }
    public static Number Parse(string s, NumberStyles style, IFormatProvider? provider)
    {
        return double.Parse(s, style, provider);
    }
    public static Number Parse(string s, NumberStyles style)
    {
        return double.Parse(s, style);
    }
    public static Number Parse(string s)
    {
        return double.Parse(s);
    }
    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out Number result)
    {
        bool suc = double.TryParse(s, style, provider, out double val);
        result = val;
        return suc;
    }
    public static bool TryParse(ReadOnlySpan<char> s, out Number result)
    {
        bool suc = double.TryParse(s, out double val);
        result = val;
        return suc;
    }
    public static bool TryParse(string s, NumberStyles style, IFormatProvider? provider, out Number result)
    {
        bool suc = double.TryParse(s, style, provider, out double val);
        result = val;
        return suc;
    }
    public static bool TryParse(string s, out Number result)
    {
        bool suc = double.TryParse(s, out double val);
        result = val;
        return suc;
    }
    #endregion

    #region IComparable
    public int CompareTo(object? obj)
    {
        return obj switch
        {
            Number n => CmpApprox(Value, n.Value),
            double d => CmpApprox(Value, d),
            _ => throw new System.ArgumentException($"object {nameof(obj)} must be of type Number or Double."),
        };
    }
    #endregion

    #region IComparable<T>
    public int CompareTo(Number other)
    {
        return CmpApprox(Value, other.Value);
    }
    public int CompareTo(double other)
    {
        return CmpApprox(Value, other);
    }
    #endregion

    #region IEquatable<T>
    public bool Equals(Number other)
    {
        return EQApprox(Value, other.Value);
    }
    public bool Equals(double other)
    {
        return EQApprox(Value, other);
    }
    #endregion

    #region IFormattable
    public string ToString(IFormatProvider? provider)
    {
        return Value.ToString(provider);
    }
    public string ToString(string? format)
    {
        return Value.ToString(format);
    }
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return Value.ToString(format, formatProvider);
    }
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        return Value.TryFormat(destination, out charsWritten, format, provider);
    }
    #endregion

    #region IConvertible
    public TypeCode GetTypeCode()
    {
        return Value.GetTypeCode();
    }
    public bool ToBoolean(IFormatProvider? provider = null)
    {
        return NEApprox(Value, 0);
    }
    public byte ToByte(IFormatProvider? provider = null)
    {
        return Convert.ToByte(Value);
    }
    public char ToChar(IFormatProvider? provider = null)
    {
        return Convert.ToChar(Value);
    }
    public DateTime ToDateTime(IFormatProvider? provider = null)
    {
        return Convert.ToDateTime(Value);
    }
    public decimal ToDecimal(IFormatProvider? provider = null)
    {
        return Convert.ToDecimal(Value);
    }
    public double ToDouble(IFormatProvider? provider = null)
    {
        return Value;
    }
    public short ToInt16(IFormatProvider? provider = null)
    {
        return Convert.ToInt16(Value);
    }
    public int ToInt32(IFormatProvider? provider = null)
    {
        return Convert.ToInt32(Value);
    }
    public long ToInt64(IFormatProvider? provider = null)
    {
        return Convert.ToInt64(Value);
    }
    public sbyte ToSByte(IFormatProvider? provider = null)
    {
        return Convert.ToSByte(Value);
    }
    public float ToSingle(IFormatProvider? provider = null)
    {
        return Convert.ToSingle(Value);
    }
    public object ToType(Type conversionType, IFormatProvider? provider = null)
    {
        if (conversionType == typeof(Number))
        {
            return this;
        }
        else
        {
            return Convert.ChangeType(Value, conversionType, provider);
        }
    }
    public ushort ToUInt16(IFormatProvider? provider = null)
    {
        return Convert.ToUInt16(Value);
    }
    public uint ToUInt32(IFormatProvider? provider = null)
    {
        return Convert.ToUInt32(Value);
    }
    public ulong ToUInt64(IFormatProvider? provider = null)
    {
        return Convert.ToUInt64(Value);
    }
    #endregion

    #region IEquatableApprox
    public bool EQApprox(Number other, double epsilon = ComparisonEpsilon)
    {
        return EQApprox(Value, other.Value, epsilon);
    }
    public bool NEApprox(Number other, double epsilon = ComparisonEpsilon)
    {
        return NEApprox(Value, other.Value, epsilon);
    }
    bool IEquatableApprox.EQApprox(object other, double epsilon)
    {
        return other switch
        {
            Number n => EQApprox(Value, n.Value, epsilon),
            double d => EQApprox(Value, d, epsilon),
            _ => false,
        };
    }
    bool IEquatableApprox.NEApprox(object other, double epsilon)
    {
        return other switch
        {
            Number n => NEApprox(Value, n.Value, epsilon),
            double d => NEApprox(Value, d, epsilon),
            _ => true,
        };
    }
    #endregion

    #region Math
    public static Number Abs(Number value)
    {
        return Math.Abs(value.Value);
    }
    public static Number Acos(Number d)
    {
        return Math.Acos(d.Value);
    }
    public static Number Acosh(Number d)
    {
        return Math.Acosh(d.Value);
    }
    public static Number Asin(Number d)
    {
        return Math.Asin(d.Value);
    }
    public static Number Asinh(Number d)
    {
        return Math.Asinh(d.Value);
    }
    public static Number Atan(Number d)
    {
        return Math.Atan(d.Value);
    }
    public static Number Atan2(Number y, Number x)
    {
        return Math.Atan2(y.Value, x.Value);
    }
    public static Number Atanh(Number d)
    {
        return Math.Atanh(d.Value);
    }
    public static Number Cbrt(Number d)
    {
        return Math.Cbrt(d.Value);
    }
    public static Number Ceiling(Number a)
    {
        return Math.Ceiling(a.Value);
    }
    public static Number Clamp(Number value, Number min, Number max)
    {
        return Math.Clamp(value.Value, min.Value, max.Value);
    }
    public static Number Cos(Number d)
    {
        return Math.Cos(d.Value);
    }
    public static Number Cosh(Number value)
    {
        return Math.Cosh(value.Value);
    }
    public static Number Exp(Number d)
    {
        return Math.Exp(d.Value);
    }
    public static Number Floor(Number d)
    {
        return Math.Floor(d.Value);
    }
    public static Number IEEERemainder(Number x, Number y)
    {
        return Math.IEEERemainder(x.Value, y.Value);
    }
    public static Number Log(Number d)
    {
        return Math.Log(d.Value);
    }
    public static Number Log(Number a, Number newBase)
    {
        return Math.Log(a.Value, newBase.Value);
    }
    public static Number Log10(Number d)
    {
        return Math.Log10(d.Value);
    }
    public static Number Max(Number val1, Number val2)
    {
        return Math.Max(val1.Value, val2.Value);
    }
    public static Number Min(Number val1, Number val2)
    {
        return Math.Min(val1.Value, val2.Value);
    }
    public static Number Pow(Number x, Number y)
    {
        return Math.Pow(x.Value, y.Value);
    }
    public static int Sign(Number value)
    {
        return Math.Sign(value.Value);
    }
    public static Number Sin(Number a)
    {
        return Math.Sin(a.Value);
    }
    public static Number Sinh(Number value)
    {
        return Math.Sinh(value.Value);
    }
    public static Number Sqrt(Number d)
    {
        return Math.Sqrt(d.Value);
    }
    public static Number Tan(Number a)
    {
        return Math.Tan(a.Value);
    }
    public static Number Tanh(Number value)
    {
        return Math.Tanh(value.Value);
    }
    public static Number Truncate(Number d)
    {
        return Math.Truncate(d.Value);
    }
    public static Number Round(Number value, MidpointRounding mode)
    {
        return Math.Round(value.Value, mode);
    }
    public static Number Round(Number value, int digits, MidpointRounding mode)
    {
        return Math.Round(value.Value, digits, mode);
    }
    public static Number Round(Number value, int digits)
    {
        return Math.Round(value.Value, digits);
    }
    public static Number Round(Number a)
    {
        return Math.Round(a.Value);
    }
    #endregion

    #region statics and consts from native double
    public static readonly Number Epsilon = double.Epsilon;
    public static readonly Number MaxValue = double.MaxValue;
    public static readonly Number MinValue = double.MinValue;
    public static readonly Number NaN = double.NaN;
    public static readonly Number NegativeInfinity = double.NegativeInfinity;
    public static readonly Number PositiveInfinity = double.PositiveInfinity;
    public static readonly Number PI = Math.PI;
    public static readonly Number E = Math.E;
    public static readonly Number Tau = Math.PI * 2.0;

    public static bool IsFinite(Number d) { return double.IsFinite(d.Value); }
    public static bool IsInfinity(Number d) { return double.IsInfinity(d.Value); }
    public static bool IsNaN(Number d) { return double.IsNaN(d.Value); }
    public static bool IsNegative(Number d) { return double.IsNegative(d.Value); }
    public static bool IsNegativeInfinity(Number d) { return double.IsNegativeInfinity(d.Value); }
    public static bool IsNormal(Number d) { return double.IsNormal(d.Value); }
    public static bool IsPositiveInfinity(Number d) { return double.IsPositiveInfinity(d.Value); }
    public static bool IsSubnormal(Number d) { return double.IsSubnormal(d.Value); }

    #region instance equivalent to static methods
    public bool IsFinite() { return double.IsFinite(Value); }
    public bool IsInfinity() { return double.IsInfinity(Value); }
    public bool IsNaN() { return double.IsNaN(Value); }
    public bool IsNegative() { return double.IsNegative(Value); }
    public bool IsNegativeInfinity() { return double.IsNegativeInfinity(Value); }
    public bool IsNormal() { return double.IsNormal(Value); }
    public bool IsPositiveInfinity() { return double.IsPositiveInfinity(Value); }
    public bool IsSubnormal() { return double.IsSubnormal(Value); }
    #endregion
    #endregion
}

public static class DoubleExtensions
{
    public static bool EQApprox(this double a, double b, double epsilon = Number.ComparisonEpsilon)
    {
        return Number.EQApprox(a, b, epsilon);
    }
    public static bool NEApprox(this double a, double b, double epsilon = Number.ComparisonEpsilon)
    {
        return Number.NEApprox(a, b, epsilon);
    }
    /// <summary>
    /// less than
    /// </summary>
    public static bool LTApprox(this double a, double b, double epsilon = Number.ComparisonEpsilon)
    {
        return Number.LTApprox(a, b, epsilon);
    }
    /// <summary>
    /// less than or equal to
    /// </summary>
    public static bool LEApprox(this double a, double b, double epsilon = Number.ComparisonEpsilon)
    {
        return Number.LEApprox(a, b, epsilon);
    }
    /// <summary>
    /// greater than
    /// </summary>
    public static bool GTApprox(this double a, double b, double epsilon = Number.ComparisonEpsilon)
    {
        return Number.GTApprox(a, b, epsilon);
    }
    /// <summary>
    /// greater than or equal to
    /// </summary>
    public static bool GEApprox(this double a, double b, double epsilon = Number.ComparisonEpsilon)
    {
        return Number.GEApprox(a, b, epsilon);
    }
    public static int CmpApprox(this double a, double b, double epsilon = Number.ComparisonEpsilon)
    {
        return Number.CmpApprox(a, b, epsilon);
    }

    #region extension equivalent to double static methods
    public static bool IsFinite(this double d) { return double.IsFinite(d); }
    public static bool IsInfinity(this double d) { return double.IsInfinity(d); }
    public static bool IsNaN(this double d) { return double.IsNaN(d); }
    public static bool IsNegative(this double d) { return double.IsNegative(d); }
    public static bool IsNegativeInfinity(this double d) { return double.IsNegativeInfinity(d); }
    public static bool IsNormal(this double d) { return double.IsNormal(d); }
    public static bool IsPositiveInfinity(this double d) { return double.IsPositiveInfinity(d); }
    public static bool IsSubnormal(this double d) { return double.IsSubnormal(d); }
    #endregion
}
