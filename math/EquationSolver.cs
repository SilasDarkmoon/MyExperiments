namespace gameplay.math;

public static class EquationSolver
{
    private static float Cbrt(float v)
    {
        //return MathF.Pow(v, 1f / 3f);
        return (float)Math.Cbrt(v);
    }
    public static (float, float, float) SolveCubic(float a, float b, float c, float d)
    {
        float b_a = b / a;
        float c_a = c / a;
        float d_a = d / a;
        float part1 = -b_a * b_a * b_a / 27f;
        float part2 = b_a * c_a / 6f;
        float part3 = -d_a / 2f;
        float part4 = c_a / 3f;
        float part5 = -b_a * b_a / 9f;
        float q_2 = part1 + part2 + part3;
        float p_3 = part4 + part5;
        float delta = q_2 * q_2 + p_3 * p_3 * p_3;
        if (delta > 0)
        {
            float sqrt_delta = MathF.Sqrt(delta);
            float result = Cbrt(q_2 - sqrt_delta) + Cbrt(q_2 + sqrt_delta) - b_a / 3f;
            return (result, float.NaN, float.NaN);
        }
        else if (delta == 0)
        {
            float offset = b_a / 3f;
            float cbrt_q_2 = Cbrt(q_2);
            float result = cbrt_q_2 * 2 - offset;
            float result2 = -cbrt_q_2 - offset;
            return (result, result2, float.NaN);
        }
        else //if (delta < 0)
        {
            float offset = b_a / 3f;
            float negdelta = -delta;
            float len2 = negdelta + q_2 * q_2;
            float len = MathF.Sqrt(len2);
            float theta = MathF.Acos(q_2 / len);
            float cbrtlen = Cbrt(len);
            float theta_3 = theta / 3;
            float result1 = MathF.Cos(theta_3) * cbrtlen * 2 - offset;
            float pi2_3 = MathF.PI * 2f / 3f;
            float result2 = MathF.Cos(theta_3 + pi2_3) * cbrtlen * 2 - offset;
            float result3 = MathF.Cos(theta_3 - pi2_3) * cbrtlen * 2 - offset;
            return (result1, result2, result3);
        }
    }
    public static float SolveCubicFirstPositive(float a, float b, float c, float d)
    {
        (float x1, float x2, float x3) = SolveCubic(a, b, c, d);
        float selectedresult = x1;
        if (!float.IsNaN(x2))
        {
            if (x2 > 0)
            {
                if (selectedresult <= 0 || x2 < selectedresult)
                {
                    selectedresult = x2;
                }
            }
        }
        if (!float.IsNaN(x3))
        {
            if (x3 > 0)
            {
                if (selectedresult <= 0 || x3 < selectedresult)
                {
                    selectedresult = x3;
                }
            }
        }
        if (selectedresult <= 0)
        {
            return float.NaN;
        }
        else
        {
            return selectedresult;
        }
    }
    public static float SolveCubicNearest(float a, float b, float c, float d, float reftarget)
    {
        (float x1, float x2, float x3) = SolveCubic(a, b, c, d);
        float selectedresult = x1;
        if (!float.IsNaN(x2))
        {
            if (MathF.Abs(x2 - reftarget) < MathF.Abs(selectedresult - reftarget))
            {
                selectedresult = x2;
            }
        }
        if (!float.IsNaN(x3))
        {
            if (MathF.Abs(x3 - reftarget) < MathF.Abs(selectedresult - reftarget))
            {
                selectedresult = x3;
            }
        }
        return selectedresult;
    }
}
