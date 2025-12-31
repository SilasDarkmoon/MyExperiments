using System.Numerics;
using System.Runtime.InteropServices;

namespace gameplay.math;

[StructLayout(LayoutKind.Sequential)]
public struct Transform : IEquatable<Transform>, IEquatableApprox<Transform, float>
{
    public QuaternionE Rotation = QuaternionE.Identity;
    public Vector3E Position = Vector3E.Zero;

    public Transform(QuaternionE rot)
    {
        Rotation = rot;
        Position = Vector3E.Zero;
    }
    public Transform(Vector3E pos)
    {
        Rotation = QuaternionE.Identity;
        Position = pos;
    }
    public Transform(QuaternionE rot, Vector3E pos)
    {
        Rotation = rot;
        Position = pos;
    }
    public readonly void Deconstruct(out QuaternionE rot, out Vector3E pos)
    {
        rot = Rotation;
        pos = Position;
    }
    public static implicit operator Transform((QuaternionE rot, Vector3E pos) tuple)
    {
        return new Transform(tuple.rot, tuple.pos);
    }
    public Transform(Matrix4x4E mat)
    {
        (var s, var r, var t) = mat;
        Rotation = r;
        Position = t;
    }
    public Transform(Matrix4x4T mat)
    {
        (var s, var r, var t) = mat;
        Rotation = r;
        Position = t;
    }
    public static implicit operator Matrix4x4(Transform trans)
    {
        return trans.ToMatrix();
    }
    public static implicit operator Matrix4x4E(Transform trans)
    {
        return trans.ToMatrix();
    }
    public static implicit operator Matrix4x4T(Transform trans)
    {
        return trans.ToMatrixT();
    }
    public static explicit operator Transform(Matrix4x4 mat)
    {
        return new Transform(mat);
    }
    public static explicit operator Transform(Matrix4x4E mat)
    {
        return new Transform(mat);
    }
    public static explicit operator Transform(Matrix4x4T mat)
    {
        return new Transform(mat);
    }

    public static bool EQApprox(Transform tf1, Transform tf2, float epsilon = NumberF.ComparisonEpsilon)
    {
        return tf1.Rotation.EQApprox(tf2.Rotation, epsilon) && tf1.Position.EQApprox(tf2.Position, epsilon);
    }
    public static bool NEApprox(Transform tf1, Transform tf2, float epsilon = NumberF.ComparisonEpsilon)
    {
        return tf1.Rotation.NEApprox(tf2.Rotation, epsilon) || tf1.Position.NEApprox(tf2.Position, epsilon);
    }
    public readonly bool EQApprox(Transform other, float epsilon = NumberF.ComparisonEpsilon)
    {
        return EQApprox(this, other, epsilon);
    }
    public readonly bool NEApprox(Transform other, float epsilon = NumberF.ComparisonEpsilon)
    {
        return NEApprox(this, other, epsilon);
    }

    public override readonly string ToString()
    {
        return $"Rotation {Rotation}, Position {Position}";
    }
    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Rotation.GetHashCode(), Position.GetHashCode());
    }
    public override readonly bool Equals(object? obj)
    {
        return obj is Transform other && EQApprox(this, other);
    }

    #region IEquatable<T>
    public readonly bool Equals(Transform other)
    {
        return EQApprox(this, other);
    }
    #endregion

    #region IEquatableApprox
    readonly bool IEquatableApprox.EQApprox(object other, double epsilon)
    {
        return other is Transform othertf && EQApprox(this, othertf, (float)epsilon);
    }
    readonly bool IEquatableApprox.NEApprox(object other, double epsilon)
    {
        return other is not Transform othertf || NEApprox(this, othertf, (float)epsilon);
    }
    #endregion

    public static readonly Transform Identity = new Transform(QuaternionE.Identity);

    #region operators
    #region op - equal
    public static bool operator ==(Transform left, Transform right)
    {
        return EQApprox(left, right);
    }
    public static bool operator !=(Transform left, Transform right)
    {
        return NEApprox(left, right);
    }
    #endregion
    #region op - concat
    public static Transform operator *(Transform nextParent, Transform firstLocal)
    {
        return Concatenate(firstLocal, nextParent);
    }

    /// <summary>
    /// 设 r * right = left，则此运算符的结果为r。
    /// 这不是用来获取left相对于right的相对Transform，如有此需求，请使用左移运算符或Relative方法
    /// </summary>
    public static Transform operator /(Transform left, Transform right)
    {
        return left * right.Inverse();
    }
    public static Transform operator <<(Transform target, Transform source)
    {
        return Relative(target, source);
    }
    public static Transform operator >>(Transform source, Transform target)
    {
        return Relative(target, source);
    }
    public static Transform operator !(Transform tf)
    {
        return tf.Inverse();
    }
    #endregion
    #region op - transform
    public static Vector3E operator *(Transform tf, Vector3E pointLocal)
    {
        return tf.TransformPoint(pointLocal);
    }
    public static Vector3E operator *(Vector3E pointWorld, Transform tf)
    {
        return tf.TransformInverse(pointWorld);
    }
    public static Vector3E operator ^(Transform tf, Vector3E dirLocal)
    {
        return tf.TransformNormal(dirLocal);
    }
    public static Vector3E operator ^(Vector3E dirWorld, Transform tf)
    {
        return tf.TransformNormalInverse(dirWorld);
    }
    #endregion
    #endregion

    #region Convert instance func
    public readonly Matrix4x4E ToMatrix()
    {
        return Matrix4x4E.CreateCompose(Vector3E.One, Rotation, Position);
    }
    public readonly Matrix4x4T ToMatrixT()
    {
        return Matrix4x4T.CreateCompose(Vector3E.One, Rotation, Position);

    }
    #endregion

    #region Arithmetic Methods
    public static Transform Relative(Transform final, Transform relativeTo)
    {
        return relativeTo.Inverse() * final;
    }
    public readonly Transform Relative(Transform to)
    {
        return Relative(this, to);
    }
    public static Vector3E TransformPoint(Transform tf, Vector3E pointLocal)
    {
        return tf.Rotation * pointLocal + tf.Position;
    }
    public readonly Vector3E TransformPoint(Vector3E pointLocal)
    {
        return TransformPoint(this, pointLocal);
    }
    public static Vector3E TransformInverse(Transform tf, Vector3E pointWorld)
    {
        return tf.Rotation.Inverse() * (pointWorld - tf.Position);
    }
    public readonly Vector3E TransformInverse(Vector3E pointWorld)
    {
        return TransformInverse(this, pointWorld);
    }
    public static Vector3E TransformNormal(Transform tf, Vector3E dirLocal)
    {
        return tf.Rotation * dirLocal;
    }
    public readonly Vector3E TransformNormal(Vector3E dirLocal)
    {
        return TransformNormal(this, dirLocal);
    }
    public static Vector3E TransformNormalInverse(Transform tf, Vector3E dirWorld)
    {
        return tf.Rotation.Inverse() * dirWorld;
    }
    public readonly Vector3E TransformNormalInverse(Vector3E dirWorld)
    {
        return TransformNormalInverse(this, dirWorld);
    }
    public static Transform Concatenate(Transform firstLocal, Transform nextParent)
    {
        var pos = nextParent.Rotation * firstLocal.Position + nextParent.Position;
        return new Transform(nextParent.Rotation * firstLocal.Rotation, pos);
    }
    public readonly Transform ConcatenateParent(Transform nextParent)
    {
        return Concatenate(this, nextParent);
    }
    public readonly Transform ConcatenateChild(Transform child)
    {
        return Concatenate(child, this);
    }
    public static Transform Inverse(Transform tf)
    {
        var invrot = tf.Rotation.Inverse();
        var invpos = -tf.Position;
        invpos = invrot * invpos;
        return new Transform(invrot, invpos);
    }
    public readonly Transform Inverse()
    {
        return Inverse(this);
    }
    #endregion
}
