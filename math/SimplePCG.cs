namespace gameplay.math;

/// <summary>
/// 简单的 PCG 随机数生成器
/// </summary>
sealed internal class SimplePCG
{
    private ulong state = 0x4d595df4d0f33173UL; // Initial state (can be seeded)
    // 常量参数：PCG 的推荐值
    private const ulong multiplier = 6364136223846793005UL;
    private ulong increment = 1442695040888963407UL;

    /// <summary>
    /// 初始化 PCG 随机数生成器
    /// </summary>
    /// <param name="seed">种子值</param>
    internal SimplePCG(ulong seed = 0)
    {
        state = seed + increment;
        // Discard the first value to enhance randomness
        Next();
    }

    static uint RotR32(uint x, int r)
    {
        return (x >> r) | (x << (-r & 31));
    }

    public uint Next()
    {
        ulong x = state;
        int count = (int)(x >> 59); // 59 = 64 - 5

        state = x * multiplier + increment;
        x ^= x >> 18;              // 18 = (64 - 27) / 2
        return RotR32((uint)(x >> 27), count); // 27 = 32 - 5
    }

    /// <summary>
    /// 生成一个 [min, max) 范围内的随机整数
    /// </summary>
    /// <param name="min">范围下限（包含）</param>
    /// <param name="max">范围上限（不包含）</param>
    /// <returns>范围内的随机整数</returns>
    internal int NextInt(int min, int max)
    {
        return min + (int)(NextDouble() * (max - min));
    }

    /// <summary>
    /// 生成一个 [0, 1) 范围的随机浮点数
    /// </summary>
    /// <returns></returns>
    internal float NextFloat()
    {
        var ret = Next() / (float)(uint.MaxValue + 1.0);
        if (ret == 1.0f)
        {
            ret = 0.999999f;
        }
        return ret;
    }

    /// <summary>
    /// 生成一个 [min, max) 范围内的随机浮点数
    /// </summary>
    internal float NextFloat(float min, float max)
    {
        return min + (max - min) * NextFloat();
    }

    /// <summary>
    /// 生成一个 [0, 1) 范围的双精度浮点数
    /// </summary>
    /// <returns>随机双精度浮点数</returns>
    internal double NextDouble()
    {
        return Next() / (double)(uint.MaxValue + 1.0); // 映射到 [0, 1)
    }

    /// <summary>
    /// 生成一个 [min, max) 范围内的随机双精度浮点数
    /// </summary>
    internal double NextDouble(double min, double max)
    {
        return min + (max - min) * NextDouble();
    }

    internal ulong GetState() => state;
    internal void SetState(ulong newState) => state = newState;

    internal ulong GetIncrement() => increment;
    internal void SetIncrement(ulong newIncrement) => increment = newIncrement | 1UL;

}
