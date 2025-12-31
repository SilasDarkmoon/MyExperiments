namespace gameplay.math;

public interface IEquatableApprox
{
    bool EQApprox(object other, double epsilon);
    bool NEApprox(object other, double epsilon);
}
public interface IEquatableApprox<T, TE> : IEquatableApprox where TE : IEquatable<TE>
{
    bool EQApprox(T other, TE epsilon);
    bool NEApprox(T other, TE epsilon);
}

public static class ComparerApprox
{
    public class EqualityApproxImp<T, TE> : IEqualityComparer<T>
        where T : IEquatableApprox<T, TE>
        where TE : IEquatable<TE>
    {
        public TE Epsilon;
        public EqualityApproxImp(TE epsilon)
        {
            Epsilon = epsilon;
        }

        public bool Equals(T? x, T? y)
        {
            if (x is null)
            {
                return y is null;
            }
            if (y is null)
            {
                return false;
            }
            return x.EQApprox(y, Epsilon);
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }

        public TD CreateDelegate<TD>() where TD : Delegate
        {
            Func<T, T, bool> temp = Equals;
            return (TD)Delegate.CreateDelegate(typeof(TD), this, temp.Method);
        }
    }

    public static EqualityApproxImp<T, TE> Equality<T, TE>(TE epsilon)
        where T : IEquatableApprox<T, TE>
        where TE : IEquatable<TE>
    {
        return new EqualityApproxImp<T, TE>(epsilon);
    }
    public static EqualityApproxImp<T, float> Equality<T>(float epsilon)
        where T : IEquatableApprox<T, float>
    {
        return new EqualityApproxImp<T, float>(epsilon);
    }
    public static EqualityApproxImp<T, double> Equality<T>(double epsilon)
        where T : IEquatableApprox<T, double>
    {
        return new EqualityApproxImp<T, double>(epsilon);
    }
}

// TODO: int Compare(a, b)
