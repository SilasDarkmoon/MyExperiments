namespace gameplay.math;

public class StatefulRandom
{
    private SimplePCG randomGenerator;
    private ulong seed;

    public StatefulRandom(ulong seed)
    {
        this.seed = seed;
        randomGenerator = new SimplePCG(seed);
    }

    /// <summary>
    /// 生成随机 uint
    /// </summary>
    /// <returns></returns>
    public uint Next()
    {
        return randomGenerator.Next();
    }

    /// <summary>
    /// 生成随机 int [min, max)
    /// </summary>
    /// <returns></returns>
    public int NextInt(int min, int max)
    {
        if (min == max)
        {
            return min;
        }
        else if (min > max)
        {
            (min, max) = (max, min);
        }
        return randomGenerator.NextInt(min, max);
    }

    /// <summary>
    /// 生成随机 float [0, 1)
    /// </summary>
    /// <returns></returns>
    public float NextFloat()
    {
        return randomGenerator.NextFloat();
    }

    /// <summary>
    /// 生成随机 float [min, max)
    /// </summary>
    /// <returns></returns>
    public float NextFloat(float min, float max)
    {
        return randomGenerator.NextFloat(min, max);
    }

    /// <summary>
    /// 生成随机 double [0, 1)
    /// </summary>
    /// <returns></returns>
    public double NextDouble()
    {
        return randomGenerator.NextDouble();
    }

    /// <summary>
    /// 生成随机 double [min, max)
    /// </summary>
    /// <returns></returns>
    public double NextDouble(double min, double max)
    {
        return randomGenerator.NextDouble(min, max);
    }

    // 保存完整状态
    public (ulong seed, ulong state) GetState()
    {
        return (seed, randomGenerator.GetState());
    }

    // 从完整状态恢复
    public void RestoreState(ulong state)
    {
        randomGenerator.SetState(state);
    }
}
