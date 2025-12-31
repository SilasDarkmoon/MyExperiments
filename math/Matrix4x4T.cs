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
public struct Matrix4x4T : IEquatable<Matrix4x4T>, IEquatableApprox<Matrix4x4T, float>
{
    private Matrix4x4 Value;
    private Matrix4x4T(Matrix4x4 value)
    {
        Value = value;
    }
    public Matrix4x4T(float m00, float m10, float m20, float m30, float m01, float m11, float m21, float m31, float m02, float m12, float m22, float m32, float m03, float m13, float m23, float m33)
    {
        Value = new Matrix4x4(m00, m10, m20, m30, m01, m11, m21, m31, m02, m12, m22, m32, m03, m13, m23, m33);
    }
    public readonly void Deconstruct(out float m00, out float m10, out float m20, out float m30, out float m01, out float m11, out float m21, out float m31, out float m02, out float m12, out float m22, out float m32, out float m03, out float m13, out float m23, out float m33)
    {
        m00 = Value.M11;
        m10 = Value.M12;
        m20 = Value.M13;
        m30 = Value.M14;
        m01 = Value.M21;
        m11 = Value.M22;
        m21 = Value.M23;
        m31 = Value.M24;
        m02 = Value.M31;
        m12 = Value.M32;
        m22 = Value.M33;
        m32 = Value.M34;
        m03 = Value.M41;
        m13 = Value.M42;
        m23 = Value.M43;
        m33 = Value.M44;
    }
    public static implicit operator Matrix4x4T((float m00, float m10, float m20, float m30, float m01, float m11, float m21, float m31, float m02, float m12, float m22, float m32, float m03, float m13, float m23, float m33) tuple)
    {
        return new Matrix4x4T(tuple.m00, tuple.m10, tuple.m20, tuple.m30, tuple.m01, tuple.m11, tuple.m21, tuple.m31, tuple.m02, tuple.m12, tuple.m22, tuple.m32, tuple.m03, tuple.m13, tuple.m23, tuple.m33);
    }
    public Matrix4x4T(Vector4E col0, Vector4E col1, Vector4E col2, Vector4E col3)
        : this(col0.X, col0.Y, col0.Z, col0.W, col1.X, col1.Y, col1.Z, col1.W, col2.X, col2.Y, col2.Z, col2.W, col3.X, col3.Y, col3.Z, col3.W)
    {
    }
    public readonly void Deconstruct(out Vector4E col0, out Vector4E col1, out Vector4E col2, out Vector4E col3)
    {
        col0 = new Vector4E(Value.M11, Value.M12, Value.M13, Value.M14);
        col1 = new Vector4E(Value.M21, Value.M22, Value.M23, Value.M24);
        col2 = new Vector4E(Value.M31, Value.M32, Value.M33, Value.M34);
        col3 = new Vector4E(Value.M41, Value.M42, Value.M43, Value.M44);
    }
    public static implicit operator Matrix4x4T((Vector4E col0, Vector4E col1, Vector4E col2, Vector4E col3) tuple)
    {
        return new Matrix4x4T(tuple.col0, tuple.col1, tuple.col2, tuple.col3);
    }
    public Matrix4x4T(Vector3E scale, QuaternionE rotation, Vector3E translation)
    {
        var mat = Matrix4x4.CreateScale(scale);
        mat = mat * Matrix4x4.CreateFromQuaternion(rotation.Conjugate());
        mat.Translation = translation;
        Value = mat;
    }
    public readonly void Deconstruct(out Vector3E scale, out QuaternionE rotation, out Vector3E translation)
    {
        Decompose(out scale, out rotation, out translation);
    }
    public static implicit operator Matrix4x4T((Vector3E scale, QuaternionE rotation, Vector3E translation) tuple)
    {
        return new Matrix4x4T(tuple.scale, tuple.rotation, tuple.translation);
    }
    public static explicit operator Matrix4x4T(Matrix4x4 value) { return new Matrix4x4T(value); }
    public static explicit operator Matrix4x4(Matrix4x4T value) { return value.Value; }
    public static explicit operator Matrix4x4T(Matrix4x4E value) { return new Matrix4x4T(value.Value); }
    public static explicit operator Matrix4x4E(Matrix4x4T value) { return value.Value; }

    public float M00 { readonly get => Value.M11; set => Value.M11 = value; }
    public float M10 { readonly get => Value.M12; set => Value.M12 = value; }
    public float M20 { readonly get => Value.M13; set => Value.M13 = value; }
    public float M30 { readonly get => Value.M14; set => Value.M14 = value; }
    public float M01 { readonly get => Value.M21; set => Value.M21 = value; }
    public float M11 { readonly get => Value.M22; set => Value.M22 = value; }
    public float M21 { readonly get => Value.M23; set => Value.M23 = value; }
    public float M31 { readonly get => Value.M24; set => Value.M24 = value; }
    public float M02 { readonly get => Value.M31; set => Value.M31 = value; }
    public float M12 { readonly get => Value.M32; set => Value.M32 = value; }
    public float M22 { readonly get => Value.M33; set => Value.M33 = value; }
    public float M32 { readonly get => Value.M34; set => Value.M34 = value; }
    public float M03 { readonly get => Value.M41; set => Value.M41 = value; }
    public float M13 { readonly get => Value.M42; set => Value.M42 = value; }
    public float M23 { readonly get => Value.M43; set => Value.M43 = value; }
    public float M33 { readonly get => Value.M44; set => Value.M44 = value; }

    public readonly bool EQApprox(Matrix4x4T other, float epsilon = NumberF.ComparisonEpsilon)
    {
        return Matrix4x4E.EQApprox(Value, other.Value, epsilon);
    }
    public readonly bool NEApprox(Matrix4x4T other, float epsilon = NumberF.ComparisonEpsilon)
    {
        return Matrix4x4E.NEApprox(Value, other.Value, epsilon);
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
            Matrix4x4T mt => Matrix4x4E.EQApprox(Value, mt.Value),
            _ => false,
        };
    }
    public override readonly string ToString()
    {
        return $"{{ {{M00:{Value.M11} M01:{Value.M21} M02:{Value.M31} M03:{Value.M41}}} {{M10:{Value.M12} M11:{Value.M22} M12:{Value.M32} M13:{Value.M42}}} {{M20:{Value.M13} M21:{Value.M23} M22:{Value.M33} M23:{Value.M43}}} {{M30:{Value.M14} M31:{Value.M24} M32:{Value.M34} M33:{Value.M44}}} }}";
    }

    #region IEquatable<T>
    public readonly bool Equals(Matrix4x4T other)
    {
        return Matrix4x4E.EQApprox(Value, other.Value);
    }
    #endregion
    #region IEquatableApprox
    bool IEquatableApprox.EQApprox(object other, double epsilon)
    {
        return other is Matrix4x4T othermat && EQApprox(othermat, (float)epsilon);
    }
    bool IEquatableApprox.NEApprox(object other, double epsilon)
    {
        return other is not Matrix4x4T othermat || NEApprox(othermat, (float)epsilon);
    }
    #endregion

    #region operators
    #region op - equal
    public static bool operator ==(Matrix4x4T left, Matrix4x4T right)
    {
        return Matrix4x4E.EQApprox(left.Value, right.Value);
    }
    public static bool operator !=(Matrix4x4T left, Matrix4x4T right)
    {
        return Matrix4x4E.NEApprox(left.Value, right.Value);
    }
    #endregion
    #region op - arithmetic
    public static Matrix4x4T operator +(Matrix4x4T left, Matrix4x4T right)
    {
        return new Matrix4x4T(left.Value + right.Value);
    }
    public static Matrix4x4T operator -(Matrix4x4T left, Matrix4x4T right)
    {
        return new Matrix4x4T(left.Value - right.Value);
    }
    public static Matrix4x4T operator *(Matrix4x4T left, Matrix4x4T right)
    {
        return new Matrix4x4T(right.Value * left.Value);
    }
    public static Matrix4x4T operator /(Matrix4x4T left, Matrix4x4T right)
    {
        Matrix4x4.Invert(right.Value, out var invright);
        return new Matrix4x4T(invright * left.Value);
    }
    public static Matrix4x4T operator *(Matrix4x4T left, float right)
    {
        return new Matrix4x4T(left.Value * right);
    }
    public static Matrix4x4T operator *(float left, Matrix4x4T right)
    {
        return new Matrix4x4T(right.Value * left);
    }
    public static Matrix4x4T operator /(Matrix4x4T left, float right)
    {
        return new Matrix4x4T(left.Value * (1f / right));
    }
    public static Matrix4x4T operator -(Matrix4x4T value)
    {
        return new Matrix4x4T(-value.Value);
    }
    public static Matrix4x4T operator +(Matrix4x4T value)
    {
        return value;
    }
    public static Matrix4x4T operator !(Matrix4x4T value)
    {
        return value.Invert();
    }
    public static Matrix4x4T operator ~(Matrix4x4T value)
    {
        return value.Transpose();
    }
    #endregion
    #region op - transform
    public static Vector4E operator *(Matrix4x4T m, Vector4E v)
    {
        return Vector4.Transform(v.Value, m.Value);
    }
    public static Vector3E operator *(Matrix4x4T m, Vector3E v)
    {
        return Vector3.Transform(v.Value, m.Value);
    }
    public static Vector3E operator ^(Matrix4x4T m, Vector3E v)
    {
        return Vector3.TransformNormal(v.Value, m.Value);
    }
    #endregion
    #endregion

    #region Consts
    public static readonly Matrix4x4T Zero = new Matrix4x4T(0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f);
    public static readonly Matrix4x4T Identity = new Matrix4x4T(Matrix4x4.Identity);
    // Extra Consts
    #endregion

    #region statics from .net Matrix
    public static Matrix4x4T Add(Matrix4x4T value1, Matrix4x4T value2)
    {
        return (Matrix4x4T)Matrix4x4.Add(value1.Value, value2.Value);
    }
    public static Matrix4x4T Concatenate(Matrix4x4T value1, Matrix4x4T value2)
    {
        return new Matrix4x4T(value1.Value * value2.Value);
    }
    public static Matrix4x4T CreateBillboard(Vector3E objectPosition, Vector3E cameraPosition, Vector3E cameraUpVector, Vector3E cameraForwardVector)
    {
        return (Matrix4x4T)Matrix4x4.CreateBillboard(objectPosition.Value, cameraPosition.Value, cameraUpVector.Value, cameraForwardVector.Value);
    }
    public static Matrix4x4T CreateConstrainedBillboard(Vector3E objectPosition, Vector3E cameraPosition, Vector3E rotateAxis, Vector3E cameraForwardVector, Vector3E objectForwardVector)
    {
        return (Matrix4x4T)Matrix4x4.CreateConstrainedBillboard(objectPosition.Value, cameraPosition.Value, rotateAxis.Value, cameraForwardVector.Value, objectForwardVector.Value);
    }
    public static Matrix4x4T CreateFromAxisAngleRadians(Vector3E axis, float radians)
    {
        return (Matrix4x4T)Matrix4x4.CreateFromAxisAngle(axis.Value, radians);
    }
    public static Matrix4x4T CreateFromQuaternion(QuaternionE quaternion)
    {
        return (Matrix4x4T)Matrix4x4.CreateFromQuaternion(quaternion.Value.Conjugate());
    }
    public static Matrix4x4T CreateFromYawPitchRoll(float yawYLast, float pitchXSecond, float rollZFirst)
    {
        return (Matrix4x4T)Matrix4x4.CreateFromYawPitchRoll(yawYLast, pitchXSecond, rollZFirst);
    }
    public static Matrix4x4T CreateLookAtRH(Vector3E cameraPosition, Vector3E cameraTarget, Vector3E cameraUpVector)
    {
        return (Matrix4x4T)Matrix4x4.CreateLookAt(cameraPosition.Value, cameraTarget.Value, cameraUpVector.Value);
    }
    public static Matrix4x4T CreateLookAt(Vector3E cameraPosition, Vector3E cameraTarget, Vector3E cameraUpVector)
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
        return (Matrix4x4T)rmat;
    }
    public static Matrix4x4T CreateLookToRH(Vector3E cameraPosition, Vector3E cameraDirection, Vector3E cameraUpVector)
    {
        return (Matrix4x4T)Matrix4x4.CreateLookAt(cameraPosition.Value, cameraPosition.Value + cameraDirection.Value, cameraUpVector.Value);
    }
    public static Matrix4x4T CreateLookTo(Vector3E cameraPosition, Vector3E cameraDirection, Vector3E cameraUpVector)
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
        return (Matrix4x4T)rmat;
    }
    public static Matrix4x4T CreateOrthographicRH(float width, float height, float zNearPlane, float zFarPlane)
    {
        return (Matrix4x4T)Matrix4x4.CreateOrthographic(width, height, zNearPlane, zFarPlane);
    }
    public static Matrix4x4T CreateOrthographic(float width, float height, float zNearPlane, float zFarPlane)
    {
        var rmat = Matrix4x4.CreateOrthographic(width, height, zNearPlane, zFarPlane);
        rmat.M33 = -rmat.M33;
        return (Matrix4x4T)rmat;
    }
    public static Matrix4x4T CreateOrthographicOffCenterRH(float left, float right, float bottom, float top, float zNearPlane, float zFarPlane)
    {
        return (Matrix4x4T)Matrix4x4.CreateOrthographicOffCenter(left, right, bottom, top, zNearPlane, zFarPlane);
    }
    public static Matrix4x4T CreateOrthographicOffCenter(float left, float right, float bottom, float top, float zNearPlane, float zFarPlane)
    {
        var rmat = Matrix4x4.CreateOrthographicOffCenter(left, right, bottom, top, zNearPlane, zFarPlane);
        rmat.M33 = -rmat.M33;
        return (Matrix4x4T)rmat;
    }
    public static Matrix4x4T CreatePerspectiveRH(float width, float height, float nearPlaneDistance, float farPlaneDistance)
    {
        return (Matrix4x4T)Matrix4x4.CreatePerspective(width, height, nearPlaneDistance, farPlaneDistance);
    }
    public static Matrix4x4T CreatePerspective(float width, float height, float nearPlaneDistance, float farPlaneDistance)
    {
        var rmat = Matrix4x4.CreatePerspective(width, height, nearPlaneDistance, farPlaneDistance);
        rmat.M33 = -rmat.M33;
        return (Matrix4x4T)rmat;
    }
    public static Matrix4x4T CreatePerspectiveFieldOfViewRH(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
    {
        return (Matrix4x4T)Matrix4x4.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance);
    }
    public static Matrix4x4T CreatePerspectiveFieldOfView(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
    {
        var rmat = Matrix4x4.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance);
        rmat.M33 = -rmat.M33;
        return (Matrix4x4T)rmat;
    }
    public static Matrix4x4T CreatePerspectiveOffCenterRH(float left, float right, float bottom, float top, float nearPlaneDistance, float farPlaneDistance)
    {
        return (Matrix4x4T)Matrix4x4.CreatePerspectiveOffCenter(left, right, bottom, top, nearPlaneDistance, farPlaneDistance);
    }
    public static Matrix4x4T CreatePerspectiveOffCenter(float left, float right, float bottom, float top, float nearPlaneDistance, float farPlaneDistance)
    {
        var rmat = Matrix4x4.CreatePerspectiveOffCenter(left, right, bottom, top, nearPlaneDistance, farPlaneDistance);
        rmat.M33 = -rmat.M33;
        return (Matrix4x4T)rmat;
    }
    public static Matrix4x4T CreateViewportRH(float x, float y, float width, float height, float minDepth, float maxDepth)
    {
        float wx = width * 0.5f;
        float wy = height * 0.5f;
        return new Matrix4x4T(wx, 0f, 0f, 0f,
            0f, -wy, 0f, 0f,
            0f, 0f, minDepth - maxDepth, 0f,
            wx + x, wy + y, minDepth, 1f);
    }
    public static Matrix4x4T CreateViewport(float x, float y, float width, float height, float minDepth, float maxDepth)
    {
        float wx = width * 0.5f;
        float wy = height * 0.5f;
        return new Matrix4x4T(wx, 0f, 0f, 0f,
            0f, -wy, 0f, 0f,
            0f, 0f, maxDepth - minDepth, 0f,
            wx + x, wy + y, minDepth, 1f);
    }
    public static Matrix4x4T CreateReflection(Plane value)
    {
        return (Matrix4x4T)Matrix4x4.CreateReflection(value);
    }
    public static Matrix4x4T CreateRotationX(float radians)
    {
        return (Matrix4x4T)Matrix4x4.CreateRotationX(radians);
    }
    public static Matrix4x4T CreateRotationX(float radians, Vector3E centerPoint)
    {
        return (Matrix4x4T)Matrix4x4.CreateRotationX(radians, centerPoint.Value);
    }
    public static Matrix4x4T CreateRotationY(float radians)
    {
        return (Matrix4x4T)Matrix4x4.CreateRotationY(radians);
    }
    public static Matrix4x4T CreateRotationY(float radians, Vector3E centerPoint)
    {
        return (Matrix4x4T)Matrix4x4.CreateRotationY(radians, centerPoint.Value);
    }
    public static Matrix4x4T CreateRotationZ(float radians)
    {
        return (Matrix4x4T)Matrix4x4.CreateRotationZ(radians);
    }
    public static Matrix4x4T CreateRotationZ(float radians, Vector3E centerPoint)
    {
        return (Matrix4x4T)Matrix4x4.CreateRotationZ(radians, centerPoint.Value);
    }
    public static Matrix4x4T CreateScale(float scale)
    {
        return (Matrix4x4T)Matrix4x4.CreateScale(scale);
    }
    public static Matrix4x4T CreateScale(float scale, Vector3E centerPoint)
    {
        return (Matrix4x4T)Matrix4x4.CreateScale(scale, centerPoint.Value);
    }
    public static Matrix4x4T CreateScale(float xScale, float yScale, float zScale)
    {
        return (Matrix4x4T)Matrix4x4.CreateScale(xScale, yScale, zScale);
    }
    public static Matrix4x4T CreateScale(float xScale, float yScale, float zScale, Vector3E centerPoint)
    {
        return (Matrix4x4T)Matrix4x4.CreateScale(xScale, yScale, zScale, centerPoint.Value);
    }
    public static Matrix4x4T CreateScale(Vector3E scales)
    {
        return (Matrix4x4T)Matrix4x4.CreateScale(scales.Value);
    }
    public static Matrix4x4T CreateScale(Vector3E scales, Vector3E centerPoint)
    {
        return (Matrix4x4T)Matrix4x4.CreateScale(scales.Value, centerPoint.Value);
    }
    public static Matrix4x4T CreateShadow(Vector3E lightDirection, Plane plane)
    {
        return (Matrix4x4T)Matrix4x4.CreateShadow(lightDirection.Value, plane);
    }
    public static Matrix4x4T CreateTranslation(float xPosition, float yPosition, float zPosition)
    {
        return (Matrix4x4T)Matrix4x4.CreateTranslation(xPosition, yPosition, zPosition);
    }
    public static Matrix4x4T CreateTranslation(Vector3E position)
    {
        return (Matrix4x4T)Matrix4x4.CreateTranslation(position.Value);
    }
    public static Matrix4x4T CreateWorld(Vector3E position, Vector3E forward, Vector3E up)
    {
        return (Matrix4x4T)Matrix4x4.CreateWorld(position.Value, forward.Value, up.Value);
    }
    public static Matrix4x4T CreateCompose(Vector3E scale, QuaternionE rotation, Vector3E translation)
    {
        var mat = Matrix4x4.CreateScale(scale);
        mat = mat * Matrix4x4.CreateFromQuaternion(rotation.Conjugate());
        mat.Translation = translation;
        return (Matrix4x4T)mat;
    }
    public static bool Decompose(Matrix4x4T matrix, out Vector3E scale, out QuaternionE rotation, out Vector3E translation)
    {
        bool success = Matrix4x4.Decompose(matrix.Value, out var s, out var r, out var t);
        scale = s;
        rotation = r.Conjugate();
        translation = t;
        return success;
    }
    public static (Vector3E scale, QuaternionE rotation, Vector3E translation) Decompose(Matrix4x4T matrix)
    {
        bool success = Matrix4x4.Decompose(matrix.Value, out var s, out var r, out var t);
        return (s, r, t);
    }
    public static bool Invert(Matrix4x4T matrix, out Matrix4x4T result)
    {
        bool success = Matrix4x4.Invert(matrix.Value, out var inv);
        result = (Matrix4x4T)inv;
        return success;
    }
    public static Matrix4x4T Invert(Matrix4x4T matrix)
    {
        bool success = Matrix4x4.Invert(matrix.Value, out var inv);
        return (Matrix4x4T)inv;
    }
    public static Matrix4x4T Lerp(Matrix4x4T matrix1, Matrix4x4T matrix2, float amount)
    {
        return (Matrix4x4T)Matrix4x4.Lerp(matrix1.Value, matrix2.Value, amount);
    }
    public static Matrix4x4T Multiply(Matrix4x4T value1, Matrix4x4T value2)
    {
        return (Matrix4x4T)Matrix4x4.Multiply(value2.Value, value1.Value);
    }
    public static Matrix4x4T Multiply(Matrix4x4T value1, float value2)
    {
        return (Matrix4x4T)Matrix4x4.Multiply(value1.Value, value2);
    }
    public static Matrix4x4T Negate(Matrix4x4T value)
    {
        return (Matrix4x4T)Matrix4x4.Negate(value.Value);
    }
    public static Matrix4x4T Subtract(Matrix4x4T value1, Matrix4x4T value2)
    {
        return (Matrix4x4T)Matrix4x4.Subtract(value1.Value, value2.Value);
    }
    public static Matrix4x4T Transform(Matrix4x4T value, QuaternionE rotation)
    {
        return (Matrix4x4T)Matrix4x4.Transform(value.Value, rotation.Value.Conjugate());
    }
    public static Matrix4x4T Transpose(Matrix4x4T matrix)
    {
        return (Matrix4x4T)Matrix4x4.Transpose(matrix.Value);
    }
    // Transform
    public static Vector2E Transform(Vector2E position, Matrix4x4T matrix)
    {
        return Vector2.Transform(position.Value, matrix.Value);
    }
    public static Vector2E Transform(Matrix4x4T matrix, Vector2E position)
    {
        return Vector2.Transform(position.Value, matrix.Value);
    }
    public static Vector2E TransformNormal(Vector2E position, Matrix4x4T matrix)
    {
        return Vector2.TransformNormal(position.Value, matrix.Value);
    }
    public static Vector2E TransformNormal(Matrix4x4T matrix, Vector2E position)
    {
        return Vector2.TransformNormal(position.Value, matrix.Value);
    }
    public static Vector3E Transform(Vector3E position, Matrix4x4T matrix)
    {
        return Vector3.Transform(position.Value, matrix.Value);
    }
    public static Vector3E Transform(Matrix4x4T matrix, Vector3E position)
    {
        return Vector3.Transform(position.Value, matrix.Value);
    }
    public static Vector3E TransformNormal(Vector3E position, Matrix4x4T matrix)
    {
        return Vector3.TransformNormal(position.Value, matrix.Value);
    }
    public static Vector3E TransformNormal(Matrix4x4T matrix, Vector3E position)
    {
        return Vector3.TransformNormal(position.Value, matrix.Value);
    }
    public static Vector4E Transform(Vector4E position, Matrix4x4T matrix)
    {
        return Vector4.Transform(position.Value, matrix.Value);
    }
    public static Vector4E Transform(Matrix4x4T matrix, Vector4E position)
    {
        return Vector4.Transform(position.Value, matrix.Value);
    }
    public static Vector4E TransformPoint(Matrix4x4T matrix, Vector2E value)
    {
        return Vector4.Transform(value.Value, matrix.Value);
    }
    public static Vector4E TransformPoint(Vector2E value, Matrix4x4T matrix)
    {
        return Vector4.Transform(value.Value, matrix.Value);
    }
    public static Vector4E TransformPoint(Matrix4x4T matrix, Vector3E value)
    {
        return Vector4.Transform(value.Value, matrix.Value);
    }
    public static Vector4E TransformPoint(Vector3E value, Matrix4x4T matrix)
    {
        return Vector4.Transform(value.Value, matrix.Value);
    }
    #endregion

    #region instance equivalent to static methods
    public readonly Matrix4x4T Concatenate(Matrix4x4T next)
    {
        return new Matrix4x4T(Value * next.Value);
    }
    public readonly bool Decompose(out Vector3E scale, out QuaternionE rotation, out Vector3E translation)
    {
        bool success = Matrix4x4.Decompose(Value, out var s, out var r, out var t);
        scale = s;
        rotation = r.Conjugate();
        translation = t;
        return success;
    }
    public readonly (Vector3E scale, QuaternionE rotation, Vector3E translation) Decompose()
    {
        bool success = Matrix4x4.Decompose(Value, out var s, out var r, out var t);
        return (s, r, t);
    }
    public readonly bool Invert(out Matrix4x4T result)
    {
        bool success = Matrix4x4.Invert(Value, out var inv);
        result = (Matrix4x4T)inv;
        return success;
    }
    public readonly Matrix4x4T Invert()
    {
        bool success = Matrix4x4.Invert(Value, out var inv);
        return (Matrix4x4T)inv;
    }
    public readonly Matrix4x4T Lerp(Matrix4x4T matrix2, float amount)
    {
        return (Matrix4x4T)Matrix4x4.Lerp(Value, matrix2.Value, amount);
    }
    public readonly Matrix4x4T Negate()
    {
        return (Matrix4x4T)Matrix4x4.Negate(Value);
    }
    public readonly Matrix4x4T Transform(QuaternionE rotation)
    {
        return (Matrix4x4T)Matrix4x4.Transform(Value, rotation.Value.Conjugate());
    }
    public readonly Matrix4x4T Transpose()
    {
        return (Matrix4x4T)Matrix4x4.Transpose(Value);
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
        return Quaternion.CreateFromRotationMatrix(Value).Conjugate();
    }
    #endregion

    #region index
    public readonly float GetElementAt(int col, int row)
    {
        return Matrix4x4E.GetElementAt(Value, col, row);
    }
    public void SetElementAt(int col, int row, float value)
    {
        Matrix4x4E.SetElementAt(ref Value, col, row, value);
    }
    public readonly Matrix4x4T WithElement(int col, int row, float value)
    {
        var mat = Value;
        Matrix4x4E.SetElementAt(ref mat, col, row, value);
        return new Matrix4x4T(mat);
    }
    public readonly float GetElementAt(int index)
    {
        return Matrix4x4E.GetElementAt(Value, index);
    }
    public void SetElementAt(int index, float value)
    {
        Matrix4x4E.SetElementAt(ref Value, index, value);
    }
    public readonly Matrix4x4T WithElement(int index, float value)
    {
        var mat = Value;
        Matrix4x4E.SetElementAt(ref mat, index, value);
        return new Matrix4x4T(mat);
    }
    public float this[int col, int row]
    {
        readonly get { return Matrix4x4E.GetElementAt(Value, col, row); }
        set { Matrix4x4E.SetElementAt(ref Value, col, row, value); }
    }
    public readonly Matrix4x4T this[int col, int row, float newvalue] => WithElement(col, row, newvalue);
    public float this[int index]
    {
        readonly get { return Matrix4x4E.GetElementAt(Value, index); }
        set { Matrix4x4E.SetElementAt(ref Value, index, value); }
    }
    public readonly Matrix4x4T this[int index, float newvalue] => WithElement(index, newvalue);
    #endregion

    #region Arithmetic Methods
    public readonly bool IsIdentity
    {
        get
        {
            return Matrix4x4E.EQApprox(Value, Matrix4x4T.Identity.Value);
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
