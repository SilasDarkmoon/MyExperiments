using System.Numerics;
using System.Runtime.InteropServices;

namespace gameplay.math;

[StructLayout(LayoutKind.Explicit)]
public struct TransformScaled : IEquatable<TransformScaled>, IEquatable<Transform>, IEquatableApprox<TransformScaled, float>, IEquatableApprox<Transform, float>
{
    [FieldOffset(0)]
    private Vector3E _Scale = Vector3E.One;
    [FieldOffset(32)]
    private Transform _RotationTranslation = Transform.Identity;
    [FieldOffset(0)]
    private Matrix4x4E _Composed;
    [FieldOffset(64)]
    private bool _IsIndecomposable;

    public readonly bool IsDecomposable => !_IsIndecomposable;
    public Vector3E Scale
    {
        readonly get
        {
            if (_IsIndecomposable)
            {
                var (rowX, rowY, rowZ, _) = _Composed;
                return new Vector3E(rowX.Length(), rowY.Length(), rowZ.Length());
            }
            else
            {
                return _Scale;
            }
        }
        set
        {
            if (_IsIndecomposable)
            {
                var (rowX, rowY, rowZ, rowW) = _Composed;
                rowX = rowX.Normalize() * value.X;
                rowY = rowY.Normalize() * value.Y;
                rowZ = rowZ.Normalize() * value.Z;
                _Composed = new Matrix4x4E(rowX, rowY, rowZ, rowW);
            }
            else
            {
                _Scale = value;
            }
        }
    }
    public QuaternionE Rotation
    {
        readonly get
        {
            if (_IsIndecomposable)
            {
                var (rowX, rowY, rowZ, _) = _Composed;
                rowX = rowX.Normalize();
                rowY = rowY.Normalize();
                rowZ = rowZ.Normalize();
                var rmat = new Matrix4x4E(rowX, rowY, rowZ, Vector4E.UnitW);
                return rmat.ToQuaternion();
            }
            else
            {
                return _RotationTranslation.Rotation;
            }
        }
        set
        {
            if (_IsIndecomposable)
            {
                var (olds, _, oldt) = this;
                _IsIndecomposable = false;
                _Scale = olds;
                _RotationTranslation = new Transform(value, oldt);
            }
            else
            {
                _RotationTranslation.Rotation = value;
            }
        }
    }
    public Vector3E Position
    {
        readonly get { return _RotationTranslation.Position; }
        set { _RotationTranslation.Position = value; }
    }
    public Matrix4x4E Composed
    {
        readonly get
        {
            if (_IsIndecomposable)
            {
                return _Composed;
            }
            else
            {
                return new Matrix4x4E(_Scale, _RotationTranslation.Rotation, _RotationTranslation.Position);
            }
        }
        set
        {
            var success = Matrix4x4E.Decompose(value, out var s, out var r, out var t);
            if (success)
            {
                _IsIndecomposable = false;
                _Scale = s;
                _RotationTranslation = new Transform(r, t);
            }
            else
            {
                _IsIndecomposable = true;
                _Composed = value;
            }
        }
    }

    public static ref Vector3E RefScale(ref TransformScaled trans)
    {
        return ref trans._Scale;
    }
    public static ref QuaternionE RefRotation(ref TransformScaled trans)
    {
        return ref trans._RotationTranslation.Rotation;
    }
    public static ref Vector3E RefPosition(ref TransformScaled trans)
    {
        return ref trans._RotationTranslation.Position;
    }
    public static ref Matrix4x4E RefComposed(ref TransformScaled trans)
    {
        return ref trans._Composed;
    }

    public static bool CheckUniformScale(Vector3E scale)
    {
        return NumberF.EQApprox(scale.X, scale.Y) && NumberF.EQApprox(scale.X, scale.Z);
    }
    public readonly bool IsUniformScale()
    {
        return IsDecomposable && CheckUniformScale(_Scale);
    }
    public static bool CheckIdentityScale(Vector3E scale)
    {
        return NumberF.EQApprox(scale.X, 1.0f) && NumberF.EQApprox(scale.Y, 1.0f) && NumberF.EQApprox(scale.Z, 1.0f);
    }
    public readonly bool IsIdentityScale()
    {
        return IsDecomposable && CheckIdentityScale(_Scale);
    }

    public TransformScaled(Matrix4x4E mat)
    {
        Composed = mat;
    }
    public TransformScaled(Matrix4x4T mat)
    {
        if (Matrix4x4T.Decompose(mat, out var s, out var r, out var t))
        {
            _IsIndecomposable = false;
            _Scale = s;
            _RotationTranslation = new Transform(r, t);
        }
        else
        {
            _IsIndecomposable = true;
            _Composed = (Matrix4x4E)mat;
        }
    }

    public TransformScaled(Transform rnt)
    {
        _IsIndecomposable = false;
        _RotationTranslation = rnt;
    }
    public static implicit operator TransformScaled(Transform rnt)
    {
        return new TransformScaled(rnt);
    }
    public static explicit operator Transform(TransformScaled srt)
    {
        if (srt._IsIndecomposable)
        {
            var (_, r, t) = srt;
            return new Transform(r, t);
        }
        else
        {
            return srt._RotationTranslation;
        }
    }

    public TransformScaled(Vector3E scale, Transform rnt)
    {
        _IsIndecomposable = false;
        _Scale = scale;
        _RotationTranslation = rnt;
    }
    public readonly void Deconstruct(out Vector3E scale, out Transform rotationAndTranslation)
    {
        if (_IsIndecomposable)
        {
            var (s, r, t) = this;
            scale = s;
            rotationAndTranslation = new Transform(r, t);
        }
        else
        {
            scale = _Scale;
            rotationAndTranslation = _RotationTranslation;
        }
    }
    public static implicit operator TransformScaled((Vector3E scale, Transform rotationAndTranslation) tuple)
    {
        return new TransformScaled(tuple.scale, tuple.rotationAndTranslation);
    }

    public TransformScaled(Vector3E? scale = null, QuaternionE? rotation = null, Vector3E? position = null)
        : this(scale ?? Vector3E.One, rotation ?? QuaternionE.Identity, position ?? Vector3E.Zero)
    {
    }
    public TransformScaled(QuaternionE rotation, Vector3E position)
        : this(Vector3E.One, rotation, position)
    {
    }
    public TransformScaled(Vector3E scale, QuaternionE rotation, Vector3E position)
    {
        _IsIndecomposable = false;
        _Scale = scale;
        _RotationTranslation = new Transform(rotation, position);
    }
    public readonly void Deconstruct(out Vector3E scale, out QuaternionE rotation, out Vector3E position)
    {
        if (_IsIndecomposable)
        {
            var (rowX, rowY, rowZ, rowW) = _Composed;
            position = rowW;
            scale = new Vector3E(rowX.Length(), rowY.Length(), rowZ.Length());
            rowX /= scale.X;
            rowY /= scale.Y;
            rowZ /= scale.Z;
            var rmat = new Matrix4x4E(rowX, rowY, rowZ, Vector4E.UnitW);
            rotation = rmat.ToQuaternion();
        }
        else
        {
            scale = _Scale;
            rotation = _RotationTranslation.Rotation;
            position = _RotationTranslation.Position;
        }
    }
    public static implicit operator TransformScaled((Vector3E scale, QuaternionE rot, Vector3E pos) tuple)
    {
        return new TransformScaled(tuple.scale, tuple.rot, tuple.pos);
    }
    public static implicit operator Matrix4x4(TransformScaled trans)
    {
        return trans.ToMatrix();
    }
    public static implicit operator Matrix4x4E(TransformScaled trans)
    {
        return trans.ToMatrix();
    }
    public static implicit operator Matrix4x4T(TransformScaled trans)
    {
        return trans.ToMatrixT();
    }
    public static implicit operator TransformScaled(Matrix4x4 mat)
    {
        return new TransformScaled(mat);
    }
    public static implicit operator TransformScaled(Matrix4x4E mat)
    {
        return new TransformScaled(mat);
    }
    public static implicit operator TransformScaled(Matrix4x4T mat)
    {
        return new TransformScaled(mat);
    }

    public static bool EQApprox(TransformScaled tf1, TransformScaled tf2, float epsilon = NumberF.ComparisonEpsilon)
    {
        if (tf1._IsIndecomposable != tf2._IsIndecomposable)
        {
            return false;
        }
        if (tf1._IsIndecomposable)
        {
            return tf1._Composed.EQApprox(tf2._Composed, epsilon);
        }
        else
        {
            return tf1._Scale.EQApprox(tf2._Scale, epsilon) && tf1._RotationTranslation.EQApprox(tf2._RotationTranslation, epsilon);
        }
    }
    public static bool NEApprox(TransformScaled tf1, TransformScaled tf2, float epsilon = NumberF.ComparisonEpsilon)
    {
        if (tf1._IsIndecomposable != tf2._IsIndecomposable)
        {
            return true;
        }
        if (tf1._IsIndecomposable)
        {
            return tf1._Composed.NEApprox(tf2._Composed, epsilon);
        }
        else
        {
            return tf1._Scale.NEApprox(tf2._Scale, epsilon) || tf1._RotationTranslation.NEApprox(tf2._RotationTranslation, epsilon);
        }
    }
    public readonly bool EQApprox(TransformScaled other, float epsilon = NumberF.ComparisonEpsilon)
    {
        return EQApprox(this, other, epsilon);
    }
    public readonly bool NEApprox(TransformScaled other, float epsilon = NumberF.ComparisonEpsilon)
    {
        return NEApprox(this, other, epsilon);
    }
    public readonly bool EQApprox(Transform other, float epsilon = NumberF.ComparisonEpsilon)
    {
        return !_IsIndecomposable && _Scale.EQApprox(Vector3E.One, epsilon) && _RotationTranslation.EQApprox(other, epsilon);
    }
    public readonly bool NEApprox(Transform other, float epsilon = NumberF.ComparisonEpsilon)
    {
        return _IsIndecomposable || _Scale.NEApprox(Vector3E.One, epsilon) || _RotationTranslation.NEApprox(other, epsilon);
    }

    public override readonly string ToString()
    {
        if (_IsIndecomposable)
        {
            return $"Matrix {_Composed}";
        }
        else
        {
            return $"Scale {_Scale}, {_RotationTranslation}";
        }
    }
    public override readonly int GetHashCode()
    {
        if (_IsIndecomposable)
        {
            return _Composed.GetHashCode();
        }
        else
        {
            return HashCode.Combine(_Scale.GetHashCode(), _RotationTranslation.GetHashCode());
        }
    }
    public override readonly bool Equals(object? obj)
    {
        return obj switch
        {
            TransformScaled other => EQApprox(this, other),
            Transform other => EQApprox(other),
            _ => false,
        };
    }

    #region IEquatable<T>
    public readonly bool Equals(TransformScaled other)
    {
        return EQApprox(this, other);
    }
    public readonly bool Equals(Transform other)
    {
        return EQApprox(other);
    }
    #endregion

    #region IEquatableApprox
    readonly bool IEquatableApprox.EQApprox(object other, double epsilon)
    {
        return other switch
        {
            TransformScaled othertrans => EQApprox(this, othertrans, (float)epsilon),
            Transform othertrans => EQApprox(othertrans, (float)epsilon),
            _ => false,
        };
    }
    readonly bool IEquatableApprox.NEApprox(object other, double epsilon)
    {
        return other switch
        {
            TransformScaled othertrans => NEApprox(this, othertrans, (float)epsilon),
            Transform othertrans => NEApprox(othertrans, (float)epsilon),
            _ => true,
        };
    }
    #endregion

    public static readonly TransformScaled Identity = new TransformScaled(Vector3E.One, Transform.Identity);

    #region operators
    #region op - equal
    public static bool operator ==(TransformScaled left, TransformScaled right)
    {
        return EQApprox(left, right);
    }
    public static bool operator !=(TransformScaled left, TransformScaled right)
    {
        return NEApprox(left, right);
    }
    public static bool operator ==(TransformScaled left, Transform right)
    {
        return left.EQApprox(right);
    }
    public static bool operator !=(TransformScaled left, Transform right)
    {
        return left.NEApprox(right);
    }
    public static bool operator ==(Transform left, TransformScaled right)
    {
        return right.EQApprox(left);
    }
    public static bool operator !=(Transform left, TransformScaled right)
    {
        return right.NEApprox(left);
    }
    #endregion
    #region op - concat
    public static TransformScaled operator *(TransformScaled nextParent, TransformScaled firstLocal)
    {
        return Concatenate(firstLocal, nextParent);
    }
    public static TransformScaled operator /(TransformScaled left, TransformScaled right)
    {
        return left * right.Inverse();
    }
    public static TransformScaled operator <<(TransformScaled target, TransformScaled source)
    {
        return Relative(target, source);
    }
    public static TransformScaled operator >>(TransformScaled source, TransformScaled target)
    {
        return Relative(target, source);
    }
    public static TransformScaled operator !(TransformScaled tf)
    {
        return tf.Inverse();
    }
    #endregion
    #region op - transform
    public static Vector3E operator *(TransformScaled tf, Vector3E pointLocal)
    {
        return tf.TransformPoint(pointLocal);
    }
    public static Vector3E operator *(Vector3E pointWorld, TransformScaled tf)
    {
        return tf.TransformInverse(pointWorld);
    }
    public static Vector3E operator ^(TransformScaled tf, Vector3E dirLocal)
    {
        return tf.TransformNormal(dirLocal);
    }
    public static Vector3E operator ^(Vector3E dirWorld, TransformScaled tf)
    {
        return tf.TransformNormalInverse(dirWorld);
    }
    #endregion
    #endregion

    #region Convert instance func
    public Matrix4x4E ToMatrix()
    {
        return Composed;
    }
    public readonly Matrix4x4T ToMatrixT()
    {
        if (_IsIndecomposable)
        {
            return (Matrix4x4T)_Composed;
        }
        else
        {
            return new Matrix4x4T(_Scale, _RotationTranslation.Rotation, _RotationTranslation.Position);
        }
    }
    #endregion

    #region index and with
    public float ScaleX
    {
        get => _Scale.X;
        set => _Scale = _Scale.WithX(value);
    }
    public float ScaleY
    {
        get => _Scale.Y;
        set => _Scale = _Scale.WithY(value);
    }
    public float ScaleZ
    {
        get => _Scale.Z;
        set => _Scale = _Scale.WithZ(value);
    }
    public float RotationX
    {
        get => _RotationTranslation.Rotation.X;
        set => _RotationTranslation.Rotation = _RotationTranslation.Rotation.WithX(value);
    }
    public float RotationY
    {
        get => _RotationTranslation.Rotation.Y;
        set => _RotationTranslation.Rotation = _RotationTranslation.Rotation.WithY(value);
    }
    public float RotationZ
    {
        get => _RotationTranslation.Rotation.Z;
        set => _RotationTranslation.Rotation = _RotationTranslation.Rotation.WithZ(value);
    }
    public float RotationW
    {
        get => _RotationTranslation.Rotation.W;
        set => _RotationTranslation.Rotation = _RotationTranslation.Rotation.WithW(value);
    }
    public float PositionX
    {
        get => _RotationTranslation.Position.X;
        set => _RotationTranslation.Position = _RotationTranslation.Position.WithX(value);
    }
    public float PositionY
    {
        get => _RotationTranslation.Position.Y;
        set => _RotationTranslation.Position = _RotationTranslation.Position.WithY(value);
    }
    public float PositionZ
    {
        get => _RotationTranslation.Position.Z;
        set => _RotationTranslation.Position = _RotationTranslation.Position.WithZ(value);
    }
    #endregion

    #region Arithmetic Methods
    public static TransformScaled Relative(TransformScaled final, TransformScaled relativeTo)
    {
        return relativeTo.Inverse() * final;
    }
    public readonly TransformScaled Relative(TransformScaled to)
    {
        return Relative(this, to);
    }
    public static Vector3E TransformPoint(TransformScaled tf, Vector3E pointLocal)
    {
        if (tf._IsIndecomposable)
        {
            return tf.Composed.Transform(pointLocal);
        }
        else
        {
            return tf.Rotation * (tf.Scale * pointLocal) + tf.Position;
        }
    }
    public readonly Vector3E TransformPoint(Vector3E pointLocal)
    {
        return TransformPoint(this, pointLocal);
    }
    public static Vector3E TransformInverse(TransformScaled tf, Vector3E pointWorld)
    {
        if (tf._IsIndecomposable)
        {
            return tf.Composed.Invert().Transform(pointWorld);
        }
        else
        {
            return tf.Rotation.Inverse() * (pointWorld - tf.Position) / tf.Scale;
        }
    }
    public readonly Vector3E TransformInverse(Vector3E pointWorld)
    {
        return TransformInverse(this, pointWorld);
    }
    public static Vector3E TransformNormal(TransformScaled tf, Vector3E dirLocal)
    {
        if (tf._IsIndecomposable)
        {
            return tf.Composed.TransformNormal(dirLocal);
        }
        else
        {
            return tf.Rotation * (tf.Scale * dirLocal);
        }
    }
    public readonly Vector3E TransformNormal(Vector3E dirLocal)
    {
        return TransformNormal(this, dirLocal);
    }
    public static Vector3E TransformNormalInverse(TransformScaled tf, Vector3E dirWorld)
    {
        if (tf._IsIndecomposable)
        {
            return tf.Composed.Invert().TransformNormal(dirWorld);
        }
        else
        {
            return tf.Rotation.Inverse() * dirWorld / tf.Scale;
        }
    }
    public readonly Vector3E TransformNormalInverse(Vector3E dirWorld)
    {
        return TransformNormalInverse(this, dirWorld);
    }
    public static TransformScaled Concatenate(TransformScaled firstLocal, TransformScaled nextParent)
    {
        if (firstLocal._IsIndecomposable || nextParent._IsIndecomposable || !nextParent.IsUniformScale())
        {
            return firstLocal.ToMatrix().Concatenate(nextParent.ToMatrix());
        }
        else
        {
            var pos = nextParent.Rotation * (nextParent.Scale * firstLocal.Position) + nextParent.Position;
            return new TransformScaled(nextParent.Scale * firstLocal.Scale, nextParent.Rotation * firstLocal.Rotation, pos);
        }
    }
    public readonly TransformScaled ConcatenateParent(TransformScaled nextParent)
    {
        return Concatenate(this, nextParent);
    }
    public readonly TransformScaled ConcatenateChild(TransformScaled child)
    {
        return Concatenate(child, this);
    }
    public static TransformScaled Inverse(TransformScaled tf)
    {
        if (tf._IsIndecomposable || !tf.IsUniformScale())
        {
            return new TransformScaled(tf.ToMatrix().Invert());
        }
        else
        {
            var invrot = tf.Rotation.Inverse();
            var invpos = -tf.Position;
            var invscl = Vector3E.One / tf.Scale;
            invpos = invrot * invpos * invscl;
            return new TransformScaled(invscl, invrot, invpos);
        }
    }
    public readonly TransformScaled Inverse()
    {
        return Inverse(this);
    }
    #endregion
}

public static class TransformScaledRefExtensions
{
    public static ref Vector3E RefScale(this ref TransformScaled trans)
    {
        return ref TransformScaled.RefScale(ref trans);
    }
    public static ref QuaternionE RefRotation(this ref TransformScaled trans)
    {
        return ref TransformScaled.RefRotation(ref trans);
    }
    public static ref Vector3E RefPosition(this ref TransformScaled trans)
    {
        return ref TransformScaled.RefPosition(ref trans);
    }
    public static ref Matrix4x4E RefComposed(this ref TransformScaled trans)
    {
        return ref TransformScaled.RefComposed(ref trans);
    }
}
