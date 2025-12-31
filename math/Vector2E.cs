using System.Numerics;
using System.Runtime.InteropServices;

namespace gameplay.math;

[StructLayout(LayoutKind.Sequential)]
public struct Vector2E : IEquatable<Vector2E>, IEquatable<Vector2>, IFormattable, IEquatableApprox<Vector2E, float>, IEquatableApprox<Vector2, float>
{
    public Vector2 Value;
    public Vector2E(Vector2 value)
    {
        Value = value;
    }
    public Vector2E(float value) : this(value, value)
    {
    }
    public Vector2E(float x, float y)
    {
        Value = new Vector2(x, y);
    }
    public readonly void Deconstruct(out float x, out float y)
    {
        x = Value.X;
        y = Value.Y;
    }
    public static implicit operator Vector2E((float x, float y) tuple)
    {
        return new Vector2E(tuple.x, tuple.y);
    }

    public static implicit operator Vector2E(Vector2 value) { return new Vector2E(value); }
    public static implicit operator Vector2(Vector2E value) { return value.Value; }
    public float X
    {
        readonly get => Value.X;
        set => Value.X = value;
    }
    public float Y
    {
        readonly get => Value.Y;
        set => Value.Y = value;
    }

    public static bool EQApprox(Vector2 v1, Vector2 v2, float epsilon = NumberF.ComparisonEpsilon)
    {
        return NumberF.EQApprox(v1.X, v2.X, epsilon) && NumberF.EQApprox(v1.Y, v2.Y, epsilon);
    }
    public static bool NEApprox(Vector2 v1, Vector2 v2, float epsilon = NumberF.ComparisonEpsilon)
    {
        return NumberF.NEApprox(v1.X, v2.X, epsilon) || NumberF.NEApprox(v1.Y, v2.Y, epsilon);
    }
    public readonly bool EQApprox(Vector2 other, float epsilon = NumberF.ComparisonEpsilon)
    {
        return EQApprox(Value, other, epsilon);
    }
    public readonly bool NEApprox(Vector2 other, float epsilon = NumberF.ComparisonEpsilon)
    {
        return NEApprox(Value, other, epsilon);
    }
    public readonly bool EQApprox(Vector2E other, float epsilon = NumberF.ComparisonEpsilon)
    {
        return EQApprox(Value, other.Value, epsilon);
    }
    public readonly bool NEApprox(Vector2E other, float epsilon = NumberF.ComparisonEpsilon)
    {
        return NEApprox(Value, other.Value, epsilon);
    }

    public readonly override int GetHashCode()
    {
        return HashCode.Combine(MathF.Round(Value.X, 4), MathF.Round(Value.Y, 4));
    }
    public readonly override bool Equals(object? obj)
    {
        return obj switch
        {
            Vector2E ve => EQApprox(Value, ve.Value),
            Vector2 v => EQApprox(Value, v),
            _ => false,
        };
    }
    public readonly override string ToString()
    {
        return Value.ToString();
    }
    public readonly void CopyTo(float[] array, int index)
    {
        Value.CopyTo(array, index);
    }
    public readonly void CopyTo(float[] array)
    {
        Value.CopyTo(array);
    }

    #region IEquatable<T>
    public readonly bool Equals(Vector2E other)
    {
        return EQApprox(Value, other.Value);
    }
    public readonly bool Equals(Vector2 other)
    {
        return EQApprox(Value, other);
    }
    #endregion

    #region IFormattable
    public readonly string ToString(string? format)
    {
        return Value.ToString(format);
    }
    public readonly string ToString(string? format, IFormatProvider? formatProvider)
    {
        return Value.ToString(format, formatProvider);
    }
    #endregion

    #region IEquatableApprox
    readonly bool IEquatableApprox.EQApprox(object other, double epsilon)
    {
        return other switch
        {
            Vector2E ve => EQApprox(Value, ve.Value, (float)epsilon),
            Vector2 v => EQApprox(Value, v, (float)epsilon),
            _ => false,
        };
    }
    readonly bool IEquatableApprox.NEApprox(object other, double epsilon)
    {
        return other switch
        {
            Vector2E ve => NEApprox(Value, ve.Value, (float)epsilon),
            Vector2 v => NEApprox(Value, v, (float)epsilon),
            _ => true,
        };
    }
    #endregion

    #region operators
    #region op - equal
    public static bool operator ==(Vector2E left, Vector2E right)
    {
        return EQApprox(left.Value, right.Value);
    }
    public static bool operator !=(Vector2E left, Vector2E right)
    {
        return NEApprox(left.Value, right.Value);
    }
    public static bool operator ==(Vector2E left, Vector2 right)
    {
        return EQApprox(left.Value, right);
    }
    public static bool operator !=(Vector2E left, Vector2 right)
    {
        return NEApprox(left.Value, right);
    }
    public static bool operator ==(Vector2 left, Vector2E right)
    {
        return EQApprox(left, right.Value);
    }
    public static bool operator !=(Vector2 left, Vector2E right)
    {
        return NEApprox(left, right.Value);
    }
    #endregion
    #region op - scalar
    public static Vector2E operator +(Vector2E left, Vector2E right)
    {
        return left.Value + right.Value;
    }
    public static Vector2E operator -(Vector2E left, Vector2E right)
    {
        return left.Value - right.Value;
    }
    public static Vector2E operator *(Vector2E left, Vector2E right)
    {
        return left.Value * right.Value;
    }
    public static Vector2E operator /(Vector2E left, Vector2E right)
    {
        return left.Value / right.Value;
    }
    public static Vector2E operator +(Vector2E left, Vector2 right)
    {
        return left.Value + right;
    }
    public static Vector2E operator -(Vector2E left, Vector2 right)
    {
        return left.Value - right;
    }
    public static Vector2E operator *(Vector2E left, Vector2 right)
    {
        return left.Value * right;
    }
    public static Vector2E operator /(Vector2E left, Vector2 right)
    {
        return left.Value / right;
    }
    public static Vector2E operator +(Vector2 left, Vector2E right)
    {
        return left + right.Value;
    }
    public static Vector2E operator -(Vector2 left, Vector2E right)
    {
        return left - right.Value;
    }
    public static Vector2E operator *(Vector2 left, Vector2E right)
    {
        return left * right.Value;
    }
    public static Vector2E operator /(Vector2 left, Vector2E right)
    {
        return left / right.Value;
    }
    public static Vector2E operator *(Vector2E left, float right)
    {
        return left.Value * right;
    }
    public static Vector2E operator *(float left, Vector2E right)
    {
        return left * right.Value;
    }
    public static Vector2E operator /(Vector2E left, float right)
    {
        return left.Value / right;
    }
    public static Vector2E operator -(Vector2E value)
    {
        return -value.Value;
    }
    public static Vector2E operator +(Vector2E value)
    {
        return value;
    }
    #endregion
    #region vector cross(&) and dot(|)
    public static float operator |(Vector2E left, Vector2E right)
    {
        return left.Dot(right);
    }
    public static float operator &(Vector2E left, Vector2E right)
    {
        return left.Cross(right);
    }
    #endregion
    #region op - convert
    public static implicit operator Vector4(Vector2E v)
    {
        return new Vector4(v.Value, 0f, 0f);
    }
    public static implicit operator Vector2E(Vector4 v)
    {
        return new Vector2E(v.X, v.Y);
    }
    public static implicit operator Vector3(Vector2E v)
    {
        return new Vector3(v.Value, 0f);
    }
    public static implicit operator Vector2E(Vector3 v)
    {
        return new Vector2E(v.X, v.Y);
    }
    #endregion
    #endregion

    #region Consts
    public static readonly Vector2E UnitX = new Vector2E(1f, 0f);
    public static readonly Vector2E UnitY = new Vector2E(0f, 1f);
    public static readonly Vector2E Zero = new Vector2E(0f, 0f);
    public static readonly Vector2E One = new Vector2E(1f, 1f);
    // Extra Consts
    public static readonly Vector2E Forward = UnitX;
    #endregion

    #region statics from .net Vector2
    public static Vector2E Abs(Vector2E value)
    {
        return Vector2.Abs(value.Value);
    }
    public static Vector2E Add(Vector2E left, Vector2E right)
    {
        return Vector2.Add(left.Value, right.Value);
    }
    public static Vector2E Clamp(Vector2E value1, Vector2E min, Vector2E max)
    {
        return Vector2.Clamp(value1.Value, min.Value, max.Value);
    }
    public static float Distance(Vector2E value1, Vector2E value2)
    {
        return Vector2.Distance(value1.Value, value2.Value);
    }
    public static float DistanceSquared(Vector2E value1, Vector2E value2)
    {
        return Vector2.DistanceSquared(value1.Value, value2.Value);
    }
    public static Vector2E Divide(Vector2E left, Vector2E right)
    {
        return Vector2.Divide(left.Value, right.Value);
    }
    public static Vector2E Divide(Vector2E left, float divisor)
    {
        return Vector2.Divide(left.Value, divisor);
    }
    public static float Dot(Vector2E value1, Vector2E value2)
    {
        return Vector2.Dot(value1.Value, value2.Value);
    }
    public static Vector2E Lerp(Vector2E value1, Vector2E value2, float amount)
    {
        return Vector2.Lerp(value1.Value, value2.Value, amount);
    }
    public static Vector2E Max(Vector2E value1, Vector2E value2)
    {
        return Vector2.Max(value1.Value, value2.Value);
    }
    public static Vector2E Min(Vector2E value1, Vector2E value2)
    {
        return Vector2.Min(value1.Value, value2.Value);
    }
    public static Vector2E Multiply(float left, Vector2E right)
    {
        return Vector2.Multiply(left, right.Value);
    }
    public static Vector2E Multiply(Vector2E left, float right)
    {
        return Vector2.Multiply(left.Value, right);
    }
    public static Vector2E Multiply(Vector2E left, Vector2E right)
    {
        return Vector2.Multiply(left.Value, right.Value);
    }
    public static Vector2E Negate(Vector2E value)
    {
        return Vector2.Negate(value.Value);
    }
    public static Vector2E Normalize(Vector2E value)
    {
        return Vector2.Normalize(value.Value);
    }
    public static Vector2E Reflect(Vector2E vector, Vector2E normal)
    {
        return Vector2.Reflect(vector.Value, normal.Value);
    }
    public static Vector2E SquareRoot(Vector2E value)
    {
        return Vector2.SquareRoot(value.Value);
    }
    public static Vector2E Subtract(Vector2E left, Vector2E right)
    {
        return Vector2.Subtract(left.Value, right.Value);
    }

    #region instance equivalent to static methods
    public readonly Vector2E Abs()
    {
        return Vector2.Abs(Value);
    }
    public readonly Vector2E Clamp(Vector2E min, Vector2E max)
    {
        return Vector2.Clamp(Value, min.Value, max.Value);
    }
    public readonly float Distance(Vector2E value2)
    {
        return Vector2.Distance(Value, value2.Value);
    }
    public readonly float DistanceSquared(Vector2E value2)
    {
        return Vector2.DistanceSquared(Value, value2.Value);
    }
    public readonly float Dot(Vector2E value2)
    {
        return Vector2.Dot(Value, value2.Value);
    }
    public readonly Vector2E Lerp(Vector2E value2, float amount)
    {
        return Vector2.Lerp(Value, value2.Value, amount);
    }
    public readonly Vector2E Negate()
    {
        return Vector2.Negate(Value);
    }
    public readonly Vector2E Normalize()
    {
        return Vector2.Normalize(Value);
    }
    public readonly Vector2E Reflect(Vector2E normal)
    {
        return Vector2.Reflect(Value, normal.Value);
    }
    public readonly Vector2E SquareRoot()
    {
        return Vector2.SquareRoot(Value);
    }
    #endregion

    // Transform
    public static Vector2E Transform(Vector2E value, QuaternionE rotation)
    {
        return Vector2.Transform(value.Value, rotation.Value);
    }
    public static Vector4E TransformPoint(Vector2E value, QuaternionE rotation)
    {
        return Vector4.Transform(value.Value, rotation.Value);
    }
    public static Vector2E Transform(Vector2E position, Matrix4x4E matrix)
    {
        return Vector2.Transform(position.Value, matrix.Value);
    }
    public static Vector2E TransformNormal(Vector2E position, Matrix4x4E matrix)
    {
        return Vector2.TransformNormal(position.Value, matrix.Value);
    }
    public static Vector4E TransformPoint(Vector2E value, Matrix4x4E matrix)
    {
        return Vector4.Transform(value.Value, matrix.Value);
    }
    public static Vector2E Transform(Vector2E position, Matrix4x4T matrix)
    {
        return Vector2.Transform(position.Value, (Matrix4x4)matrix);
    }
    public static Vector2E TransformNormal(Vector2E position, Matrix4x4T matrix)
    {
        return Vector2.TransformNormal(position.Value, (Matrix4x4)matrix);
    }
    public static Vector4E TransformPoint(Vector2E value, Matrix4x4T matrix)
    {
        return Vector4.Transform(value.Value, (Matrix4x4)matrix);
    }
    // instance Transform
    public readonly Vector2E Transform(QuaternionE rotation)
    {
        return Vector2.Transform(Value, rotation.Value);
    }
    public readonly Vector4E TransformPoint(QuaternionE rotation)
    {
        return Vector4.Transform(Value, rotation.Value);
    }
    public readonly Vector2E Transform(Matrix4x4E matrix)
    {
        return Vector2.Transform(Value, matrix.Value);
    }
    public readonly Vector2E TransformNormal(Matrix4x4E matrix)
    {
        return Vector2.TransformNormal(Value, matrix.Value);
    }
    public readonly Vector4E TransformPoint(Matrix4x4E matrix)
    {
        return Vector4.Transform(Value, matrix.Value);
    }
    public readonly Vector2E Transform(Matrix4x4T matrix)
    {
        return Vector2.Transform(Value, (Matrix4x4)matrix);
    }
    public readonly Vector2E TransformNormal(Matrix4x4T matrix)
    {
        return Vector2.TransformNormal(Value, (Matrix4x4)matrix);
    }
    public readonly Vector4E TransformPoint(Matrix4x4T matrix)
    {
        return Vector4.Transform(Value, (Matrix4x4)matrix);
    }
    #endregion

    #region index
    private delegate ref float GetElementRefDel(ref Vector2 vec);
    private static GetElementRefDel[] _GetElementRefFuncs = new GetElementRefDel[2]
    {
        GetElementRef0,
        GetElementRef1,
    };
    private static ref float GetElementRef0(ref Vector2 vec)
    {
        return ref vec.X;
    }
    private static ref float GetElementRef1(ref Vector2 vec)
    {
        return ref vec.Y;
    }
    public static float GetElementAt(Vector2 vec, int index)
    {
        return _GetElementRefFuncs[index](ref vec);
    }
    public static void SetElementAt(ref Vector2 vec, int index, float value)
    {
        _GetElementRefFuncs[index](ref vec) = value;
    }
    public readonly float GetElementAt(int index)
    {
        return GetElementAt(Value, index);
    }
    public readonly Vector2E WithElement(int index, float value)
    {
        Vector2 v = Value;
        SetElementAt(ref v, index, value);
        return v;
    }
    public float this[int index]
    {
        readonly get => GetElementAt(Value, index);
        set => SetElementAt(ref Value, index, value);
    }
    public readonly Vector2E this[int index, float newvalue] => WithElement(index, newvalue);

    public readonly Vector2E WithX(float value)
    {
        return Value with { X = value };
    }
    public readonly Vector2E WithY(float value)
    {
        return Value with { Y = value };
    }
    #endregion

    #region Arithmetic Methods
    public readonly float Length()
    {
        return Value.Length();
    }
    public readonly float LengthSquared()
    {
        return Value.LengthSquared();
    }
    // Extra Methods
    public readonly float AngleRadians()
    {
        return Value.AngleRadians();
    }

    public readonly float AngleDegrees()
    {
        return Value.AngleDegrees();
    }

    public readonly Vector2E Rotate(float radians)
    {
        return Value.Rotate(radians);
    }
    public static float Cross(Vector2E left, Vector2E right)
    {
        return Vector2Extensions.Cross(left.Value, right.Value);
    }
    public static float AngleRadians(Vector2E from, Vector2E to)
    {
        return Vector2Extensions.AngleRadians(from.Value, to.Value);
    }
    public readonly float Cross(Vector2E right)
    {
        return Vector2Extensions.Cross(Value, right.Value);
    }
    public readonly float AngleRadians(Vector2E to)
    {
        return Vector2Extensions.AngleRadians(Value, to.Value);
    }

    public static float AngleDegrees(Vector2E from, Vector2E to)
    {
        return Vector2Extensions.AngleDegrees(from.Value, to.Value);
    }

    public readonly float AngleDegrees(Vector2E to)
    {
        return Vector2Extensions.AngleDegrees(Value, to.Value);
    }
    #endregion
}

public static class Vector2Extensions
{
    public static bool EQApprox(this Vector2 a, Vector2 b, float epsilon = NumberF.ComparisonEpsilon)
    {
        return Vector2E.EQApprox(a, b, epsilon);
    }
    public static bool NEApprox(this Vector2 a, Vector2 b, float epsilon = NumberF.ComparisonEpsilon)
    {
        return Vector2E.NEApprox(a, b, epsilon);
    }

    public static float AngleRadians(this Vector2 direction)
    {
        return MathF.Atan2(direction.Y, direction.X);
    }

    public static float AngleDegrees(this Vector2 direction)
    {
        return direction.AngleRadians() * Constants.kRadianToDegreeF;
    }

    public static Vector2 Rotate(this Vector2 vec, float radians)
    {
        float sa = MathF.Sin(radians);
        float ca = MathF.Cos(radians);
        return new Vector2(vec.X * ca - vec.Y * sa, vec.X * sa + vec.Y * ca);
    }
    public static float Cross(this Vector2 left, Vector2 right)
    {
        return left.X * right.Y - left.Y * right.X;
    }
    public static float AngleRadians(this Vector2 from, Vector2 to)
    {
        float cross = Cross(from, to);
        return MathF.Asin(Math.Clamp(cross / (from.Length() * to.Length()), -1, 1f));
    }

    public static float AngleDegrees(this Vector2 from, Vector2 to)
    {
        return AngleRadians(from, to) * Constants.kRadianToDegreeF;
    }

    #region extension equivalent to .net Vector2 static methods
    public static Vector2 Abs(this Vector2 value)
    {
        return Vector2.Abs(value);
    }
    public static Vector2 Clamp(this Vector2 value1, Vector2 min, Vector2 max)
    {
        return Vector2.Clamp(value1, min, max);
    }
    public static float Distance(this Vector2 value1, Vector2 value2)
    {
        return Vector2.Distance(value1, value2);
    }
    public static float DistanceSquared(this Vector2 value1, Vector2 value2)
    {
        return Vector2.DistanceSquared(value1, value2);
    }
    public static float Dot(this Vector2 value1, Vector2 value2)
    {
        return Vector2.Dot(value1, value2);
    }
    public static Vector2 Lerp(this Vector2 value1, Vector2 value2, float amount)
    {
        return Vector2.Lerp(value1, value2, amount);
    }
    public static Vector2 Negate(this Vector2 value)
    {
        return Vector2.Negate(value);
    }
    public static Vector2 Normalize(this Vector2 value)
    {
        return Vector2.Normalize(value);
    }
    public static Vector2 Reflect(this Vector2 vector, Vector2 normal)
    {
        return Vector2.Reflect(vector, normal);
    }
    public static Vector2 SquareRoot(this Vector2 value)
    {
        return Vector2.SquareRoot(value);
    }
    #endregion

    #region Deconstructors
    public static void Deconstruct(this Vector2 value, out float x, out float y)
    {
        x = value.X;
        y = value.Y;
    }
    #endregion

    #region index
    public static float GetElementAt(this Vector2 vec, int index)
    {
        return Vector2E.GetElementAt(vec, index);
    }
    public static void SetElementAt(this ref Vector2 vec, int index, float value)
    {
        Vector2E.SetElementAt(ref vec, index, value);
    }
    public static Vector2 WithElement(this Vector2 vec, int index, float value)
    {
        Vector2E.SetElementAt(ref vec, index, value);
        return vec;
    }
    #endregion
}
