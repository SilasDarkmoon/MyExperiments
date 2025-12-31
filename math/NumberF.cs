using System.Globalization;
using System.Runtime.InteropServices;

namespace gameplay.math;

[StructLayout(LayoutKind.Sequential)]
public readonly struct NumberF : IComparable, IComparable<NumberF>, IComparable<float>, IConvertible, IEquatable<NumberF>, IEquatable<float>, IFormattable, IEquatableApprox<NumberF, float>, IEquatableApprox<float, float>
{
    public readonly float Value;
    public NumberF(float value)
    {
        Value = value;
    }
    public static implicit operator NumberF(float value) { return new NumberF(value); }
    public static implicit operator float(NumberF value) { return value.Value; }
    public static explicit operator NumberF(Number v) { return new NumberF((float)v); }
    public static implicit operator Number(NumberF v) { return v.Value; }

    public override int GetHashCode()
    {
        return MathF.Round(Value, 4).GetHashCode();
    }
    public override bool Equals(object? obj)
    {
        return obj switch
        {
            NumberF n => EQApprox(Value, n.Value),
            float d => EQApprox(Value, d),
            _ => false,
        };
    }
    public override string ToString()
    {
        return Value.ToString();
    }

    public const float ComparisonEpsilon = 1e-6f;
    public static bool EQApprox(float a, float b, float epsilon = ComparisonEpsilon)
    {
        return MathF.Abs(a - b) <= epsilon;
    }
    public static bool NEApprox(float a, float b, float epsilon = ComparisonEpsilon)
    {
        return MathF.Abs(a - b) > epsilon;
    }
    public static bool LTApprox(float a, float b, float epsilon = ComparisonEpsilon)
    {
        return b - a > epsilon;
    }
    public static bool LEApprox(float a, float b, float epsilon = ComparisonEpsilon)
    {
        return b - a >= -epsilon;
    }
    public static bool GTApprox(float a, float b, float epsilon = ComparisonEpsilon)
    {
        return LTApprox(b, a, epsilon);
    }
    public static bool GEApprox(float a, float b, float epsilon = ComparisonEpsilon)
    {
        return LEApprox(b, a, epsilon);
    }
    public static int CmpApprox(float a, float b, float epsilon = ComparisonEpsilon)
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
    public bool EQApprox(float other, float epsilon = ComparisonEpsilon)
    {
        return EQApprox(Value, other, epsilon);
    }
    public bool NEApprox(float other, float epsilon = ComparisonEpsilon)
    {
        return NEApprox(Value, other, epsilon);
    }
    public bool LTApprox(float b, float epsilon = ComparisonEpsilon)
    {
        return LTApprox(Value, b, epsilon);
    }
    public bool LEApprox(float b, float epsilon = ComparisonEpsilon)
    {
        return LEApprox(Value, b, epsilon);
    }
    public bool GTApprox(float b, float epsilon = ComparisonEpsilon)
    {
        return GTApprox(Value, b, epsilon);
    }
    public bool GEApprox(float b, float epsilon = ComparisonEpsilon)
    {
        return GEApprox(Value, b, epsilon);
    }
    public int CmpApprox(float b, float epsilon = ComparisonEpsilon)
    {
        return CmpApprox(Value, b, epsilon);
    }

    #region operators
    #region op - equal
    public static bool operator ==(NumberF left, NumberF right)
    {
        return EQApprox(left.Value, right.Value);
    }
    public static bool operator !=(NumberF left, NumberF right)
    {
        return NEApprox(left.Value, right.Value);
    }
    public static bool operator ==(NumberF left, float right)
    {
        return EQApprox(left.Value, right);
    }
    public static bool operator !=(NumberF left, float right)
    {
        return NEApprox(left.Value, right);
    }
    public static bool operator ==(float left, NumberF right)
    {
        return EQApprox(left, right.Value);
    }
    public static bool operator !=(float left, NumberF right)
    {
        return NEApprox(left, right.Value);
    }
    #endregion
    #region op - compare
    public static bool operator <(NumberF left, NumberF right)
    {
        return LTApprox(left.Value, right.Value);
    }
    public static bool operator >(NumberF left, NumberF right)
    {
        return LTApprox(right.Value, left.Value);
    }
    public static bool operator <=(NumberF left, NumberF right)
    {
        return LEApprox(left.Value, right.Value);
    }
    public static bool operator >=(NumberF left, NumberF right)
    {
        return LEApprox(right.Value, left.Value);
    }
    public static bool operator <(NumberF left, float right)
    {
        return LTApprox(left.Value, right);
    }
    public static bool operator >(NumberF left, float right)
    {
        return LTApprox(right, left.Value);
    }
    public static bool operator <=(NumberF left, float right)
    {
        return LEApprox(left.Value, right);
    }
    public static bool operator >=(NumberF left, float right)
    {
        return LEApprox(right, left.Value);
    }
    public static bool operator <(float left, NumberF right)
    {
        return LTApprox(left, right.Value);
    }
    public static bool operator >(float left, NumberF right)
    {
        return LTApprox(right.Value, left);
    }
    public static bool operator <=(float left, NumberF right)
    {
        return LEApprox(left, right.Value);
    }
    public static bool operator >=(float left, NumberF right)
    {
        return LEApprox(right.Value, left);
    }
    #endregion
    #region op - arithmetic
    public static NumberF operator +(NumberF left, NumberF right)
    {
        return left.Value + right.Value;
    }
    public static NumberF operator -(NumberF left, NumberF right)
    {
        return left.Value - right.Value;
    }
    public static NumberF operator *(NumberF left, NumberF right)
    {
        return left.Value * right.Value;
    }
    public static NumberF operator /(NumberF left, NumberF right)
    {
        return left.Value / right.Value;
    }
    public static NumberF operator +(NumberF left, float right)
    {
        return left.Value + right;
    }
    public static NumberF operator -(NumberF left, float right)
    {
        return left.Value - right;
    }
    public static NumberF operator *(NumberF left, float right)
    {
        return left.Value * right;
    }
    public static NumberF operator /(NumberF left, float right)
    {
        return left.Value / right;
    }
    public static NumberF operator +(float left, NumberF right)
    {
        return left + right.Value;
    }
    public static NumberF operator -(float left, NumberF right)
    {
        return left - right.Value;
    }
    public static NumberF operator *(float left, NumberF right)
    {
        return left * right.Value;
    }
    public static NumberF operator /(float left, NumberF right)
    {
        return left / right.Value;
    }
    public static NumberF operator +(NumberF n)
    {
        return n;
    }
    public static NumberF operator -(NumberF n)
    {
        return -n.Value;
    }
    #endregion
    #endregion

    #region Converters
    // to bool
    public static explicit operator NumberF(bool v)
    {
        return v ? 1 : 0;
    }
    public static explicit operator bool(NumberF v)
    {
        return NEApprox(v.Value, 0);
    }
    // to char
    public static implicit operator NumberF(char v)
    {
        return new NumberF(v);
    }
    public static explicit operator char(NumberF v)
    {
        return (char)v.Value;
    }
    // to byte
    public static implicit operator NumberF(byte v)
    {
        return new NumberF(v);
    }
    public static explicit operator byte(NumberF v)
    {
        return (byte)v.Value;
    }
    // to sbyte
    public static implicit operator NumberF(sbyte v)
    {
        return new NumberF(v);
    }
    public static explicit operator sbyte(NumberF v)
    {
        return (sbyte)v.Value;
    }
    // to double
    public static explicit operator NumberF(double v)
    {
        return new NumberF((float)v);
    }
    public static implicit operator double(NumberF v)
    {
        return v.Value;
    }
    // to decimal
    public static explicit operator NumberF(decimal v)
    {
        return new NumberF((float)v);
    }
    public static explicit operator decimal(NumberF v)
    {
        return (decimal)v.Value;
    }
    // to short
    public static implicit operator NumberF(short v)
    {
        return new NumberF(v);
    }
    public static explicit operator short(NumberF v)
    {
        return (short)v.Value;
    }
    // to ushort
    public static implicit operator NumberF(ushort v)
    {
        return new NumberF(v);
    }
    public static explicit operator ushort(NumberF v)
    {
        return (ushort)v.Value;
    }
    // to int
    public static implicit operator NumberF(int v)
    {
        return new NumberF(v);
    }
    public static explicit operator int(NumberF v)
    {
        return (int)v.Value;
    }
    // to uint
    public static implicit operator NumberF(uint v)
    {
        return new NumberF(v);
    }
    public static explicit operator uint(NumberF v)
    {
        return (uint)v.Value;
    }
    // to long
    public static implicit operator NumberF(long v)
    {
        return new NumberF(v);
    }
    public static explicit operator long(NumberF v)
    {
        return (long)v.Value;
    }
    // to ulong
    public static implicit operator NumberF(ulong v)
    {
        return new NumberF(v);
    }
    public static explicit operator ulong(NumberF v)
    {
        return (ulong)v.Value;
    }
    #endregion

    #region Parser
    public static NumberF Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.AllowThousands | NumberStyles.Float, IFormatProvider? provider = null)
    {
        return float.Parse(s, style, provider);
    }
    public static NumberF Parse(string s, IFormatProvider? provider)
    {
        return float.Parse(s, provider);
    }
    public static NumberF Parse(string s, NumberStyles style, IFormatProvider? provider)
    {
        return float.Parse(s, style, provider);
    }
    public static NumberF Parse(string s, NumberStyles style)
    {
        return float.Parse(s, style);
    }
    public static NumberF Parse(string s)
    {
        return float.Parse(s);
    }
    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out NumberF result)
    {
        bool suc = float.TryParse(s, style, provider, out float val);
        result = val;
        return suc;
    }
    public static bool TryParse(ReadOnlySpan<char> s, out NumberF result)
    {
        bool suc = float.TryParse(s, out float val);
        result = val;
        return suc;
    }
    public static bool TryParse(string s, NumberStyles style, IFormatProvider? provider, out NumberF result)
    {
        bool suc = float.TryParse(s, style, provider, out float val);
        result = val;
        return suc;
    }
    public static bool TryParse(string s, out NumberF result)
    {
        bool suc = float.TryParse(s, out float val);
        result = val;
        return suc;
    }
    #endregion

    #region IComparable
    public int CompareTo(object? obj)
    {
        return obj switch
        {
            NumberF n => CmpApprox(Value, n.Value),
            float d => CmpApprox(Value, d),
            _ => throw new System.ArgumentException($"object {nameof(obj)} must be of type NumberF or Single."),
        };
    }
    #endregion

    #region IComparable<T>
    public int CompareTo(NumberF other)
    {
        return CmpApprox(Value, other.Value);
    }
    public int CompareTo(float other)
    {
        return CmpApprox(Value, other);
    }
    #endregion

    #region IEquatable<T>
    public bool Equals(NumberF other)
    {
        return EQApprox(Value, other.Value);
    }
    public bool Equals(float other)
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
        return Convert.ToDouble(Value);
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
        return Value;
    }
    public object ToType(Type conversionType, IFormatProvider? provider = null)
    {
        if (conversionType == typeof(NumberF))
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
    public bool EQApprox(NumberF other, float epsilon = ComparisonEpsilon)
    {
        return EQApprox(Value, other.Value, epsilon);
    }
    public bool NEApprox(NumberF other, float epsilon = ComparisonEpsilon)
    {
        return NEApprox(Value, other.Value, epsilon);
    }
    bool IEquatableApprox.EQApprox(object other, double epsilon)
    {
        return other switch
        {
            NumberF n => EQApprox(Value, n.Value, (float)epsilon),
            float d => EQApprox(Value, d, (float)epsilon),
            _ => false,
        };
    }
    bool IEquatableApprox.NEApprox(object other, double epsilon)
    {
        return other switch
        {
            NumberF n => NEApprox(Value, n.Value, (float)epsilon),
            float d => NEApprox(Value, d, (float)epsilon),
            _ => true,
        };
    }
    #endregion

    #region Math
    public static NumberF Abs(NumberF value)
    {
        return MathF.Abs(value.Value);
    }
    public static NumberF Acos(NumberF d)
    {
        return MathF.Acos(d.Value);
    }
    public static NumberF Acosh(NumberF d)
    {
        return MathF.Acosh(d.Value);
    }
    public static NumberF Asin(NumberF d)
    {
        return MathF.Asin(d.Value);
    }
    public static NumberF Asinh(NumberF d)
    {
        return MathF.Asinh(d.Value);
    }
    public static NumberF Atan(NumberF d)
    {
        return MathF.Atan(d.Value);
    }
    public static NumberF Atan2(NumberF y, NumberF x)
    {
        return MathF.Atan2(y.Value, x.Value);
    }
    public static NumberF Atanh(NumberF d)
    {
        return MathF.Atanh(d.Value);
    }
    public static NumberF Cbrt(NumberF d)
    {
        return MathF.Cbrt(d.Value);
    }
    public static NumberF Ceiling(NumberF a)
    {
        return MathF.Ceiling(a.Value);
    }
    public static NumberF Clamp(NumberF value, NumberF min, NumberF max)
    {
        return Math.Clamp(value.Value, min.Value, max.Value);
    }
    public static NumberF Cos(NumberF d)
    {
        return MathF.Cos(d.Value);
    }
    public static NumberF Cosh(NumberF value)
    {
        return MathF.Cosh(value.Value);
    }
    public static NumberF Exp(NumberF d)
    {
        return MathF.Exp(d.Value);
    }
    public static NumberF Floor(NumberF d)
    {
        return MathF.Floor(d.Value);
    }
    public static NumberF IEEERemainder(NumberF x, NumberF y)
    {
        return MathF.IEEERemainder(x.Value, y.Value);
    }
    public static NumberF Log(NumberF d)
    {
        return MathF.Log(d.Value);
    }
    public static NumberF Log(NumberF a, NumberF newBase)
    {
        return MathF.Log(a.Value, newBase.Value);
    }
    public static NumberF Log10(NumberF d)
    {
        return MathF.Log10(d.Value);
    }
    public static NumberF Max(NumberF val1, NumberF val2)
    {
        return MathF.Max(val1.Value, val2.Value);
    }
    public static NumberF Min(NumberF val1, NumberF val2)
    {
        return MathF.Min(val1.Value, val2.Value);
    }
    public static NumberF Pow(NumberF x, NumberF y)
    {
        return MathF.Pow(x.Value, y.Value);
    }
    public static int Sign(NumberF value)
    {
        return MathF.Sign(value.Value);
    }
    public static NumberF Sin(NumberF a)
    {
        return MathF.Sin(a.Value);
    }
    public static NumberF Sinh(NumberF value)
    {
        return MathF.Sinh(value.Value);
    }
    public static NumberF Sqrt(NumberF d)
    {
        return MathF.Sqrt(d.Value);
    }
    public static NumberF Tan(NumberF a)
    {
        return MathF.Tan(a.Value);
    }
    public static NumberF Tanh(NumberF value)
    {
        return MathF.Tanh(value.Value);
    }
    public static NumberF Truncate(NumberF d)
    {
        return MathF.Truncate(d.Value);
    }
    public static NumberF Round(NumberF value, MidpointRounding mode)
    {
        return MathF.Round(value.Value, mode);
    }
    public static NumberF Round(NumberF value, int digits, MidpointRounding mode)
    {
        return MathF.Round(value.Value, digits, mode);
    }
    public static NumberF Round(NumberF value, int digits)
    {
        return MathF.Round(value.Value, digits);
    }
    public static NumberF Round(NumberF a)
    {
        return MathF.Round(a.Value);
    }
    #endregion

    #region statics and consts from native float
    public static readonly NumberF Epsilon = float.Epsilon;
    public static readonly NumberF MaxValue = float.MaxValue;
    public static readonly NumberF MinValue = float.MinValue;
    public static readonly NumberF NaN = float.NaN;
    public static readonly NumberF NegativeInfinity = float.NegativeInfinity;
    public static readonly NumberF PositiveInfinity = float.PositiveInfinity;
    public static readonly NumberF PI = MathF.PI;
    public static readonly NumberF E = MathF.E;
    public static readonly NumberF Tau = MathF.PI * 2.0f;

    public static bool IsFinite(NumberF d) { return float.IsFinite(d.Value); }
    public static bool IsInfinity(NumberF d) { return float.IsInfinity(d.Value); }
    public static bool IsNaN(NumberF d) { return float.IsNaN(d.Value); }
    public static bool IsNegative(NumberF d) { return float.IsNegative(d.Value); }
    public static bool IsNegativeInfinity(NumberF d) { return float.IsNegativeInfinity(d.Value); }
    public static bool IsNormal(NumberF d) { return float.IsNormal(d.Value); }
    public static bool IsPositiveInfinity(NumberF d) { return float.IsPositiveInfinity(d.Value); }
    public static bool IsSubnormal(NumberF d) { return float.IsSubnormal(d.Value); }

    #region instance equivalent to static methods
    public bool IsFinite() { return float.IsFinite(Value); }
    public bool IsInfinity() { return float.IsInfinity(Value); }
    public bool IsNaN() { return float.IsNaN(Value); }
    public bool IsNegative() { return float.IsNegative(Value); }
    public bool IsNegativeInfinity() { return float.IsNegativeInfinity(Value); }
    public bool IsNormal() { return float.IsNormal(Value); }
    public bool IsPositiveInfinity() { return float.IsPositiveInfinity(Value); }
    public bool IsSubnormal() { return float.IsSubnormal(Value); }
    #endregion
    #endregion
}

public static class SingleExtensions
{
    public static bool EQApprox(this float a, float b, float epsilon = NumberF.ComparisonEpsilon)
    {
        return NumberF.EQApprox(a, b, epsilon);
    }
    public static bool NEApprox(this float a, float b, float epsilon = NumberF.ComparisonEpsilon)
    {
        return NumberF.NEApprox(a, b, epsilon);
    }
    public static bool LTApprox(this float a, float b, float epsilon = NumberF.ComparisonEpsilon)
    {
        return NumberF.LTApprox(a, b, epsilon);
    }
    public static bool LEApprox(this float a, float b, float epsilon = NumberF.ComparisonEpsilon)
    {
        return NumberF.LEApprox(a, b, epsilon);
    }
    public static bool GTApprox(this float a, float b, float epsilon = NumberF.ComparisonEpsilon)
    {
        return NumberF.GTApprox(a, b, epsilon);
    }
    public static bool GEApprox(this float a, float b, float epsilon = NumberF.ComparisonEpsilon)
    {
        return NumberF.GEApprox(a, b, epsilon);
    }
    public static int CmpApprox(this float a, float b, float epsilon = NumberF.ComparisonEpsilon)
    {
        return NumberF.CmpApprox(a, b, epsilon);
    }

    public static float Repeat(this float a, float length = Constants.kDegreeFullCircleF)
    {
        return a - MathF.Floor(a / length) * length;
    }

    public static float NormalizeDegree(this float a)
    {
        float num = a.Repeat();
        if (num.GTApprox(Constants.kDegreeHalfCircleF))
        {
            num -= Constants.kDegreeFullCircleF;
        }
        else if (num.LTApprox(-Constants.kDegreeHalfCircleF))
        {
            num += Constants.kDegreeFullCircleF;
        }
        return num;
    }

    public static float DeltaDegree(this float a, float target)
    {
        float num = (target - a).Repeat();
        if (num.GTApprox(Constants.kDegreeHalfCircleF))
        {
            num -= Constants.kDegreeFullCircleF;
        }
        else if (num.LTApprox(-Constants.kDegreeHalfCircleF))
        {
            num += Constants.kDegreeFullCircleF;
        }
        return num;
    }

    public static float UnsignedDeltaDegree(this float a, float target)
    {
        return MathF.Abs(a.DeltaDegree(target));
    }

    public static bool BetweenDegrees(this float a, float start, float end)
    {
        float deltaDegree = (end - start).Repeat();

        if (deltaDegree.EQApprox(0f))
        {
            return true;
        }

        float startToTargetDeltaDegree = (a - start).Repeat();
        if (startToTargetDeltaDegree.IsNegative())
        {
            startToTargetDeltaDegree += Constants.kDegreeFullCircleF;
        }
        if (startToTargetDeltaDegree.EQApprox(0f) || startToTargetDeltaDegree.EQApprox(Constants.kDegreeFullCircleF))
        {
            return true;
        }

        if (deltaDegree.IsNegative())
        {
            deltaDegree += Constants.kDegreeFullCircleF;
        }
        return startToTargetDeltaDegree.LEApprox(deltaDegree);
    }

    public static float InverseLerp(this float value, float start, float end)
    {
        return Math.Clamp((value - start) / (end - start), 0f, 1f);
    }

    #region extension equivalent to float static methods
    public static bool IsFinite(this float d) { return float.IsFinite(d); }
    public static bool IsInfinity(this float d) { return float.IsInfinity(d); }
    public static bool IsNaN(this float d) { return float.IsNaN(d); }
    public static bool IsNegative(this float d) { return float.IsNegative(d); }
    public static bool IsNegativeInfinity(this float d) { return float.IsNegativeInfinity(d); }
    public static bool IsNormal(this float d) { return float.IsNormal(d); }
    public static bool IsPositiveInfinity(this float d) { return float.IsPositiveInfinity(d); }
    public static bool IsSubnormal(this float d) { return float.IsSubnormal(d); }
    #endregion
}
