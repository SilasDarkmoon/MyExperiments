using System.Numerics;
using System.Runtime.InteropServices;

namespace gameplay.math;

[StructLayout(LayoutKind.Sequential)]
public struct QuaternionE : IEquatable<QuaternionE>, IEquatable<Quaternion>, IEquatableApprox<QuaternionE, float>, IEquatableApprox<Quaternion, float>
{
    public Quaternion Value;
    public QuaternionE(Quaternion value)
    {
        Value = value;
    }
    public QuaternionE(float x, float y, float z, float w)
    {
        Value = new Quaternion(x, y, z, w);
    }
    public readonly void Deconstruct(out float x, out float y, out float z, out float w)
    {
        x = Value.X;
        y = Value.Y;
        z = Value.Z;
        w = Value.W;
    }
    public static implicit operator QuaternionE((float x, float y, float z, float w) tuple)
    {
        return new QuaternionE(tuple.x, tuple.y, tuple.z, tuple.w);
    }
    public QuaternionE(float yawYLast, float pitchXSecond, float rollZFirst)
    {
        Value = Quaternion.CreateFromYawPitchRoll(yawYLast, pitchXSecond, rollZFirst);
    }
    public readonly void Deconstruct(out float yawYLast, out float pitchXSecond, out float rollZFirst)
    {
        DecomposeYawPitchRoll(Value, out yawYLast, out pitchXSecond, out rollZFirst);
    }
    public static implicit operator QuaternionE((float yawYLast, float pitchXSecond, float rollZFirst) tuple)
    {
        return new QuaternionE(tuple.yawYLast, tuple.pitchXSecond, tuple.rollZFirst);
    }
    public QuaternionE(Vector3E axis, float radians)
    {
        Value = Quaternion.CreateFromAxisAngle(axis.Normalize(), radians);
    }
    public readonly void Deconstruct(out Vector3E axis, out float radians)
    {
        DecomposeAxisAngleRadians(Value, out var axisraw, out radians);
        axis = axisraw;
    }
    public static implicit operator QuaternionE((Vector3E axis, float radians) tuple)
    {
        return new QuaternionE(tuple.axis, tuple.radians);
    }

    public static implicit operator QuaternionE(Quaternion value) { return new QuaternionE(value); }
    public static implicit operator Quaternion(QuaternionE value) { return value.Value; }
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

    public static bool EQApprox(Quaternion q1, Quaternion q2, float epsilon = NumberF.ComparisonEpsilon)
    {
        return NumberF.EQApprox(q1.X, q2.X, epsilon) && NumberF.EQApprox(q1.Y, q2.Y, epsilon) && NumberF.EQApprox(q1.Z, q2.Z, epsilon) && NumberF.EQApprox(q1.W, q2.W, epsilon);
    }
    public static bool NEApprox(Quaternion q1, Quaternion q2, float epsilon = NumberF.ComparisonEpsilon)
    {
        return NumberF.NEApprox(q1.X, q2.X, epsilon) || NumberF.NEApprox(q1.Y, q2.Y, epsilon) || NumberF.NEApprox(q1.Z, q2.Z, epsilon) || NumberF.NEApprox(q1.W, q2.W, epsilon);
    }
    public readonly bool EQApprox(Quaternion other, float epsilon = NumberF.ComparisonEpsilon)
    {
        return EQApprox(Value, other, epsilon);
    }
    public readonly bool NEApprox(Quaternion other, float epsilon = NumberF.ComparisonEpsilon)
    {
        return NEApprox(Value, other, epsilon);
    }
    public readonly bool EQApprox(QuaternionE other, float epsilon = NumberF.ComparisonEpsilon)
    {
        return EQApprox(Value, other.Value, epsilon);
    }
    public readonly bool NEApprox(QuaternionE other, float epsilon = NumberF.ComparisonEpsilon)
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
            QuaternionE qe => EQApprox(Value, qe.Value),
            Quaternion q => EQApprox(Value, q),
            _ => false,
        };
    }
    public readonly override string ToString()
    {
        return Value.ToString();
    }

    #region IEquatable<T>
    public readonly bool Equals(QuaternionE other)
    {
        return EQApprox(Value, other.Value);
    }
    public readonly bool Equals(Quaternion other)
    {
        return EQApprox(Value, other);
    }
    #endregion

    #region IEquatableApprox
    readonly bool IEquatableApprox.EQApprox(object other, double epsilon)
    {
        return other switch
        {
            QuaternionE qe => EQApprox(Value, qe.Value, (float)epsilon),
            Quaternion q => EQApprox(Value, q, (float)epsilon),
            _ => false,
        };
    }
    readonly bool IEquatableApprox.NEApprox(object other, double epsilon)
    {
        return other switch
        {
            QuaternionE qe => NEApprox(Value, qe.Value, (float)epsilon),
            Quaternion q => NEApprox(Value, q, (float)epsilon),
            _ => true,
        };
    }
    #endregion

    #region operators
    #region op - equal
    public static bool operator ==(QuaternionE left, QuaternionE right)
    {
        return EQApprox(left.Value, right.Value);
    }
    public static bool operator !=(QuaternionE left, QuaternionE right)
    {
        return NEApprox(left.Value, right.Value);
    }
    public static bool operator ==(QuaternionE left, Quaternion right)
    {
        return EQApprox(left.Value, right);
    }
    public static bool operator !=(QuaternionE left, Quaternion right)
    {
        return NEApprox(left.Value, right);
    }
    public static bool operator ==(Quaternion left, QuaternionE right)
    {
        return EQApprox(left, right.Value);
    }
    public static bool operator !=(Quaternion left, QuaternionE right)
    {
        return NEApprox(left, right.Value);
    }
    #endregion
    #region op - scalar
    public static QuaternionE operator +(QuaternionE left, QuaternionE right)
    {
        return left.Value + right.Value;
    }
    public static QuaternionE operator -(QuaternionE left, QuaternionE right)
    {
        return left.Value - right.Value;
    }
    public static QuaternionE operator *(QuaternionE left, QuaternionE right)
    {
        return left.Value * right.Value;
    }
    public static QuaternionE operator /(QuaternionE left, QuaternionE right)
    {
        return left.Value / right.Value;
    }
    public static QuaternionE operator +(QuaternionE left, Quaternion right)
    {
        return left.Value + right;
    }
    public static QuaternionE operator -(QuaternionE left, Quaternion right)
    {
        return left.Value - right;
    }
    public static QuaternionE operator *(QuaternionE left, Quaternion right)
    {
        return left.Value * right;
    }
    public static QuaternionE operator /(QuaternionE left, Quaternion right)
    {
        return left.Value / right;
    }
    public static QuaternionE operator +(Quaternion left, QuaternionE right)
    {
        return left + right.Value;
    }
    public static QuaternionE operator -(Quaternion left, QuaternionE right)
    {
        return left - right.Value;
    }
    public static QuaternionE operator *(Quaternion left, QuaternionE right)
    {
        return left * right.Value;
    }
    public static QuaternionE operator /(Quaternion left, QuaternionE right)
    {
        return left / right.Value;
    }
    public static QuaternionE operator *(QuaternionE left, float right)
    {
        return left.Value * right;
    }
    public static QuaternionE operator *(float left, QuaternionE right)
    {
        return right.Value * left;
    }
    public static QuaternionE operator /(QuaternionE left, float right)
    {
        return left.Value * (1f / right);
    }
    public static QuaternionE operator -(QuaternionE value)
    {
        return -value.Value;
    }
    public static QuaternionE operator +(QuaternionE value)
    {
        return value;
    }
    public static QuaternionE operator !(QuaternionE value)
    {
        return value.Inverse();
    }
    public static QuaternionE operator ~(QuaternionE value)
    {
        return value.Conjugate();
    }
    #endregion
    #region vector cross(&) and dot(|)
    public static float operator |(QuaternionE left, QuaternionE right)
    {
        return left.Dot(right);
    }
    #endregion
    #region op - transform
    public static Vector3E operator *(QuaternionE q, Vector3E v)
    {
        return Vector3.Transform(v, q);
    }
    #endregion
    #endregion

    #region Consts
    public static readonly QuaternionE Zero = new QuaternionE(0f, 0f, 0f, 0f);
    public static readonly QuaternionE Identity = Quaternion.Identity;
    // Extra Consts
    #endregion

    #region statics from .net Quaternion
    public static QuaternionE Add(QuaternionE value1, QuaternionE value2)
    {
        return Quaternion.Add(value1.Value, value2.Value);
    }
    public static QuaternionE Concatenate(QuaternionE value1, QuaternionE value2)
    {
        return Quaternion.Concatenate(value1.Value, value2.Value);
    }
    public static QuaternionE Conjugate(QuaternionE value)
    {
        return Quaternion.Conjugate(value.Value);
    }
    public static QuaternionE CreateFromAxisAngleRadians(Vector3E axis, float radians)
    {
        return Quaternion.CreateFromAxisAngle(axis.Value, radians);
    }
    public static QuaternionE CreateFromNonnormalizedAxisAngleRadians(Vector3E axis, float radians)
    {
        return Quaternion.CreateFromAxisAngle(axis.Value.Normalize(), radians);
    }
    public static QuaternionE CreateFromRotationMatrix(Matrix4x4E matrix)
    {
        return Quaternion.CreateFromRotationMatrix(matrix.Value);
    }
    public static QuaternionE CreateFromRotationMatrix(Matrix4x4T matrix)
    {
        return Quaternion.CreateFromRotationMatrix((Matrix4x4)matrix).Conjugate();
    }
    public static QuaternionE CreateFromYawPitchRoll(float yawYLast, float pitchXSecond, float rollZFirst)
    {
        return Quaternion.CreateFromYawPitchRoll(yawYLast, pitchXSecond, rollZFirst);
    }
    public static QuaternionE Divide(QuaternionE value1, QuaternionE value2)
    {
        return Quaternion.Divide(value1.Value, value2.Value);
    }
    public static float Dot(QuaternionE quaternion1, QuaternionE quaternion2)
    {
        return Quaternion.Dot(quaternion1.Value, quaternion2.Value);
    }
    public static QuaternionE Inverse(QuaternionE value)
    {
        return Quaternion.Inverse(value.Value);
    }
    public static QuaternionE Lerp(QuaternionE quaternion1, QuaternionE quaternion2, float amount)
    {
        return Quaternion.Lerp(quaternion1.Value, quaternion2.Value, amount);
    }
    public static QuaternionE Multiply(QuaternionE value1, QuaternionE value2)
    {
        return Quaternion.Multiply(value1.Value, value2.Value);
    }
    public static QuaternionE Multiply(QuaternionE value1, float value2)
    {
        return Quaternion.Multiply(value1.Value, value2);
    }
    public static QuaternionE Negate(QuaternionE value)
    {
        return Quaternion.Negate(value.Value);
    }
    public static QuaternionE Normalize(QuaternionE value)
    {
        return Quaternion.Normalize(value.Value);
    }
    public static QuaternionE Slerp(QuaternionE quaternion1, QuaternionE quaternion2, float amount)
    {
        return Quaternion.Slerp(quaternion1.Value, quaternion2.Value, amount);
    }
    public static QuaternionE Subtract(QuaternionE value1, QuaternionE value2)
    {
        return Quaternion.Subtract(value1.Value, value2.Value);
    }
    // Transform
    public static Vector2E Transform(QuaternionE rotation, Vector2E value)
    {
        return Vector2.Transform(value.Value, rotation.Value);
    }
    public static Vector2E Transform(Vector2E value, QuaternionE rotation)
    {
        return Vector2.Transform(value.Value, rotation.Value);
    }
    public static Vector3E Transform(QuaternionE rotation, Vector3E value)
    {
        return Vector3.Transform(value.Value, rotation.Value);
    }
    public static Vector3E Transform(Vector3E value, QuaternionE rotation)
    {
        return Vector3.Transform(value.Value, rotation.Value);
    }
    public static Vector4E Transform(QuaternionE rotation, Vector4E value)
    {
        return Vector4.Transform(value.Value, rotation.Value);
    }
    public static Vector4E Transform(Vector4E value, QuaternionE rotation)
    {
        return Vector4.Transform(value.Value, rotation.Value);
    }
    public static Vector4E TransformPoint(QuaternionE rotation, Vector2E value)
    {
        return Vector4.Transform(value.Value, rotation.Value);
    }
    public static Vector4E TransformPoint(Vector2E value, QuaternionE rotation)
    {
        return Vector4.Transform(value.Value, rotation.Value);
    }
    public static Vector4E TransformPoint(QuaternionE rotation, Vector3E value)
    {
        return Vector4.Transform(value.Value, rotation.Value);
    }
    public static Vector4E TransformPoint(Vector3E value, QuaternionE rotation)
    {
        return Vector4.Transform(value.Value, rotation.Value);
    }

    #region instance equivalent to static methods
    public readonly QuaternionE Concatenate(QuaternionE value2)
    {
        return Quaternion.Concatenate(Value, value2.Value);
    }
    public readonly QuaternionE Conjugate()
    {
        return Quaternion.Conjugate(Value);
    }
    public readonly float Dot(QuaternionE quaternion2)
    {
        return Quaternion.Dot(Value, quaternion2.Value);
    }
    public readonly QuaternionE Inverse()
    {
        return Quaternion.Inverse(Value);
    }
    public readonly QuaternionE Lerp(QuaternionE quaternion2, float amount)
    {
        return Quaternion.Lerp(Value, quaternion2.Value, amount);
    }
    public readonly QuaternionE Negate()
    {
        return Quaternion.Negate(Value);
    }
    public readonly QuaternionE Normalize()
    {
        return Quaternion.Normalize(Value);
    }
    public readonly QuaternionE Slerp(QuaternionE quaternion2, float amount)
    {
        return Quaternion.Slerp(Value, quaternion2.Value, amount);
    }
    // instance Transform
    public readonly Vector2E Transform(Vector2E value)
    {
        return Vector2.Transform(value.Value, Value);
    }
    public readonly Vector3E Transform(Vector3E value)
    {
        return Vector3.Transform(value.Value, Value);
    }
    public readonly Vector4E Transform(Vector4E value)
    {
        return Vector4.Transform(value.Value, Value);
    }
    public readonly Vector4E TransformPoint(Vector2E value)
    {
        return Vector4.Transform(value.Value, Value);
    }
    public readonly Vector4E TransformPoint(Vector3E value)
    {
        return Vector4.Transform(value.Value, Value);
    }
    // Convert to Matrix
    public readonly Matrix4x4E ToMatrix()
    {
        return Matrix4x4.CreateFromQuaternion(Value);
    }
    public readonly Matrix4x4T ToMatrixT()
    {
        return (Matrix4x4T)Matrix4x4.CreateFromQuaternion(Value).Transpose();
    }
    #endregion
    #endregion

    #region Extra static
    public static QuaternionE CreateLookAt(Vector3E cameraPosition, Vector3E cameraTarget, Vector3E cameraUpVector)
    {
        return Matrix4x4.CreateLookAt(cameraPosition, cameraTarget, cameraUpVector).ToQuaternion();
    }
    public static QuaternionE CreateLookTo(Vector3E cameraDirection, Vector3E cameraUpVector)
    {
        return Matrix4x4.CreateLookAt(Vector3.Zero, cameraDirection.Value, cameraUpVector.Value).ToQuaternion();
    }
    public static QuaternionE CreateLookAtLH(Vector3E cameraPosition, Vector3E cameraTarget, Vector3E cameraUpVector)
    {
        return Matrix4x4E.CreateLookAtLH(cameraPosition, cameraTarget, cameraUpVector).Transpose().ToQuaternion();
    }
    public static QuaternionE CreateLookToLH(Vector3E cameraDirection, Vector3E cameraUpVector)
    {
        return Matrix4x4E.CreateLookAtLH(Vector3.Zero, cameraDirection.Value, cameraUpVector.Value).Transpose().ToQuaternion();
    }
    public static void DecomposeYawPitchRoll(Quaternion quaternion, out float yawYLast, out float pitchXSecond, out float rollZFirst)
    {
        float w2 = quaternion.W;
        w2 *= w2;
        float x2 = quaternion.X;
        x2 *= x2;
        float y2 = quaternion.Y;
        y2 *= y2;
        float z2 = quaternion.Z;
        z2 *= z2;

        float len2 = w2 + x2 + y2 + z2;
        float test = quaternion.Y * quaternion.Z - quaternion.X * quaternion.W;

        pitchXSecond = -MathF.Asin(2f * test / len2);

        if (MathF.Abs(test) < 0.499999f * len2)
        {
            yawYLast = MathF.Atan2(2f * (quaternion.X * quaternion.Z + quaternion.Y * quaternion.W), z2 - x2 - y2 + w2);
            rollZFirst = MathF.Atan2(2f * (quaternion.X * quaternion.Y + quaternion.Z * quaternion.W), y2 - z2 - x2 + w2);
        }
        else
        {
            float a, b, c, e;
            a = quaternion.X * quaternion.Y + quaternion.Z * quaternion.W;
            b = -quaternion.Y * quaternion.Z + quaternion.X * quaternion.W;
            c = quaternion.X * quaternion.Y - quaternion.Z * quaternion.W;
            e = quaternion.Y * quaternion.Z + quaternion.X * quaternion.W;
            yawYLast = MathF.Atan2(a * e + b * c, b * e - a * c);
            rollZFirst = 0f;
        }
    }
    public static void DecomposeAxisAngleRadians(Quaternion quaternion, out Vector3 axis, out float radians)
    {
        float cw = Math.Clamp(quaternion.W, -1, 1);
        radians = 2f * MathF.Acos(cw);
        float s = MathF.Sqrt(1 - cw * cw);
        if (NumberF.EQApprox(s, 0f))
        {
            axis = Vector3.UnitX;
        }
        else
        {
            float invs = 1f / s;
            axis = new Vector3(quaternion.X * invs, quaternion.Y * invs, quaternion.Z * invs);
        }
    }
    #endregion

    #region index
    private delegate ref float GetElementRefDel(ref Quaternion quaternion);
    private static GetElementRefDel[] _GetElementRefFuncs = new GetElementRefDel[4]
    {
        GetElementRef0,
        GetElementRef1,
        GetElementRef2,
        GetElementRef3,
    };
    private static ref float GetElementRef0(ref Quaternion quaternion)
    {
        return ref quaternion.X;
    }
    private static ref float GetElementRef1(ref Quaternion quaternion)
    {
        return ref quaternion.Y;
    }
    private static ref float GetElementRef2(ref Quaternion quaternion)
    {
        return ref quaternion.Z;
    }
    private static ref float GetElementRef3(ref Quaternion quaternion)
    {
        return ref quaternion.W;
    }
    public static float GetElementAt(Quaternion quaternion, int index)
    {
        return _GetElementRefFuncs[index](ref quaternion);
    }
    public static void SetElementAt(ref Quaternion quaternion, int index, float value)
    {
        _GetElementRefFuncs[index](ref quaternion) = value;
    }
    public readonly float GetElementAt(int index)
    {
        return GetElementAt(Value, index);
    }
    public readonly QuaternionE WithElement(int index, float value)
    {
        Quaternion q = Value;
        SetElementAt(ref q, index, value);
        return q;
    }
    public float this[int index]
    {
        readonly get => GetElementAt(Value, index);
        set => SetElementAt(ref Value, index, value);
    }
    public readonly QuaternionE this[int index, float newvalue] => WithElement(index, newvalue);

    public readonly QuaternionE WithX(float value)
    {
        return Value with { X = value };
    }
    public readonly QuaternionE WithY(float value)
    {
        return Value with { Y = value };
    }
    public readonly QuaternionE WithZ(float value)
    {
        return Value with { Z = value };
    }
    public readonly QuaternionE WithW(float value)
    {
        return Value with { W = value };
    }
    #endregion

    #region Arithmetic Methods
    public readonly bool IsIdentity
    {
        get
        {
            return EQApprox(Value, Quaternion.Identity);
        }
    }
    public readonly float Length()
    {
        return Value.Length();
    }
    public readonly float LengthSquared()
    {
        return Value.LengthSquared();
    }
    // Extra Methods
    public readonly (float yawYLast, float pitchXSecond, float rollZFirst) DecomposeYawPitchRoll()
    {
        DecomposeYawPitchRoll(Value, out float yaw, out float pitch, out float roll);
        return (yaw, pitch, roll);
    }
    public readonly (Vector3E axis, float radians) DecomposeAxisAngleRadians()
    {
        DecomposeAxisAngleRadians(Value, out var axis, out float radians);
        return (axis, radians);
    }
    #endregion
}

public static class QuaternionExtensions
{
    public static bool EQApprox(this Quaternion a, Quaternion b, float epsilon = NumberF.ComparisonEpsilon)
    {
        return QuaternionE.EQApprox(a, b, epsilon);
    }
    public static bool NEApprox(this Quaternion a, Quaternion b, float epsilon = NumberF.ComparisonEpsilon)
    {
        return QuaternionE.NEApprox(a, b, epsilon);
    }

    public static (float yawYLast, float pitchXSecond, float rollZFirst) DecomposeYawPitchRoll(this Quaternion quaternion)
    {
        QuaternionE.DecomposeYawPitchRoll(quaternion, out float yaw, out float pitch, out float roll);
        return (yaw, pitch, roll);
    }
    public static (Vector3 axis, float radians) DecomposeAxisAngleRadians(this Quaternion quaternion)
    {
        QuaternionE.DecomposeAxisAngleRadians(quaternion, out var axis, out float radians);
        return (axis, radians);
    }

    #region extension equivalent to .net Quaternion static methods
    public static Quaternion Concatenate(this Quaternion value1, Quaternion value2)
    {
        return Quaternion.Concatenate(value1, value2);
    }
    public static Quaternion Conjugate(this Quaternion value)
    {
        return Quaternion.Conjugate(value);
    }
    public static float Dot(this Quaternion quaternion1, Quaternion quaternion2)
    {
        return Quaternion.Dot(quaternion1, quaternion2);
    }
    public static Quaternion Inverse(this Quaternion value)
    {
        return Quaternion.Inverse(value);
    }
    public static Quaternion Lerp(this Quaternion quaternion1, Quaternion quaternion2, float amount)
    {
        return Quaternion.Lerp(quaternion1, quaternion2, amount);
    }
    public static Quaternion Negate(this Quaternion value)
    {
        return Quaternion.Negate(value);
    }
    public static Quaternion Normalize(this Quaternion value)
    {
        return Quaternion.Normalize(value);
    }
    public static Quaternion Slerp(this Quaternion quaternion1, Quaternion quaternion2, float amount)
    {
        return Quaternion.Slerp(quaternion1, quaternion2, amount);
    }

    // Transform
    public static Vector2 Transform(this Quaternion rotation, Vector2 value)
    {
        return Vector2.Transform(value, rotation);
    }
    public static Vector2 Transform(this Vector2 value, Quaternion rotation)
    {
        return Vector2.Transform(value, rotation);
    }
    public static Vector3 Transform(this Quaternion rotation, Vector3 value)
    {
        return Vector3.Transform(value, rotation);
    }
    public static Vector3 Transform(this Vector3 value, Quaternion rotation)
    {
        return Vector3.Transform(value, rotation);
    }
    public static Vector4 Transform(this Quaternion rotation, Vector4 value)
    {
        return Vector4.Transform(value, rotation);
    }
    public static Vector4 Transform(this Vector4 value, Quaternion rotation)
    {
        return Vector4.Transform(value, rotation);
    }
    public static Vector4 TransformPoint(this Quaternion rotation, Vector2 value)
    {
        return Vector4.Transform(value, rotation);
    }
    public static Vector4 TransformPoint(this Vector2 value, Quaternion rotation)
    {
        return Vector4.Transform(value, rotation);
    }
    public static Vector4 TransformPoint(this Quaternion rotation, Vector3 value)
    {
        return Vector4.Transform(value, rotation);
    }
    public static Vector4 TransformPoint(this Vector3 value, Quaternion rotation)
    {
        return Vector4.Transform(value, rotation);
    }
    // Convert to Matrix
    public static Matrix4x4E ToMatrix(this Quaternion rotation)
    {
        return Matrix4x4.CreateFromQuaternion(rotation);
    }
    public static Matrix4x4T ToMatrixT(this Quaternion rotation)
    {
        return (Matrix4x4T)Matrix4x4.CreateFromQuaternion(rotation).Transpose();
    }
    #endregion

    #region Deconstructors
    public static void Deconstruct(this Quaternion quaternion, out float x, out float y, out float z, out float w)
    {
        x = quaternion.X;
        y = quaternion.Y;
        z = quaternion.Z;
        w = quaternion.W;
    }
    public static void Deconstruct(this Quaternion quaternion, out float yawYLast, out float pitchXSecond, out float rollZFirst)
    {
        QuaternionE.DecomposeYawPitchRoll(quaternion, out yawYLast, out pitchXSecond, out rollZFirst);
    }
    public static void Deconstruct(this Quaternion quaternion, out Vector3 axis, out float radians)
    {
        QuaternionE.DecomposeAxisAngleRadians(quaternion, out axis, out radians);
    }
    #endregion

    #region index
    public static float GetElementAt(this Quaternion quaternion, int index)
    {
        return QuaternionE.GetElementAt(quaternion, index);
    }
    public static void SetElementAt(this ref Quaternion quaternion, int index, float value)
    {
        QuaternionE.SetElementAt(ref quaternion, index, value);
    }
    public static Quaternion WithElement(this Quaternion quaternion, int index, float value)
    {
        QuaternionE.SetElementAt(ref quaternion, index, value);
        return quaternion;
    }
    #endregion
}
