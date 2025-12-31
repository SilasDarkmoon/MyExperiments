using System.Numerics;
using System.Runtime.InteropServices;

namespace gameplay.math;

[StructLayout(LayoutKind.Sequential)]
public struct Vector3E : IEquatable<Vector3E>, IEquatable<Vector3>, IFormattable, IEquatableApprox<Vector3E, float>, IEquatableApprox<Vector3, float>
{
    public Vector3 Value;
    public Vector3E(Vector3 value)
    {
        Value = value;
    }
    public Vector3E(float value)
    {
        Value = new Vector3(value);
    }
    public Vector3E(Vector2E value, float z)
    {
        Value = new Vector3(value, z);
    }
    public readonly void Deconstruct(out Vector2E v2, out float z)
    {
        v2 = new Vector2E(Value.X, Value.Y);
        z = Value.Z;
    }
    public static implicit operator Vector3E((Vector2E value, float z) tuple)
    {
        return new Vector3E(tuple.value, tuple.z);
    }
    public Vector3E(float x, float y, float z)
    {
        Value = new Vector3(x, y, z);
    }
    public readonly void Deconstruct(out float x, out float y, out float z)
    {
        x = Value.X;
        y = Value.Y;
        z = Value.Z;
    }
    public static implicit operator Vector3E((float x, float y, float z) tuple)
    {
        return new Vector3E(tuple.x, tuple.y, tuple.z);
    }

    public static implicit operator Vector3E(Vector3 value) { return new Vector3E(value); }
    public static implicit operator Vector3(Vector3E value) { return value.Value; }
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

    public static bool EQApprox(Vector3 v1, Vector3 v2, float epsilon = NumberF.ComparisonEpsilon)
    {
        return NumberF.EQApprox(v1.X, v2.X, epsilon) && NumberF.EQApprox(v1.Y, v2.Y, epsilon) && NumberF.EQApprox(v1.Z, v2.Z, epsilon);
    }
    public static bool NEApprox(Vector3 v1, Vector3 v2, float epsilon = NumberF.ComparisonEpsilon)
    {
        return NumberF.NEApprox(v1.X, v2.X, epsilon) || NumberF.NEApprox(v1.Y, v2.Y, epsilon) || NumberF.NEApprox(v1.Z, v2.Z, epsilon);
    }
    public readonly bool EQApprox(Vector3 other, float epsilon = NumberF.ComparisonEpsilon)
    {
        return EQApprox(Value, other, epsilon);
    }
    public readonly bool NEApprox(Vector3 other, float epsilon = NumberF.ComparisonEpsilon)
    {
        return NEApprox(Value, other, epsilon);
    }
    public readonly bool EQApprox(Vector3E other, float epsilon = NumberF.ComparisonEpsilon)
    {
        return EQApprox(Value, other.Value, epsilon);
    }
    public readonly bool NEApprox(Vector3E other, float epsilon = NumberF.ComparisonEpsilon)
    {
        return NEApprox(Value, other.Value, epsilon);
    }

    public readonly override int GetHashCode()
    {
        return HashCode.Combine(MathF.Round(Value.X, 4), MathF.Round(Value.Y, 4), MathF.Round(Value.Z, 4));
    }
    public readonly override bool Equals(object? obj)
    {
        return obj switch
        {
            Vector3E ve => EQApprox(Value, ve.Value),
            Vector3 v => EQApprox(Value, v),
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
    public readonly bool Equals(Vector3E other)
    {
        return EQApprox(Value, other.Value);
    }
    public readonly bool Equals(Vector3 other)
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
            Vector3E ve => EQApprox(Value, ve.Value, (float)epsilon),
            Vector3 v => EQApprox(Value, v, (float)epsilon),
            _ => false,
        };
    }
    readonly bool IEquatableApprox.NEApprox(object other, double epsilon)
    {
        return other switch
        {
            Vector3E ve => NEApprox(Value, ve.Value, (float)epsilon),
            Vector3 v => NEApprox(Value, v, (float)epsilon),
            _ => true,
        };
    }
    #endregion

    #region operators
    #region op - equal
    public static bool operator ==(Vector3E left, Vector3E right)
    {
        return EQApprox(left.Value, right.Value);
    }
    public static bool operator !=(Vector3E left, Vector3E right)
    {
        return NEApprox(left.Value, right.Value);
    }
    public static bool operator ==(Vector3E left, Vector3 right)
    {
        return EQApprox(left.Value, right);
    }
    public static bool operator !=(Vector3E left, Vector3 right)
    {
        return NEApprox(left.Value, right);
    }
    public static bool operator ==(Vector3 left, Vector3E right)
    {
        return EQApprox(left, right.Value);
    }
    public static bool operator !=(Vector3 left, Vector3E right)
    {
        return NEApprox(left, right.Value);
    }
    #endregion
    #region op - scalar
    public static Vector3E operator +(Vector3E left, Vector3E right)
    {
        return left.Value + right.Value;
    }
    public static Vector3E operator -(Vector3E left, Vector3E right)
    {
        return left.Value - right.Value;
    }
    public static Vector3E operator *(Vector3E left, Vector3E right)
    {
        return left.Value * right.Value;
    }
    public static Vector3E operator /(Vector3E left, Vector3E right)
    {
        return left.Value / right.Value;
    }
    public static Vector3E operator +(Vector3E left, Vector3 right)
    {
        return left.Value + right;
    }
    public static Vector3E operator -(Vector3E left, Vector3 right)
    {
        return left.Value - right;
    }
    public static Vector3E operator *(Vector3E left, Vector3 right)
    {
        return left.Value * right;
    }
    public static Vector3E operator /(Vector3E left, Vector3 right)
    {
        return left.Value / right;
    }
    public static Vector3E operator +(Vector3 left, Vector3E right)
    {
        return left + right.Value;
    }
    public static Vector3E operator -(Vector3 left, Vector3E right)
    {
        return left - right.Value;
    }
    public static Vector3E operator *(Vector3 left, Vector3E right)
    {
        return left * right.Value;
    }
    public static Vector3E operator /(Vector3 left, Vector3E right)
    {
        return left / right.Value;
    }
    public static Vector3E operator *(Vector3E left, float right)
    {
        return left.Value * right;
    }
    public static Vector3E operator *(float left, Vector3E right)
    {
        return left * right.Value;
    }
    public static Vector3E operator /(Vector3E left, float right)
    {
        return left.Value / right;
    }
    public static Vector3E operator -(Vector3E value)
    {
        return -value.Value;
    }
    public static Vector3E operator +(Vector3E value)
    {
        return value;
    }
    #endregion
    #region vector cross(&) and dot(|)
    public static float operator |(Vector3E left, Vector3E right)
    {
        return left.Dot(right);
    }
    public static Vector3E operator &(Vector3E left, Vector3E right)
    {
        return left.Cross(right);
    }
    #endregion
    #region op - convert
    public static implicit operator Vector3E(Vector2 v)
    {
        return new Vector3E(v, 0f);
    }
    public static implicit operator Vector3E(Vector2E v)
    {
        return new Vector3E(v.Value, 0f);
    }
    public static implicit operator Vector2(Vector3E v)
    {
        return new Vector2(v.X, v.Y);
    }
    public static implicit operator Vector2E(Vector3E v)
    {
        return new Vector2E(v.X, v.Y);
    }
    public static implicit operator Vector4(Vector3E v)
    {
        return new Vector4(v.Value, 0f);
    }
    public static implicit operator Vector3E(Vector4 v)
    {
        return new Vector3E(v.X, v.Y, v.Z);
    }
    #endregion
    #endregion

    #region Consts
    public static readonly Vector3E UnitX = new Vector3E(1f, 0f, 0f);
    public static readonly Vector3E UnitY = new Vector3E(0f, 1f, 0f);
    public static readonly Vector3E UnitZ = new Vector3E(0f, 0f, 1f);
    public static readonly Vector3E Zero = new Vector3E(0f, 0f, 0f);
    public static readonly Vector3E One = new Vector3E(1f, 1f, 1f);
    // Extra Consts
    public static readonly Vector3E Forward = UnitX;
    public static readonly Vector3E Up = UnitZ;
    #endregion

    #region statics from .net Vector3
    public static Vector3E Abs(Vector3E value)
    {
        return Vector3.Abs(value.Value);
    }
    public static Vector3E Add(Vector3E left, Vector3E right)
    {
        return Vector3.Add(left.Value, right.Value);
    }
    public static Vector3E Clamp(Vector3E value1, Vector3E min, Vector3E max)
    {
        return Vector3.Clamp(value1.Value, min.Value, max.Value);
    }
    public static Vector3E Cross(Vector3E vector1, Vector3E vector2)
    {
        return Vector3.Cross(vector1.Value, vector2.Value);
    }
    public static float Distance(Vector3E value1, Vector3E value2)
    {
        return Vector3.Distance(value1.Value, value2.Value);
    }
    public static float DistanceSquared(Vector3E value1, Vector3E value2)
    {
        return Vector3.DistanceSquared(value1.Value, value2.Value);
    }
    public static Vector3E Divide(Vector3E left, Vector3E right)
    {
        return Vector3.Divide(left.Value, right.Value);
    }
    public static Vector3E Divide(Vector3E left, float divisor)
    {
        return Vector3.Divide(left.Value, divisor);
    }
    public static float Dot(Vector3E vector1, Vector3E vector2)
    {
        return Vector3.Dot(vector1.Value, vector2.Value);
    }
    public static Vector3E Lerp(Vector3E value1, Vector3E value2, float amount)
    {
        return Vector3.Lerp(value1.Value, value2.Value, amount);
    }
    public static Vector3E Max(Vector3E value1, Vector3E value2)
    {
        return Vector3.Max(value1.Value, value2.Value);
    }
    public static Vector3E Min(Vector3E value1, Vector3E value2)
    {
        return Vector3.Min(value1.Value, value2.Value);
    }
    public static Vector3E Multiply(float left, Vector3E right)
    {
        return Vector3.Multiply(left, right.Value);
    }
    public static Vector3E Multiply(Vector3E left, Vector3E right)
    {
        return Vector3.Multiply(left.Value, right.Value);
    }
    public static Vector3E Multiply(Vector3E left, float right)
    {
        return Vector3.Multiply(left.Value, right);
    }
    public static Vector3E Negate(Vector3E value)
    {
        return Vector3.Negate(value.Value);
    }
    public static Vector3E Normalize(Vector3E value)
    {
        return Vector3.Normalize(value.Value);
    }
    public static Vector3E Reflect(Vector3E vector, Vector3E normal)
    {
        return Vector3.Reflect(vector.Value, normal.Value);
    }
    public static Vector3E SquareRoot(Vector3E value)
    {
        return Vector3.SquareRoot(value.Value);
    }
    public static Vector3E Subtract(Vector3E left, Vector3E right)
    {
        return Vector3.Subtract(left.Value, right.Value);
    }

    #region instance equivalent to static methods
    public readonly Vector3E Abs()
    {
        return Vector3.Abs(Value);
    }
    public readonly Vector3E Clamp(Vector3E min, Vector3E max)
    {
        return Vector3.Clamp(Value, min.Value, max.Value);
    }
    public readonly Vector3E Cross(Vector3E vector2)
    {
        return Vector3.Cross(Value, vector2.Value);
    }
    public readonly float Distance(Vector3E value2)
    {
        return Vector3.Distance(Value, value2.Value);
    }
    public readonly float DistanceSquared(Vector3E value2)
    {
        return Vector3.DistanceSquared(Value, value2.Value);
    }
    public readonly float Dot(Vector3E vector2)
    {
        return Vector3.Dot(Value, vector2.Value);
    }
    public readonly Vector3E Lerp(Vector3E value2, float amount)
    {
        return Vector3.Lerp(Value, value2.Value, amount);
    }
    public readonly Vector3E Negate()
    {
        return Vector3.Negate(Value);
    }
    public readonly Vector3E Normalize()
    {
        return Vector3.Normalize(Value);
    }
    public readonly Vector3E Reflect(Vector3E normal)
    {
        return Vector3.Reflect(Value, normal.Value);
    }
    public readonly Vector3E SquareRoot()
    {
        return Vector3.SquareRoot(Value);
    }
    #endregion

    // Transform
    public static Vector3E Transform(Vector3E value, QuaternionE rotation)
    {
        return Vector3.Transform(value.Value, rotation.Value);
    }
    public static Vector4E TransformPoint(Vector3E value, QuaternionE rotation)
    {
        return Vector4.Transform(value.Value, rotation.Value);
    }
    public static Vector3E Transform(Vector3E position, Matrix4x4E matrix)
    {
        return Vector3.Transform(position.Value, matrix.Value);
    }
    public static Vector3E TransformNormal(Vector3E position, Matrix4x4E matrix)
    {
        return Vector3.TransformNormal(position.Value, matrix.Value);
    }
    public static Vector4E TransformPoint(Vector3E value, Matrix4x4E matrix)
    {
        return Vector4.Transform(value.Value, matrix.Value);
    }
    public static Vector3E Transform(Vector3E position, Matrix4x4T matrix)
    {
        return Vector3.Transform(position.Value, (Matrix4x4)matrix);
    }
    public static Vector3E TransformNormal(Vector3E position, Matrix4x4T matrix)
    {
        return Vector3.TransformNormal(position.Value, (Matrix4x4)matrix);
    }
    public static Vector4E TransformPoint(Vector3E value, Matrix4x4T matrix)
    {
        return Vector4.Transform(value.Value, (Matrix4x4)matrix);
    }
    // instance Transform
    public readonly Vector3E Transform(QuaternionE rotation)
    {
        return Vector3.Transform(Value, rotation.Value);
    }
    public readonly Vector4E TransformPoint(QuaternionE rotation)
    {
        return Vector4.Transform(Value, rotation.Value);
    }
    public readonly Vector3E Transform(Matrix4x4E matrix)
    {
        return Vector3.Transform(Value, matrix.Value);
    }
    public readonly Vector3E TransformNormal(Matrix4x4E matrix)
    {
        return Vector3.TransformNormal(Value, matrix.Value);
    }
    public readonly Vector4E TransformPoint(Matrix4x4E matrix)
    {
        return Vector4.Transform(Value, matrix.Value);
    }
    public readonly Vector3E Transform(Matrix4x4T matrix)
    {
        return Vector3.Transform(Value, (Matrix4x4)matrix);
    }
    public readonly Vector3E TransformNormal(Matrix4x4T matrix)
    {
        return Vector3.TransformNormal(Value, (Matrix4x4)matrix);
    }
    public readonly Vector4E TransformPoint(Matrix4x4T matrix)
    {
        return Vector4.Transform(Value, (Matrix4x4)matrix);
    }
    #endregion

    #region index
    private delegate ref float GetElementRefDel(ref Vector3 vec);
    private static GetElementRefDel[] _GetElementRefFuncs = new GetElementRefDel[3]
    {
        GetElementRef0,
        GetElementRef1,
        GetElementRef2,
    };
    private static ref float GetElementRef0(ref Vector3 vec)
    {
        return ref vec.X;
    }
    private static ref float GetElementRef1(ref Vector3 vec)
    {
        return ref vec.Y;
    }
    private static ref float GetElementRef2(ref Vector3 vec)
    {
        return ref vec.Z;
    }
    public static float GetElementAt(Vector3 vec, int index)
    {
        return _GetElementRefFuncs[index](ref vec);
    }
    public static void SetElementAt(ref Vector3 vec, int index, float value)
    {
        _GetElementRefFuncs[index](ref vec) = value;
    }
    public readonly float GetElementAt(int index)
    {
        return GetElementAt(Value, index);
    }
    public readonly Vector3E WithElement(int index, float value)
    {
        Vector3 v = Value;
        SetElementAt(ref v, index, value);
        return v;
    }
    public float this[int index]
    {
        readonly get => GetElementAt(Value, index);
        set => SetElementAt(ref Value, index, value);
    }
    public readonly Vector3E this[int index, float newvalue] => WithElement(index, newvalue);

    public readonly Vector3E WithX(float value)
    {
        return Value with { X = value };
    }
    public readonly Vector3E WithY(float value)
    {
        return Value with { Y = value };
    }
    public readonly Vector3E WithZ(float value)
    {
        return Value with { Z = value };
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

public static class Vector3Extensions
{
    public static bool EQApprox(this Vector3 a, Vector3 b, float epsilon = NumberF.ComparisonEpsilon)
    {
        return Vector3E.EQApprox(a, b, epsilon);
    }
    public static bool NEApprox(this Vector3 a, Vector3 b, float epsilon = NumberF.ComparisonEpsilon)
    {
        return Vector3E.NEApprox(a, b, epsilon);
    }

    #region extension equivalent to .net Vector3 static methods
    public static Vector3 Abs(this Vector3 value)
    {
        return Vector3.Abs(value);
    }
    public static Vector3 Clamp(this Vector3 value1, Vector3 min, Vector3 max)
    {
        return Vector3.Clamp(value1, min, max);
    }
    public static Vector3 Cross(this Vector3 vector1, Vector3 vector2)
    {
        return Vector3.Cross(vector1, vector2);
    }
    public static float Distance(this Vector3 value1, Vector3 value2)
    {
        return Vector3.Distance(value1, value2);
    }
    public static float DistanceSquared(this Vector3 value1, Vector3 value2)
    {
        return Vector3.DistanceSquared(value1, value2);
    }
    public static float Dot(this Vector3 vector1, Vector3 vector2)
    {
        return Vector3.Dot(vector1, vector2);
    }
    public static Vector3 Lerp(this Vector3 value1, Vector3 value2, float amount)
    {
        return Vector3.Lerp(value1, value2, amount);
    }
    public static Vector3 Negate(this Vector3 value)
    {
        return Vector3.Negate(value);
    }
    public static Vector3 Normalize(this Vector3 value)
    {
        return Vector3.Normalize(value);
    }
    public static Vector3 Reflect(this Vector3 vector, Vector3 normal)
    {
        return Vector3.Reflect(vector, normal);
    }
    public static Vector3 SquareRoot(this Vector3 value)
    {
        return Vector3.SquareRoot(value);
    }
    #endregion

    #region Deconstructors
    public static void Deconstruct(this Vector3 value, out Vector2 v2, out float z)
    {
        v2 = new Vector2(value.X, value.Y);
        z = value.Z;
    }
    public static void Deconstruct(this Vector3 value, out float x, out float y, out float z)
    {
        x = value.X;
        y = value.Y;
        z = value.Z;
    }
    #endregion

    #region index
    public static float GetElementAt(this Vector3 vec, int index)
    {
        return Vector3E.GetElementAt(vec, index);
    }
    public static void SetElementAt(this ref Vector3 vec, int index, float value)
    {
        Vector3E.SetElementAt(ref vec, index, value);
    }
    public static Vector3 WithElement(this Vector3 vec, int index, float value)
    {
        Vector3E.SetElementAt(ref vec, index, value);
        return vec;
    }
    #endregion
}
