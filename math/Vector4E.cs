using System.Numerics;
using System.Runtime.InteropServices;

namespace gameplay.math;

[StructLayout(LayoutKind.Sequential)]
public struct Vector4E : IEquatable<Vector4E>, IEquatable<Vector4>, IFormattable, IEquatableApprox<Vector4E, float>, IEquatableApprox<Vector4, float>
{
    public Vector4 Value;
    public Vector4E(Vector4 value)
    {
        Value = value;
    }
    public Vector4E(float value)
    {
        Value = new Vector4(value);
    }
    public Vector4E(Vector3E value, float w)
    {
        Value = new Vector4(value, w);
    }
    public readonly void Deconstruct(out Vector3E v3, out float w)
    {
        v3 = new Vector3E(Value.X, Value.Y, Value.Z);
        w = Value.W;
    }
    public static implicit operator Vector4E((Vector3E value, float w) tuple)
    {
        return new Vector4E(tuple.value, tuple.w);
    }
    public Vector4E(Vector2E value, float z, float w)
    {
        Value = new Vector4(value, z, w);
    }
    public readonly void Deconstruct(out Vector2E v2, out float z, out float w)
    {
        v2 = new Vector2E(Value.X, Value.Y);
        z = Value.Z;
        w = Value.W;
    }
    public static implicit operator Vector4E((Vector2E value, float z, float w) tuple)
    {
        return new Vector4E(tuple.value, tuple.z, tuple.w);
    }
    public Vector4E(float x, float y, float z, float w)
    {
        Value = new Vector4(x, y, z, w);
    }
    public readonly void Deconstruct(out float x, out float y, out float z, out float w)
    {
        x = Value.X;
        y = Value.Y;
        z = Value.Z;
        w = Value.W;
    }
    public static implicit operator Vector4E((float x, float y, float z, float w) tuple)
    {
        return new Vector4E(tuple.x, tuple.y, tuple.z, tuple.w);
    }
    public static implicit operator Vector4E(Vector4 value) { return new Vector4E(value); }
    public static implicit operator Vector4(Vector4E value) { return value.Value; }
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
    public float Z
    {
        readonly get => Value.Z;
        set => Value.Z = value;
    }
    public float W
    {
        readonly get => Value.W;
        set => Value.W = value;
    }

    public static bool EQApprox(Vector4 v1, Vector4 v2, float epsilon = NumberF.ComparisonEpsilon)
    {
        return NumberF.EQApprox(v1.X, v2.X, epsilon) && NumberF.EQApprox(v1.Y, v2.Y, epsilon) && NumberF.EQApprox(v1.Z, v2.Z, epsilon) && NumberF.EQApprox(v1.W, v2.W, epsilon);
    }
    public static bool NEApprox(Vector4 v1, Vector4 v2, float epsilon = NumberF.ComparisonEpsilon)
    {
        return NumberF.NEApprox(v1.X, v2.X, epsilon) || NumberF.NEApprox(v1.Y, v2.Y, epsilon) || NumberF.NEApprox(v1.Z, v2.Z, epsilon) || NumberF.NEApprox(v1.W, v2.W, epsilon);
    }
    public readonly bool EQApprox(Vector4 other, float epsilon = NumberF.ComparisonEpsilon)
    {
        return EQApprox(Value, other, epsilon);
    }
    public readonly bool NEApprox(Vector4 other, float epsilon = NumberF.ComparisonEpsilon)
    {
        return NEApprox(Value, other, epsilon);
    }
    public readonly bool EQApprox(Vector4E other, float epsilon = NumberF.ComparisonEpsilon)
    {
        return EQApprox(Value, other.Value, epsilon);
    }
    public readonly bool NEApprox(Vector4E other, float epsilon = NumberF.ComparisonEpsilon)
    {
        return NEApprox(Value, other.Value, epsilon);
    }

    public readonly override int GetHashCode()
    {
        return HashCode.Combine(MathF.Round(Value.X, 4), MathF.Round(Value.Y, 4), MathF.Round(Value.Z, 4), MathF.Round(Value.W, 4));
    }
    public readonly override bool Equals(object? obj)
    {
        return obj switch
        {
            Vector4E ve => EQApprox(Value, ve.Value),
            Vector4 v => EQApprox(Value, v),
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
    public readonly bool Equals(Vector4E other)
    {
        return EQApprox(Value, other.Value);
    }
    public readonly bool Equals(Vector4 other)
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
            Vector4E ve => EQApprox(Value, ve.Value, (float)epsilon),
            Vector4 v => EQApprox(Value, v, (float)epsilon),
            _ => false,
        };
    }
    readonly bool IEquatableApprox.NEApprox(object other, double epsilon)
    {
        return other switch
        {
            Vector4E ve => NEApprox(Value, ve.Value, (float)epsilon),
            Vector4 v => NEApprox(Value, v, (float)epsilon),
            _ => true,
        };
    }
    #endregion

    #region operators
    #region op - equal
    public static bool operator ==(Vector4E left, Vector4E right)
    {
        return EQApprox(left.Value, right.Value);
    }
    public static bool operator !=(Vector4E left, Vector4E right)
    {
        return NEApprox(left.Value, right.Value);
    }
    public static bool operator ==(Vector4E left, Vector4 right)
    {
        return EQApprox(left.Value, right);
    }
    public static bool operator !=(Vector4E left, Vector4 right)
    {
        return NEApprox(left.Value, right);
    }
    public static bool operator ==(Vector4 left, Vector4E right)
    {
        return EQApprox(left, right.Value);
    }
    public static bool operator !=(Vector4 left, Vector4E right)
    {
        return NEApprox(left, right.Value);
    }
    #endregion
    #region op - scalar
    public static Vector4E operator +(Vector4E left, Vector4E right)
    {
        return left.Value + right.Value;
    }
    public static Vector4E operator -(Vector4E left, Vector4E right)
    {
        return left.Value - right.Value;
    }
    public static Vector4E operator *(Vector4E left, Vector4E right)
    {
        return left.Value * right.Value;
    }
    public static Vector4E operator /(Vector4E left, Vector4E right)
    {
        return left.Value / right.Value;
    }
    public static Vector4E operator +(Vector4E left, Vector4 right)
    {
        return left.Value + right;
    }
    public static Vector4E operator -(Vector4E left, Vector4 right)
    {
        return left.Value - right;
    }
    public static Vector4E operator *(Vector4E left, Vector4 right)
    {
        return left.Value * right;
    }
    public static Vector4E operator /(Vector4E left, Vector4 right)
    {
        return left.Value / right;
    }
    public static Vector4E operator +(Vector4 left, Vector4E right)
    {
        return left + right.Value;
    }
    public static Vector4E operator -(Vector4 left, Vector4E right)
    {
        return left - right.Value;
    }
    public static Vector4E operator *(Vector4 left, Vector4E right)
    {
        return left * right.Value;
    }
    public static Vector4E operator /(Vector4 left, Vector4E right)
    {
        return left / right.Value;
    }
    public static Vector4E operator *(Vector4E left, float right)
    {
        return left.Value * right;
    }
    public static Vector4E operator *(float left, Vector4E right)
    {
        return left * right.Value;
    }
    public static Vector4E operator /(Vector4E left, float right)
    {
        return left.Value / right;
    }
    public static Vector4E operator -(Vector4E value)
    {
        return -value.Value;
    }
    public static Vector4E operator +(Vector4E value)
    {
        return value;
    }
    #endregion
    #region vector cross(&) and dot(|)
    public static float operator |(Vector4E left, Vector4E right)
    {
        return left.Dot(right);
    }
    #endregion
    #region op - convert
    public static implicit operator Vector4E(Vector2 v)
    {
        return new Vector4E(v, 0f, 0f);
    }
    public static implicit operator Vector4E(Vector2E v)
    {
        return new Vector4E(v.Value, 0f, 0f);
    }
    public static implicit operator Vector2(Vector4E v)
    {
        return new Vector2(v.X, v.Y);
    }
    public static implicit operator Vector2E(Vector4E v)
    {
        return new Vector2E(v.X, v.Y);
    }
    public static implicit operator Vector4E(Vector3 v)
    {
        return new Vector4E(v, 0f);
    }
    public static implicit operator Vector4E(Vector3E v)
    {
        return new Vector4E(v.Value, 0f);
    }
    public static implicit operator Vector3(Vector4E v)
    {
        return new Vector3(v.X, v.Y, v.Z);
    }
    public static implicit operator Vector3E(Vector4E v)
    {
        return new Vector3E(v.X, v.Y, v.Z);
    }
    #endregion
    #endregion

    #region Create Vector with W = 1
    public static Vector4E Point(Vector2 v)
    {
        return new Vector4E(v, 0f, 1f);
    }
    public static Vector4E Point(Vector2E v)
    {
        return new Vector4E(v.Value, 0f, 1f);
    }
    public static Vector4E Point(Vector3 v)
    {
        return new Vector4E(v, 1f);
    }
    public static Vector4E Point(Vector3E v)
    {
        return new Vector4E(v.Value, 1f);
    }
    #endregion

    #region Consts
    public static readonly Vector4E UnitX = new Vector4E(1f, 0f, 0f, 0f);
    public static readonly Vector4E UnitY = new Vector4E(0f, 1f, 0f, 0f);
    public static readonly Vector4E UnitZ = new Vector4E(0f, 0f, 1f, 0f);
    public static readonly Vector4E UnitW = new Vector4E(0f, 0f, 0f, 1f);
    public static readonly Vector4E Zero = new Vector4E(0f, 0f, 0f, 0f);
    public static readonly Vector4E One = new Vector4E(1f, 1f, 1f, 1f);
    // Extra Consts
    public static readonly Vector4E Forward = UnitX;
    public static readonly Vector4E Up = UnitZ;
    #endregion

    #region statics from .net Vector4
    public static Vector4E Abs(Vector4E value)
    {
        return Vector4.Abs(value.Value);
    }
    public static Vector4E Add(Vector4E left, Vector4E right)
    {
        return Vector4.Add(left.Value, right.Value);
    }
    public static Vector4E Clamp(Vector4E value1, Vector4E min, Vector4E max)
    {
        return Vector4.Clamp(value1.Value, min.Value, max.Value);
    }
    public static float Distance(Vector4E value1, Vector4E value2)
    {
        return Vector4.Distance(value1.Value, value2.Value);
    }
    public static float DistanceSquared(Vector4E value1, Vector4E value2)
    {
        return Vector4.DistanceSquared(value1.Value, value2.Value);
    }
    public static Vector4E Divide(Vector4E left, Vector4E right)
    {
        return Vector4.Divide(left.Value, right.Value);
    }
    public static Vector4E Divide(Vector4E left, float divisor)
    {
        return Vector4.Divide(left.Value, divisor);
    }
    public static float Dot(Vector4E vector1, Vector4E vector2)
    {
        return Vector4.Dot(vector1.Value, vector2.Value);
    }
    public static Vector4E Lerp(Vector4E value1, Vector4E value2, float amount)
    {
        return Vector4.Lerp(value1.Value, value2.Value, amount);
    }
    public static Vector4E Max(Vector4E value1, Vector4E value2)
    {
        return Vector4.Max(value1.Value, value2.Value);
    }
    public static Vector4E Min(Vector4E value1, Vector4E value2)
    {
        return Vector4.Min(value1.Value, value2.Value);
    }
    public static Vector4E Multiply(Vector4E left, float right)
    {
        return Vector4.Multiply(left.Value, right);
    }
    public static Vector4E Multiply(float left, Vector4E right)
    {
        return Vector4.Multiply(left, right.Value);
    }
    public static Vector4E Multiply(Vector4E left, Vector4E right)
    {
        return Vector4.Multiply(left.Value, right.Value);
    }
    public static Vector4E Negate(Vector4E value)
    {
        return Vector4.Negate(value.Value);
    }
    public static Vector4E Normalize(Vector4E vector)
    {
        return Vector4.Normalize(vector.Value);
    }
    public static Vector4E SquareRoot(Vector4E value)
    {
        return Vector4.SquareRoot(value.Value);
    }
    public static Vector4E Subtract(Vector4E left, Vector4E right)
    {
        return Vector4.Subtract(left.Value, right.Value);
    }

    #region instance equivalent to static methods
    public readonly Vector4E Abs()
    {
        return Vector4.Abs(Value);
    }
    public readonly Vector4E Clamp(Vector4E min, Vector4E max)
    {
        return Vector4.Clamp(Value, min.Value, max.Value);
    }
    public readonly float Distance(Vector4E value2)
    {
        return Vector4.Distance(Value, value2.Value);
    }
    public readonly float DistanceSquared(Vector4E value2)
    {
        return Vector4.DistanceSquared(Value, value2.Value);
    }
    public readonly float Dot(Vector4E vector2)
    {
        return Vector4.Dot(Value, vector2.Value);
    }
    public readonly Vector4E Lerp(Vector4E value2, float amount)
    {
        return Vector4.Lerp(Value, value2.Value, amount);
    }
    public readonly Vector4E Negate()
    {
        return Vector4.Negate(Value);
    }
    public readonly Vector4E Normalize()
    {
        return Vector4.Normalize(Value);
    }
    public readonly Vector4E SquareRoot()
    {
        return Vector4.SquareRoot(Value);
    }
    #endregion

    // Transform
    public static Vector4E Transform(Vector4E value, QuaternionE rotation)
    {
        return Vector4.Transform(value.Value, rotation.Value);
    }
    public static Vector4E Transform(Vector2E value, QuaternionE rotation)
    {
        return Vector4.Transform(value.Value, rotation.Value);
    }
    public static Vector4E Transform(Vector3E value, QuaternionE rotation)
    {
        return Vector4.Transform(value.Value, rotation.Value);
    }
    public static Vector4E Transform(Vector4E value, Matrix4x4E matrix)
    {
        return Vector4.Transform(value.Value, matrix.Value);
    }
    public static Vector4E Transform(Vector2E value, Matrix4x4E matrix)
    {
        return Vector4.Transform(value.Value, matrix.Value);
    }
    public static Vector4E Transform(Vector3E value, Matrix4x4E matrix)
    {
        return Vector4.Transform(value.Value, matrix.Value);
    }
    public static Vector4E Transform(Vector4E value, Matrix4x4T matrix)
    {
        return Vector4.Transform(value.Value, (Matrix4x4)matrix);
    }
    public static Vector4E Transform(Vector2E value, Matrix4x4T matrix)
    {
        return Vector4.Transform(value.Value, (Matrix4x4)matrix);
    }
    public static Vector4E Transform(Vector3E value, Matrix4x4T matrix)
    {
        return Vector4.Transform(value.Value, (Matrix4x4)matrix);
    }
    // instance Transform
    public readonly Vector4E Transform(QuaternionE rotation)
    {
        return Vector4.Transform(Value, rotation.Value);
    }
    public readonly Vector4E Transform(Matrix4x4E matrix)
    {
        return Vector4.Transform(Value, matrix.Value);
    }
    public readonly Vector4E Transform(Matrix4x4T matrix)
    {
        return Vector4.Transform(Value, (Matrix4x4)matrix);
    }
    #endregion

    #region index
    private delegate ref float GetElementRefDel(ref Vector4 vec);
    private static GetElementRefDel[] _GetElementRefFuncs = new GetElementRefDel[4]
    {
        GetElementRef0,
        GetElementRef1,
        GetElementRef2,
        GetElementRef3,
    };
    private static ref float GetElementRef0(ref Vector4 vec)
    {
        return ref vec.X;
    }
    private static ref float GetElementRef1(ref Vector4 vec)
    {
        return ref vec.Y;
    }
    private static ref float GetElementRef2(ref Vector4 vec)
    {
        return ref vec.Z;
    }
    private static ref float GetElementRef3(ref Vector4 vec)
    {
        return ref vec.W;
    }
    public static float GetElementAt(Vector4 vec, int index)
    {
        return _GetElementRefFuncs[index](ref vec);
    }
    public static void SetElementAt(ref Vector4 vec, int index, float value)
    {
        _GetElementRefFuncs[index](ref vec) = value;
    }
    public readonly float GetElementAt(int index)
    {
        return GetElementAt(Value, index);
    }
    public readonly Vector4E WithElement(int index, float value)
    {
        Vector4 v = Value;
        SetElementAt(ref v, index, value);
        return v;
    }
    public float this[int index]
    {
        readonly get => GetElementAt(Value, index);
        set => SetElementAt(ref Value, index, value);
    }
    public readonly Vector4E this[int index, float newvalue] => WithElement(index, newvalue);

    public readonly Vector4E WithX(float value)
    {
        return Value with { X = value };
    }
    public readonly Vector4E WithY(float value)
    {
        return Value with { Y = value };
    }
    public readonly Vector4E WithZ(float value)
    {
        return Value with { Z = value };
    }
    public readonly Vector4E WithW(float value)
    {
        return Value with { W = value };
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
    #endregion
}

public static class Vector4Extensions
{
    public static bool EQApprox(this Vector4 a, Vector4 b, float epsilon = NumberF.ComparisonEpsilon)
    {
        return Vector4E.EQApprox(a, b, epsilon);
    }
    public static bool NEApprox(this Vector4 a, Vector4 b, float epsilon = NumberF.ComparisonEpsilon)
    {
        return Vector4E.NEApprox(a, b, epsilon);
    }

    #region extension equivalent to .net Vector4 static methods
    public static Vector4 Abs(this Vector4 value)
    {
        return Vector4.Abs(value);
    }
    public static Vector4 Clamp(this Vector4 value1, Vector4 min, Vector4 max)
    {
        return Vector4.Clamp(value1, min, max);
    }
    public static float Distance(this Vector4 value1, Vector4 value2)
    {
        return Vector4.Distance(value1, value2);
    }
    public static float DistanceSquared(this Vector4 value1, Vector4 value2)
    {
        return Vector4.DistanceSquared(value1, value2);
    }
    public static float Dot(this Vector4 vector1, Vector4 vector2)
    {
        return Vector4.Dot(vector1, vector2);
    }
    public static Vector4 Lerp(this Vector4 value1, Vector4 value2, float amount)
    {
        return Vector4.Lerp(value1, value2, amount);
    }
    public static Vector4 Negate(this Vector4 value)
    {
        return Vector4.Negate(value);
    }
    public static Vector4 Normalize(this Vector4 vector)
    {
        return Vector4.Normalize(vector);
    }
    public static Vector4 SquareRoot(this Vector4 value)
    {
        return Vector4.SquareRoot(value);
    }
    #endregion

    #region Deconstructors
    public static void Deconstruct(this Vector4 value, out Vector3 v3, out float w)
    {
        v3 = new Vector3(value.X, value.Y, value.Z);
        w = value.W;
    }
    public static void Deconstruct(this Vector4 value, out Vector2 v2, out float z, out float w)
    {
        v2 = new Vector2(value.X, value.Y);
        z = value.Z;
        w = value.W;
    }
    public static void Deconstruct(this Vector4 value, out float x, out float y, out float z, out float w)
    {
        x = value.X;
        y = value.Y;
        z = value.Z;
        w = value.W;
    }
    #endregion

    #region index
    public static float GetElementAt(this Vector4 vec, int index)
    {
        return Vector4E.GetElementAt(vec, index);
    }
    public static void SetElementAt(this ref Vector4 vec, int index, float value)
    {
        Vector4E.SetElementAt(ref vec, index, value);
    }
    public static Vector4 WithElement(this Vector4 vec, int index, float value)
    {
        Vector4E.SetElementAt(ref vec, index, value);
        return vec;
    }
    #endregion
}
