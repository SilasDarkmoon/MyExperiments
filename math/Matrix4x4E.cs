using System.Numerics;
using System.Runtime.InteropServices;

namespace gameplay.math;

/// <summary>
/// The .net Matrix transforms a row-vector at its left-hand-side, While the Unity Matrix transforms a column-vector at its right-hand-side
/// The .net Matrix layout:
/// X ( m11, m12, m13, m14 )
/// Y ( m21, m22, m23, m24 )
/// Z ( m31, m32, m33, m34 )
/// W ( m41, m42, m43, m44 )
/// The Unity Matrix layout:
/// m00, m10, m20, m30, m01, m11, m21, m31, m02, m12, m22, m32, m03, m13, m23, m33 ->
/// m00  ⬇  m01  ⬇  m02  ⬇  m03
/// m10  |  m11  |  m12  |  m13
/// m20  |  m21  |  m22  |  m23
/// m30  ⬇  m31  ⬇  m32  ⬇  m33
/// The translate Matrix in .net is at the 4th row (W), while the translate Matrix in Unity is at the 4th column.
/// so, Matrix4x4E is wrapper of .net Matrix (Right-Handed)
/// while Matrix4x4T is similar to Unity Matrix. (Left-Handed)
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Matrix4x4E : IEquatable<Matrix4x4>, IEquatable<Matrix4x4E>, IEquatableApprox<Matrix4x4, float>, IEquatableApprox<Matrix4x4E, float>
{
    public Matrix4x4 Value;
    public Matrix4x4E(Matrix4x4 value)
    {
        Value = value;
    }
    public Matrix4x4E(Matrix3x2 mat2)
    {
        Value = new Matrix4x4(mat2);
    }
    public Matrix4x4E(float m11, float m12, float m13, float m14, float m21, float m22, float m23, float m24, float m31, float m32, float m33, float m34, float m41, float m42, float m43, float m44)
    {
        Value = new Matrix4x4(m11, m12, m13, m14, m21, m22, m23, m24, m31, m32, m33, m34, m41, m42, m43, m44);
    }
    public readonly void Deconstruct(out float m11, out float m12, out float m13, out float m14, out float m21, out float m22, out float m23, out float m24, out float m31, out float m32, out float m33, out float m34, out float m41, out float m42, out float m43, out float m44)
    {
        m11 = Value.M11;
        m12 = Value.M12;
        m13 = Value.M13;
        m14 = Value.M14;
        m21 = Value.M21;
        m22 = Value.M22;
        m23 = Value.M23;
        m24 = Value.M24;
        m31 = Value.M31;
        m32 = Value.M32;
        m33 = Value.M33;
        m34 = Value.M34;
        m41 = Value.M41;
        m42 = Value.M42;
        m43 = Value.M43;
        m44 = Value.M44;
    }
    public static implicit operator Matrix4x4E((float m11, float m12, float m13, float m14, float m21, float m22, float m23, float m24, float m31, float m32, float m33, float m34, float m41, float m42, float m43, float m44) tuple)
    {
        return new Matrix4x4E(tuple.m11, tuple.m12, tuple.m13, tuple.m14, tuple.m21, tuple.m22, tuple.m23, tuple.m24, tuple.m31, tuple.m32, tuple.m33, tuple.m34, tuple.m41, tuple.m42, tuple.m43, tuple.m44);
    }
    public Matrix4x4E(Vector4E rowX, Vector4E rowY, Vector4E rowZ, Vector4E rowW)
        : this(rowX.X, rowX.Y, rowX.Z, rowX.W, rowY.X, rowY.Y, rowY.Z, rowY.W, rowZ.X, rowZ.Y, rowZ.Z, rowZ.W, rowW.X, rowW.Y, rowW.Z, rowW.W)
    {
    }
    public readonly void Deconstruct(out Vector4E rowX, out Vector4E rowY, out Vector4E rowZ, out Vector4E rowW)
    {
        rowX = new Vector4E(Value.M11, Value.M12, Value.M13, Value.M14);
        rowY = new Vector4E(Value.M21, Value.M22, Value.M23, Value.M24);
        rowZ = new Vector4E(Value.M31, Value.M32, Value.M33, Value.M34);
        rowW = new Vector4E(Value.M41, Value.M42, Value.M43, Value.M44);
    }
    public static implicit operator Matrix4x4E((Vector4E rowX, Vector4E rowY, Vector4E rowZ, Vector4E rowW) tuple)
    {
        return new Matrix4x4E(tuple.rowX, tuple.rowY, tuple.rowZ, tuple.rowW);
    }
    public Matrix4x4E(Vector3E scale, QuaternionE rotation, Vector3E translation)
    {
        var mat = Matrix4x4.CreateScale(scale);
        mat = mat * Matrix4x4.CreateFromQuaternion(rotation);
        mat.Translation = translation;
        Value = mat;
    }
    public readonly void Deconstruct(out Vector3E scale, out QuaternionE rotation, out Vector3E translation)
    {
        Decompose(out scale, out rotation, out translation);
    }
    public static implicit operator Matrix4x4E((Vector3E scale, QuaternionE rotation, Vector3E translation) tuple)
    {
        return new Matrix4x4E(tuple.scale, tuple.rotation, tuple.translation);
    }
    public static implicit operator Matrix4x4E(Matrix4x4 value) { return new Matrix4x4E(value); }
    public static implicit operator Matrix4x4(Matrix4x4E value) { return value.Value; }
    public float M11 { readonly get => Value.M11; set => Value.M11 = value; }
    public float M12 { readonly get => Value.M12; set => Value.M12 = value; }
    public float M13 { readonly get => Value.M13; set => Value.M13 = value; }
    public float M14 { readonly get => Value.M14; set => Value.M14 = value; }
    public float M21 { readonly get => Value.M21; set => Value.M21 = value; }
    public float M22 { readonly get => Value.M22; set => Value.M22 = value; }
    public float M23 { readonly get => Value.M23; set => Value.M23 = value; }
    public float M24 { readonly get => Value.M24; set => Value.M24 = value; }
    public float M31 { readonly get => Value.M31; set => Value.M31 = value; }
    public float M32 { readonly get => Value.M32; set => Value.M32 = value; }
    public float M33 { readonly get => Value.M33; set => Value.M33 = value; }
    public float M34 { readonly get => Value.M34; set => Value.M34 = value; }
    public float M41 { readonly get => Value.M41; set => Value.M41 = value; }
    public float M42 { readonly get => Value.M42; set => Value.M42 = value; }
    public float M43 { readonly get => Value.M43; set => Value.M43 = value; }
    public float M44 { readonly get => Value.M44; set => Value.M44 = value; }

    public static bool EQApprox(Matrix4x4 m1, Matrix4x4 m2, float epsilon = NumberF.ComparisonEpsilon)
    {
        return NumberF.EQApprox(m1.M11, m2.M11, epsilon) && NumberF.EQApprox(m1.M12, m2.M12, epsilon) && NumberF.EQApprox(m1.M13, m2.M13, epsilon) && NumberF.EQApprox(m1.M14, m2.M14, epsilon)
            && NumberF.EQApprox(m1.M21, m2.M21, epsilon) && NumberF.EQApprox(m1.M22, m2.M22, epsilon) && NumberF.EQApprox(m1.M23, m2.M23, epsilon) && NumberF.EQApprox(m1.M24, m2.M24, epsilon)
            && NumberF.EQApprox(m1.M31, m2.M31, epsilon) && NumberF.EQApprox(m1.M32, m2.M32, epsilon) && NumberF.EQApprox(m1.M33, m2.M33, epsilon) && NumberF.EQApprox(m1.M34, m2.M34, epsilon)
            && NumberF.EQApprox(m1.M41, m2.M41, epsilon) && NumberF.EQApprox(m1.M42, m2.M42, epsilon) && NumberF.EQApprox(m1.M43, m2.M43, epsilon) && NumberF.EQApprox(m1.M44, m2.M44, epsilon)
            ;
    }
    public static bool NEApprox(Matrix4x4 m1, Matrix4x4 m2, float epsilon = NumberF.ComparisonEpsilon)
    {
        return NumberF.NEApprox(m1.M11, m2.M11, epsilon) || NumberF.NEApprox(m1.M12, m2.M12, epsilon) || NumberF.NEApprox(m1.M13, m2.M13, epsilon) || NumberF.NEApprox(m1.M14, m2.M14, epsilon)
            || NumberF.NEApprox(m1.M21, m2.M21, epsilon) || NumberF.NEApprox(m1.M22, m2.M22, epsilon) || NumberF.NEApprox(m1.M23, m2.M23, epsilon) || NumberF.NEApprox(m1.M24, m2.M24, epsilon)
            || NumberF.NEApprox(m1.M31, m2.M31, epsilon) || NumberF.NEApprox(m1.M32, m2.M32, epsilon) || NumberF.NEApprox(m1.M33, m2.M33, epsilon) || NumberF.NEApprox(m1.M34, m2.M34, epsilon)
            || NumberF.NEApprox(m1.M41, m2.M41, epsilon) || NumberF.NEApprox(m1.M42, m2.M42, epsilon) || NumberF.NEApprox(m1.M43, m2.M43, epsilon) || NumberF.NEApprox(m1.M44, m2.M44, epsilon)
            ;
    }
    public readonly bool EQApprox(Matrix4x4 other, float epsilon = NumberF.ComparisonEpsilon)
    {
        return EQApprox(Value, other, epsilon);
    }
    public readonly bool NEApprox(Matrix4x4 other, float epsilon = NumberF.ComparisonEpsilon)
    {
        return NEApprox(Value, other, epsilon);
    }
    public readonly bool EQApprox(Matrix4x4E other, float epsilon = NumberF.ComparisonEpsilon)
    {
        return EQApprox(Value, other.Value, epsilon);
    }
    public readonly bool NEApprox(Matrix4x4E other, float epsilon = NumberF.ComparisonEpsilon)
    {
        return NEApprox(Value, other.Value, epsilon);
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(
            HashCode.Combine(MathF.Round(Value.M11, 4), MathF.Round(Value.M12, 4), MathF.Round(Value.M13, 4), MathF.Round(Value.M14, 4)
                           , MathF.Round(Value.M21, 4), MathF.Round(Value.M22, 4), MathF.Round(Value.M23, 4), MathF.Round(Value.M24, 4)),
            HashCode.Combine(MathF.Round(Value.M31, 4), MathF.Round(Value.M32, 4), MathF.Round(Value.M33, 4), MathF.Round(Value.M34, 4)
                           , MathF.Round(Value.M41, 4), MathF.Round(Value.M42, 4), MathF.Round(Value.M43, 4), MathF.Round(Value.M44, 4))
            );
    }
    public override readonly bool Equals(object? obj)
    {
        return obj switch
        {
            Matrix4x4E me => EQApprox(Value, me.Value),
            Matrix4x4 m => EQApprox(Value, m),
            _ => false,
        };
    }
    public override readonly string ToString()
    {
        return Value.ToString();
    }

    #region IEquatable<T>
    public readonly bool Equals(Matrix4x4E other)
    {
        return EQApprox(Value, other.Value);
    }
    public readonly bool Equals(Matrix4x4 other)
    {
        return EQApprox(Value, other);
    }
    #endregion
    #region IEquatableApprox
    bool IEquatableApprox.EQApprox(object other, double epsilon)
    {
        return other switch
        {
            Matrix4x4E me => EQApprox(Value, me.Value, (float)epsilon),
            Matrix4x4 m => EQApprox(Value, m, (float)epsilon),
            _ => false,
        };
    }
    bool IEquatableApprox.NEApprox(object other, double epsilon)
    {
        return other switch
        {
            Matrix4x4E me => NEApprox(Value, me.Value, (float)epsilon),
            Matrix4x4 m => NEApprox(Value, m, (float)epsilon),
            _ => true,
        };
    }
    #endregion

    #region operators
    #region op - equal
    public static bool operator ==(Matrix4x4E left, Matrix4x4E right)
    {
        return EQApprox(left.Value, right.Value);
    }
    public static bool operator !=(Matrix4x4E left, Matrix4x4E right)
    {
        return NEApprox(left.Value, right.Value);
    }
    public static bool operator ==(Matrix4x4E left, Matrix4x4 right)
    {
        return EQApprox(left.Value, right);
    }
    public static bool operator !=(Matrix4x4E left, Matrix4x4 right)
    {
        return NEApprox(left.Value, right);
    }
    public static bool operator ==(Matrix4x4 left, Matrix4x4E right)
    {
        return EQApprox(left, right.Value);
    }
    public static bool operator !=(Matrix4x4 left, Matrix4x4E right)
    {
        return NEApprox(left, right.Value);
    }
    #endregion
    #region op - arithmetic
    public static Matrix4x4E operator +(Matrix4x4E left, Matrix4x4E right)
    {
        return left.Value + right.Value;
    }
    public static Matrix4x4E operator -(Matrix4x4E left, Matrix4x4E right)
    {
        return left.Value - right.Value;
    }
    public static Matrix4x4E operator *(Matrix4x4E left, Matrix4x4E right)
    {
        return left.Value * right.Value;
    }
    public static Matrix4x4E operator /(Matrix4x4E left, Matrix4x4E right)
    {
        Matrix4x4.Invert(right.Value, out var invright);
        return left.Value * invright;
    }
    public static Matrix4x4E operator +(Matrix4x4E left, Matrix4x4 right)
    {
        return left.Value + right;
    }
    public static Matrix4x4E operator -(Matrix4x4E left, Matrix4x4 right)
    {
        return left.Value - right;
    }
    public static Matrix4x4E operator *(Matrix4x4E left, Matrix4x4 right)
    {
        return left.Value * right;
    }
    public static Matrix4x4E operator /(Matrix4x4E left, Matrix4x4 right)
    {
        Matrix4x4.Invert(right, out var invright);
        return left.Value * invright;
    }
    public static Matrix4x4E operator +(Matrix4x4 left, Matrix4x4E right)
    {
        return left + right.Value;
    }
    public static Matrix4x4E operator -(Matrix4x4 left, Matrix4x4E right)
    {
        return left - right.Value;
    }
    public static Matrix4x4E operator *(Matrix4x4 left, Matrix4x4E right)
    {
        return left * right.Value;
    }
    public static Matrix4x4E operator /(Matrix4x4 left, Matrix4x4E right)
    {
        Matrix4x4.Invert(right.Value, out var invright);
        return left * invright;
    }
    public static Matrix4x4E operator *(Matrix4x4E left, float right)
    {
        return left.Value * right;
    }
    public static Matrix4x4E operator *(float left, Matrix4x4E right)
    {
        return right.Value * left;
    }
    public static Matrix4x4E operator /(Matrix4x4E left, float right)
    {
        return left.Value * (1f / right);
    }
    public static Matrix4x4E operator -(Matrix4x4E value)
    {
        return -value.Value;
    }
    public static Matrix4x4E operator +(Matrix4x4E value)
    {
        return value;
    }
    public static Matrix4x4E operator !(Matrix4x4E value)
    {
        return value.Invert();
    }
    public static Matrix4x4E operator ~(Matrix4x4E value)
    {
        return value.Transpose();
    }
    #endregion
    #region op - transform
    public static Vector4E operator *(Vector4E v, Matrix4x4E m)
    {
        return Vector4.Transform(v.Value, m.Value);
    }
    public static Vector3E operator *(Vector3E v, Matrix4x4E m)
    {
        return Vector3.Transform(v.Value, m.Value);
    }
    public static Vector3E operator ^(Vector3E v, Matrix4x4E m)
    {
        return Vector3.TransformNormal(v.Value, m.Value);
    }
    #endregion
    #endregion

    #region Consts
    public static readonly Matrix4x4E Zero = new Matrix4x4E(0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f);
    public static readonly Matrix4x4E Identity = Matrix4x4.Identity;
    // Extra Consts
    #endregion

    #region statics from .net Matrix
    public static Matrix4x4E Add(Matrix4x4E value1, Matrix4x4E value2)
    {
        return Matrix4x4.Add(value1.Value, value2.Value);
    }
    public static Matrix4x4E Concatenate(Matrix4x4E value1, Matrix4x4E value2)
    {
        return value1.Value * value2.Value;
    }
    public static Matrix4x4E CreateBillboard(Vector3E objectPosition, Vector3E cameraPosition, Vector3E cameraUpVector, Vector3E cameraForwardVector)
    {
        return Matrix4x4.CreateBillboard(objectPosition.Value, cameraPosition.Value, cameraUpVector.Value, cameraForwardVector.Value);
    }
    public static Matrix4x4E CreateConstrainedBillboard(Vector3E objectPosition, Vector3E cameraPosition, Vector3E rotateAxis, Vector3E cameraForwardVector, Vector3E objectForwardVector)
    {
        return Matrix4x4.CreateConstrainedBillboard(objectPosition.Value, cameraPosition.Value, rotateAxis.Value, cameraForwardVector.Value, objectForwardVector.Value);
    }
    public static Matrix4x4E CreateFromAxisAngleRadians(Vector3E axis, float radians)
    {
        return Matrix4x4.CreateFromAxisAngle(axis.Value, radians);
    }
    public static Matrix4x4E CreateFromQuaternion(QuaternionE quaternion)
    {
        return Matrix4x4.CreateFromQuaternion(quaternion.Value);
    }
    public static Matrix4x4E CreateFromYawPitchRoll(float yawYLast, float pitchXSecond, float rollZFirst)
    {
        return Matrix4x4.CreateFromYawPitchRoll(yawYLast, pitchXSecond, rollZFirst);
    }
    public static Matrix4x4E CreateLookAt(Vector3E cameraPosition, Vector3E cameraTarget, Vector3E cameraUpVector)
    {
        return Matrix4x4.CreateLookAt(cameraPosition.Value, cameraTarget.Value, cameraUpVector.Value);
    }
    public static Matrix4x4E CreateLookAtLH(Vector3E cameraPosition, Vector3E cameraTarget, Vector3E cameraUpVector)
    {
        var rmat = Matrix4x4.CreateLookAt(cameraPosition.Value, cameraTarget.Value, cameraUpVector.Value);
        rmat.M11 = -rmat.M11;
        rmat.M13 = -rmat.M13;
        rmat.M21 = -rmat.M21;
        rmat.M23 = -rmat.M23;
        rmat.M31 = -rmat.M31;
        rmat.M33 = -rmat.M33;
        rmat.M41 = -rmat.M41;
        rmat.M43 = -rmat.M43;
        return rmat;
    }
    public static Matrix4x4E CreateLookTo(Vector3E cameraPosition, Vector3E cameraDirection, Vector3E cameraUpVector)
    {
        return Matrix4x4.CreateLookAt(cameraPosition.Value, cameraPosition.Value + cameraDirection.Value, cameraUpVector.Value);
    }
    public static Matrix4x4E CreateLookToLH(Vector3E cameraPosition, Vector3E cameraDirection, Vector3E cameraUpVector)
    {
        var rmat = Matrix4x4.CreateLookAt(cameraPosition.Value, cameraPosition.Value + cameraDirection.Value, cameraUpVector.Value);
        rmat.M11 = -rmat.M11;
        rmat.M13 = -rmat.M13;
        rmat.M21 = -rmat.M21;
        rmat.M23 = -rmat.M23;
        rmat.M31 = -rmat.M31;
        rmat.M33 = -rmat.M33;
        rmat.M41 = -rmat.M41;
        rmat.M43 = -rmat.M43;
        return rmat;
    }
    public static Matrix4x4E CreateOrthographic(float width, float height, float zNearPlane, float zFarPlane)
    {
        return Matrix4x4.CreateOrthographic(width, height, zNearPlane, zFarPlane);
    }
    public static Matrix4x4E CreateOrthographicLH(float width, float height, float zNearPlane, float zFarPlane)
    {
        var rmat = Matrix4x4.CreateOrthographic(width, height, zNearPlane, zFarPlane);
        rmat.M33 = -rmat.M33;
        return rmat;
    }
    public static Matrix4x4E CreateOrthographicOffCenter(float left, float right, float bottom, float top, float zNearPlane, float zFarPlane)
    {
        return Matrix4x4.CreateOrthographicOffCenter(left, right, bottom, top, zNearPlane, zFarPlane);
    }
    public static Matrix4x4E CreateOrthographicOffCenterLH(float left, float right, float bottom, float top, float zNearPlane, float zFarPlane)
    {
        var rmat = Matrix4x4.CreateOrthographicOffCenter(left, right, bottom, top, zNearPlane, zFarPlane);
        rmat.M33 = -rmat.M33;
        return rmat;
    }
    public static Matrix4x4E CreatePerspective(float width, float height, float nearPlaneDistance, float farPlaneDistance)
    {
        return Matrix4x4.CreatePerspective(width, height, nearPlaneDistance, farPlaneDistance);
    }
    public static Matrix4x4E CreatePerspectiveLH(float width, float height, float nearPlaneDistance, float farPlaneDistance)
    {
        var rmat = Matrix4x4.CreatePerspective(width, height, nearPlaneDistance, farPlaneDistance);
        rmat.M33 = -rmat.M33;
        return rmat;
    }
    public static Matrix4x4E CreatePerspectiveFieldOfView(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
    {
        return Matrix4x4.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance);
    }
    public static Matrix4x4E CreatePerspectiveFieldOfViewLH(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
    {
        var rmat = Matrix4x4.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance);
        rmat.M33 = -rmat.M33;
        return rmat;
    }
    public static Matrix4x4E CreatePerspectiveOffCenter(float left, float right, float bottom, float top, float nearPlaneDistance, float farPlaneDistance)
    {
        return Matrix4x4.CreatePerspectiveOffCenter(left, right, bottom, top, nearPlaneDistance, farPlaneDistance);
    }
    public static Matrix4x4E CreatePerspectiveOffCenterLH(float left, float right, float bottom, float top, float nearPlaneDistance, float farPlaneDistance)
    {
        var rmat = Matrix4x4.CreatePerspectiveOffCenter(left, right, bottom, top, nearPlaneDistance, farPlaneDistance);
        rmat.M33 = -rmat.M33;
        return rmat;
    }
    public static Matrix4x4E CreateViewport(float x, float y, float width, float height, float minDepth, float maxDepth)
    {
        float wx = width * 0.5f;
        float wy = height * 0.5f;
        return new Matrix4x4E(wx, 0f, 0f, 0f,
            0f, -wy, 0f, 0f,
            0f, 0f, minDepth - maxDepth, 0f,
            wx + x, wy + y, minDepth, 1f);
    }
    public static Matrix4x4E CreateViewportLH(float x, float y, float width, float height, float minDepth, float maxDepth)
    {
        float wx = width * 0.5f;
        float wy = height * 0.5f;
        return new Matrix4x4E(wx, 0f, 0f, 0f,
            0f, -wy, 0f, 0f,
            0f, 0f, maxDepth - minDepth, 0f,
            wx + x, wy + y, minDepth, 1f);
    }
    public static Matrix4x4E CreateReflection(Plane value)
    {
        return Matrix4x4.CreateReflection(value);
    }
    public static Matrix4x4E CreateRotationX(float radians)
    {
        return Matrix4x4.CreateRotationX(radians);
    }
    public static Matrix4x4E CreateRotationX(float radians, Vector3E centerPoint)
    {
        return Matrix4x4.CreateRotationX(radians, centerPoint.Value);
    }
    public static Matrix4x4E CreateRotationY(float radians)
    {
        return Matrix4x4.CreateRotationY(radians);
    }
    public static Matrix4x4E CreateRotationY(float radians, Vector3E centerPoint)
    {
        return Matrix4x4.CreateRotationY(radians, centerPoint.Value);
    }
    public static Matrix4x4E CreateRotationZ(float radians)
    {
        return Matrix4x4.CreateRotationZ(radians);
    }
    public static Matrix4x4E CreateRotationZ(float radians, Vector3E centerPoint)
    {
        return Matrix4x4.CreateRotationZ(radians, centerPoint.Value);
    }
    public static Matrix4x4E CreateScale(float scale)
    {
        return Matrix4x4.CreateScale(scale);
    }
    public static Matrix4x4E CreateScale(float scale, Vector3E centerPoint)
    {
        return Matrix4x4.CreateScale(scale, centerPoint.Value);
    }
    public static Matrix4x4E CreateScale(float xScale, float yScale, float zScale)
    {
        return Matrix4x4.CreateScale(xScale, yScale, zScale);
    }
    public static Matrix4x4E CreateScale(float xScale, float yScale, float zScale, Vector3E centerPoint)
    {
        return Matrix4x4.CreateScale(xScale, yScale, zScale, centerPoint.Value);
    }
    public static Matrix4x4E CreateScale(Vector3E scales)
    {
        return Matrix4x4.CreateScale(scales.Value);
    }
    public static Matrix4x4E CreateScale(Vector3E scales, Vector3E centerPoint)
    {
        return Matrix4x4.CreateScale(scales.Value, centerPoint.Value);
    }
    public static Matrix4x4E CreateShadow(Vector3E lightDirection, Plane plane)
    {
        return Matrix4x4.CreateShadow(lightDirection.Value, plane);
    }
    public static Matrix4x4E CreateTranslation(float xPosition, float yPosition, float zPosition)
    {
        return Matrix4x4.CreateTranslation(xPosition, yPosition, zPosition);
    }
    public static Matrix4x4E CreateTranslation(Vector3E position)
    {
        return Matrix4x4.CreateTranslation(position.Value);
    }
    public static Matrix4x4E CreateWorld(Vector3E position, Vector3E forward, Vector3E up)
    {
        return Matrix4x4.CreateWorld(position.Value, forward.Value, up.Value);
    }
    public static Matrix4x4E CreateCompose(Vector3E scale, QuaternionE rotation, Vector3E translation)
    {
        var mat = Matrix4x4.CreateScale(scale);
        mat = mat * Matrix4x4.CreateFromQuaternion(rotation);
        mat.Translation = translation;
        return mat;
    }
    public static bool Decompose(Matrix4x4E matrix, out Vector3E scale, out QuaternionE rotation, out Vector3E translation)
    {
        bool success = Matrix4x4.Decompose(matrix.Value, out var s, out var r, out var t);
        scale = s;
        rotation = r;
        translation = t;
        return success;
    }
    public static (Vector3E scale, QuaternionE rotation, Vector3E translation) Decompose(Matrix4x4E matrix)
    {
        bool success = Matrix4x4.Decompose(matrix.Value, out var s, out var r, out var t);
        return (s, r, t);
    }
    public static bool Invert(Matrix4x4E matrix, out Matrix4x4E result)
    {
        bool success = Matrix4x4.Invert(matrix.Value, out var inv);
        result = inv;
        return success;
    }
    public static Matrix4x4E Invert(Matrix4x4E matrix)
    {
        bool success = Matrix4x4.Invert(matrix.Value, out var inv);
        return inv;
    }
    public static Matrix4x4E Lerp(Matrix4x4E matrix1, Matrix4x4E matrix2, float amount)
    {
        return Matrix4x4.Lerp(matrix1.Value, matrix2.Value, amount);
    }
    public static Matrix4x4E Multiply(Matrix4x4E value1, Matrix4x4E value2)
    {
        return Matrix4x4.Multiply(value1.Value, value2.Value);
    }
    public static Matrix4x4E Multiply(Matrix4x4E value1, float value2)
    {
        return Matrix4x4.Multiply(value1.Value, value2);
    }
    public static Matrix4x4E Negate(Matrix4x4E value)
    {
        return Matrix4x4.Negate(value.Value);
    }
    public static Matrix4x4E Subtract(Matrix4x4E value1, Matrix4x4E value2)
    {
        return Matrix4x4.Subtract(value1.Value, value2.Value);
    }
    public static Matrix4x4E Transform(Matrix4x4E value, QuaternionE rotation)
    {
        return Matrix4x4.Transform(value.Value, rotation.Value);
    }
    public static Matrix4x4E Transpose(Matrix4x4E matrix)
    {
        return Matrix4x4.Transpose(matrix.Value);
    }
    // Transform
    public static Vector2E Transform(Vector2E position, Matrix4x4E matrix)
    {
        return Vector2.Transform(position.Value, matrix.Value);
    }
    public static Vector2E Transform(Matrix4x4E matrix, Vector2E position)
    {
        return Vector2.Transform(position.Value, matrix.Value);
    }
    public static Vector2E TransformNormal(Vector2E position, Matrix4x4E matrix)
    {
        return Vector2.TransformNormal(position.Value, matrix.Value);
    }
    public static Vector2E TransformNormal(Matrix4x4E matrix, Vector2E position)
    {
        return Vector2.TransformNormal(position.Value, matrix.Value);
    }
    public static Vector3E Transform(Vector3E position, Matrix4x4E matrix)
    {
        return Vector3.Transform(position.Value, matrix.Value);
    }
    public static Vector3E Transform(Matrix4x4E matrix, Vector3E position)
    {
        return Vector3.Transform(position.Value, matrix.Value);
    }
    public static Vector3E TransformNormal(Vector3E position, Matrix4x4E matrix)
    {
        return Vector3.TransformNormal(position.Value, matrix.Value);
    }
    public static Vector3E TransformNormal(Matrix4x4E matrix, Vector3E position)
    {
        return Vector3.TransformNormal(position.Value, matrix.Value);
    }
    public static Vector4E Transform(Vector4E position, Matrix4x4E matrix)
    {
        return Vector4.Transform(position.Value, matrix.Value);
    }
    public static Vector4E Transform(Matrix4x4E matrix, Vector4E position)
    {
        return Vector4.Transform(position.Value, matrix.Value);
    }
    public static Vector4E TransformPoint(Matrix4x4E matrix, Vector2E value)
    {
        return Vector4.Transform(value.Value, matrix.Value);
    }
    public static Vector4E TransformPoint(Vector2E value, Matrix4x4E matrix)
    {
        return Vector4.Transform(value.Value, matrix.Value);
    }
    public static Vector4E TransformPoint(Matrix4x4E matrix, Vector3E value)
    {
        return Vector4.Transform(value.Value, matrix.Value);
    }
    public static Vector4E TransformPoint(Vector3E value, Matrix4x4E matrix)
    {
        return Vector4.Transform(value.Value, matrix.Value);
    }
    #endregion

    #region instance equivalent to static methods
    public readonly Matrix4x4E Concatenate(Matrix4x4E next)
    {
        return Value * next.Value;
    }
    public readonly bool Decompose(out Vector3E scale, out QuaternionE rotation, out Vector3E translation)
    {
        bool success = Matrix4x4.Decompose(Value, out var s, out var r, out var t);
        scale = s;
        rotation = r;
        translation = t;
        return success;
    }
    public readonly (Vector3E scale, QuaternionE rotation, Vector3E translation) Decompose()
    {
        bool success = Matrix4x4.Decompose(Value, out var s, out var r, out var t);
        return (s, r, t);
    }
    public readonly bool Invert(out Matrix4x4E result)
    {
        bool success = Matrix4x4.Invert(Value, out var inv);
        result = inv;
        return success;
    }
    public readonly Matrix4x4E Invert()
    {
        bool success = Matrix4x4.Invert(Value, out var inv);
        return inv;
    }
    public readonly Matrix4x4E Lerp(Matrix4x4E matrix2, float amount)
    {
        return Matrix4x4.Lerp(Value, matrix2.Value, amount);
    }
    public readonly Matrix4x4E Negate()
    {
        return Matrix4x4.Negate(Value);
    }
    public readonly Matrix4x4E Transform(QuaternionE rotation)
    {
        return Matrix4x4.Transform(Value, rotation.Value);
    }
    public readonly Matrix4x4E Transpose()
    {
        return Matrix4x4.Transpose(Value);
    }
    // instance Transform
    public readonly Vector2E Transform(Vector2E position)
    {
        return Vector2.Transform(position.Value, Value);
    }
    public readonly Vector2E TransformNormal(Vector2E position)
    {
        return Vector2.TransformNormal(position.Value, Value);
    }
    public readonly Vector3E Transform(Vector3E position)
    {
        return Vector3.Transform(position.Value, Value);
    }
    public readonly Vector3E TransformNormal(Vector3E position)
    {
        return Vector3.TransformNormal(position.Value, Value);
    }
    public readonly Vector4E Transform(Vector4E position)
    {
        return Vector4.Transform(position.Value, Value);
    }
    public readonly Vector4E TransformPoint(Vector2E value)
    {
        return Vector4.Transform(value.Value, Value);
    }
    public readonly Vector4E TransformPoint(Vector3E value)
    {
        return Vector4.Transform(value.Value, Value);
    }
    // Convert to Quaternion
    public readonly QuaternionE ToQuaternion()
    {
        return Quaternion.CreateFromRotationMatrix(Value);
    }
    #endregion

    #region index
    private delegate ref float GetElementRefDel(ref Matrix4x4 matrix);
    private static GetElementRefDel[] _GetElementRefFuncs = new GetElementRefDel[16]
    {
        GetElementRef0,
        GetElementRef1,
        GetElementRef2,
        GetElementRef3,
        GetElementRef4,
        GetElementRef5,
        GetElementRef6,
        GetElementRef7,
        GetElementRef8,
        GetElementRef9,
        GetElementRef10,
        GetElementRef11,
        GetElementRef12,
        GetElementRef13,
        GetElementRef14,
        GetElementRef15,
    };
    private static ref float GetElementRef0(ref Matrix4x4 matrix)
    {
        return ref matrix.M11;
    }
    private static ref float GetElementRef1(ref Matrix4x4 matrix)
    {
        return ref matrix.M12;
    }
    private static ref float GetElementRef2(ref Matrix4x4 matrix)
    {
        return ref matrix.M13;
    }
    private static ref float GetElementRef3(ref Matrix4x4 matrix)
    {
        return ref matrix.M14;
    }
    private static ref float GetElementRef4(ref Matrix4x4 matrix)
    {
        return ref matrix.M21;
    }
    private static ref float GetElementRef5(ref Matrix4x4 matrix)
    {
        return ref matrix.M22;
    }
    private static ref float GetElementRef6(ref Matrix4x4 matrix)
    {
        return ref matrix.M23;
    }
    private static ref float GetElementRef7(ref Matrix4x4 matrix)
    {
        return ref matrix.M24;
    }
    private static ref float GetElementRef8(ref Matrix4x4 matrix)
    {
        return ref matrix.M31;
    }
    private static ref float GetElementRef9(ref Matrix4x4 matrix)
    {
        return ref matrix.M32;
    }
    private static ref float GetElementRef10(ref Matrix4x4 matrix)
    {
        return ref matrix.M33;
    }
    private static ref float GetElementRef11(ref Matrix4x4 matrix)
    {
        return ref matrix.M34;
    }
    private static ref float GetElementRef12(ref Matrix4x4 matrix)
    {
        return ref matrix.M41;
    }
    private static ref float GetElementRef13(ref Matrix4x4 matrix)
    {
        return ref matrix.M42;
    }
    private static ref float GetElementRef14(ref Matrix4x4 matrix)
    {
        return ref matrix.M43;
    }
    private static ref float GetElementRef15(ref Matrix4x4 matrix)
    {
        return ref matrix.M44;
    }

    public static float GetElementAt(Matrix4x4 matrix, int row, int col)
    {
        return _GetElementRefFuncs[4 * row + col](ref matrix);
    }
    public static void SetElementAt(ref Matrix4x4 matrix, int row, int col, float value)
    {
        _GetElementRefFuncs[4 * row + col](ref matrix) = value;
    }
    public static float GetElementAt(Matrix4x4 matrix, int index)
    {
        return _GetElementRefFuncs[index](ref matrix);
    }
    public static void SetElementAt(ref Matrix4x4 matrix, int index, float value)
    {
        _GetElementRefFuncs[index](ref matrix) = value;
    }
    public readonly float GetElementAt(int row, int col)
    {
        return GetElementAt(Value, row, col);
    }
    public void SetElementAt(int row, int col, float value)
    {
        SetElementAt(ref Value, row, col, value);
    }
    public readonly Matrix4x4E WithElement(int row, int col, float value)
    {
        var mat = Value;
        SetElementAt(ref mat, row, col, value);
        return mat;
    }
    public readonly float GetElementAt(int index)
    {
        return GetElementAt(Value, index);
    }
    public void SetElementAt(int index, float value)
    {
        SetElementAt(ref Value, index, value);
    }
    public readonly Matrix4x4E WithElement(int index, float value)
    {
        var mat = Value;
        SetElementAt(ref mat, index, value);
        return mat;
    }
    public float this[int row, int col]
    {
        readonly get { return GetElementAt(Value, row, col); }
        set { SetElementAt(ref Value, row, col, value); }
    }
    public readonly Matrix4x4E this[int row, int col, float newvalue] => WithElement(row, col, newvalue);
    public float this[int index]
    {
        readonly get { return GetElementAt(Value, index); }
        set { SetElementAt(ref Value, index, value); }
    }
    public readonly Matrix4x4E this[int index, float newvalue] => WithElement(index, newvalue);
    #endregion

    #region Arithmetic Methods
    public readonly bool IsIdentity
    {
        get
        {
            return EQApprox(Value, Matrix4x4E.Identity);
        }
    }
    public Vector3 Translation
    {
        readonly get
        {
            return Value.Translation;
        }
        set
        {
            Value.Translation = value;
        }
    }
    public readonly float GetDeterminant()
    {
        return Value.GetDeterminant();
    }
    #endregion
}

public static class Matrix4x4Extensions
{
    public static bool EQApprox(this Matrix4x4 a, Matrix4x4 b, float epsilon = NumberF.ComparisonEpsilon)
    {
        return Matrix4x4E.EQApprox(a, b, epsilon);
    }
    public static bool NEApprox(this Matrix4x4 a, Matrix4x4 b, float epsilon = NumberF.ComparisonEpsilon)
    {
        return Matrix4x4E.NEApprox(a, b, epsilon);
    }

    #region extension equivalent to .net Matrix4x4 static methods
    public static Matrix4x4 Concatenate(this Matrix4x4 value1, Matrix4x4 next)
    {
        return value1 * next;
    }
    public static bool Decompose(this Matrix4x4 matrix, out Vector3 scale, out Quaternion rotation, out Vector3 translation)
    {
        return Matrix4x4.Decompose(matrix, out scale, out rotation, out translation);
    }
    public static (Vector3 scale, Quaternion rotation, Vector3 translation) Decompose(this Matrix4x4 matrix)
    {
        bool success = Matrix4x4.Decompose(matrix, out var s, out var r, out var t);
        return (s, r, t);
    }
    public static bool Invert(this Matrix4x4 matrix, out Matrix4x4 result)
    {
        return Matrix4x4.Invert(matrix, out result);
    }
    public static Matrix4x4 Invert(this Matrix4x4 matrix)
    {
        bool success = Matrix4x4.Invert(matrix, out var inv);
        return inv;
    }
    public static Matrix4x4 Lerp(this Matrix4x4 matrix1, Matrix4x4 matrix2, float amount)
    {
        return Matrix4x4.Lerp(matrix1, matrix2, amount);
    }
    public static Matrix4x4 Negate(this Matrix4x4 value)
    {
        return Matrix4x4.Negate(value);
    }
    public static Matrix4x4 Transform(this Matrix4x4 value, Quaternion rotation)
    {
        return Matrix4x4.Transform(value, rotation);
    }
    public static Matrix4x4 Transpose(this Matrix4x4 matrix)
    {
        return Matrix4x4.Transpose(matrix);
    }
    // Transform
    public static Vector2 Transform(this Vector2 position, Matrix4x4 matrix)
    {
        return Vector2.Transform(position, matrix);
    }
    public static Vector2 Transform(this Matrix4x4 matrix, Vector2 position)
    {
        return Vector2.Transform(position, matrix);
    }
    public static Vector2 TransformNormal(this Vector2 position, Matrix4x4 matrix)
    {
        return Vector2.TransformNormal(position, matrix);
    }
    public static Vector2 TransformNormal(this Matrix4x4 matrix, Vector2 position)
    {
        return Vector2.TransformNormal(position, matrix);
    }
    public static Vector3 Transform(this Vector3 position, Matrix4x4 matrix)
    {
        return Vector3.Transform(position, matrix);
    }
    public static Vector3 Transform(this Matrix4x4 matrix, Vector3 position)
    {
        return Vector3.Transform(position, matrix);
    }
    public static Vector3 TransformNormal(this Vector3 position, Matrix4x4 matrix)
    {
        return Vector3.TransformNormal(position, matrix);
    }
    public static Vector3 TransformNormal(this Matrix4x4 matrix, Vector3 position)
    {
        return Vector3.TransformNormal(position, matrix);
    }
    public static Vector4 Transform(this Vector4 position, Matrix4x4 matrix)
    {
        return Vector4.Transform(position, matrix);
    }
    public static Vector4 Transform(this Matrix4x4 matrix, Vector4 position)
    {
        return Vector4.Transform(position, matrix);
    }
    public static Vector4 TransformPoint(this Matrix4x4 matrix, Vector2 value)
    {
        return Vector4.Transform(value, matrix);
    }
    public static Vector4 TransformPoint(this Vector2 value, Matrix4x4 matrix)
    {
        return Vector4.Transform(value, matrix);
    }
    public static Vector4 TransformPoint(this Matrix4x4 matrix, Vector3 value)
    {
        return Vector4.Transform(value, matrix);
    }
    public static Vector4 TransformPoint(this Vector3 value, Matrix4x4 matrix)
    {
        return Vector4.Transform(value, matrix);
    }
    // Convert to Quaternion
    public static Quaternion ToQuaternion(this Matrix4x4 matrix)
    {
        return Quaternion.CreateFromRotationMatrix(matrix);
    }
    #endregion

    #region Deconstructors
    public static void Deconstruct(this Matrix4x4 matrix, out float m11, out float m12, out float m13, out float m14, out float m21, out float m22, out float m23, out float m24, out float m31, out float m32, out float m33, out float m34, out float m41, out float m42, out float m43, out float m44)
    {
        m11 = matrix.M11;
        m12 = matrix.M12;
        m13 = matrix.M13;
        m14 = matrix.M14;
        m21 = matrix.M21;
        m22 = matrix.M22;
        m23 = matrix.M23;
        m24 = matrix.M24;
        m31 = matrix.M31;
        m32 = matrix.M32;
        m33 = matrix.M33;
        m34 = matrix.M34;
        m41 = matrix.M41;
        m42 = matrix.M42;
        m43 = matrix.M43;
        m44 = matrix.M44;
    }
    public static void Deconstruct(this Matrix4x4 matrix, out Vector4 rowX, out Vector4 rowY, out Vector4 rowZ, out Vector4 rowW)
    {
        rowX = new Vector4(matrix.M11, matrix.M12, matrix.M13, matrix.M14);
        rowY = new Vector4(matrix.M21, matrix.M22, matrix.M23, matrix.M24);
        rowZ = new Vector4(matrix.M31, matrix.M32, matrix.M33, matrix.M34);
        rowW = new Vector4(matrix.M41, matrix.M42, matrix.M43, matrix.M44);
    }
    public static void Deconstruct(this Matrix4x4 matrix, out Vector3 scale, out Quaternion rotation, out Vector3 translation)
    {
        Matrix4x4.Decompose(matrix, out scale, out rotation, out translation);
    }
    #endregion

    #region index
    public static float GetElementAt(this Matrix4x4 matrix, int row, int col)
    {
        return Matrix4x4E.GetElementAt(matrix, row, col);
    }
    public static void SetElementAt(this ref Matrix4x4 matrix, int row, int col, float value)
    {
        Matrix4x4E.SetElementAt(ref matrix, row, col, value);
    }
    public static Matrix4x4E WithElement(this Matrix4x4 matrix, int row, int col, float value)
    {
        SetElementAt(ref matrix, row, col, value);
        return matrix;
    }
    public static float GetElementAt(this Matrix4x4 matrix, int index)
    {
        return Matrix4x4E.GetElementAt(matrix, index);
    }
    public static void SetElementAt(this ref Matrix4x4 matrix, int index, float value)
    {
        Matrix4x4E.SetElementAt(ref matrix, index, value);
    }
    public static Matrix4x4E WithElement(this Matrix4x4 matrix, int index, float value)
    {
        SetElementAt(ref matrix, index, value);
        return matrix;
    }
    #endregion
}
