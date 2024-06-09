using System.Diagnostics;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace MotionProfiler;

public static class ProfileGen
{
    /// <summary>
    /// Calculates the polynomial curve that fits the given fixed points.
    /// </summary>
    /// <param name="p1">The first fixed point.</param>
    /// <param name="p2">The second fixed point.</param>
    /// <returns>The calculated polynomial curve.</returns>
    public static CamPolynomial CalcCamPolynomial(CamFixedPoint p1, CamFixedPoint p2)
    {
        var ret = new CamPolynomial(6);
        if (p1.Equals(p2))
            return ret;
        var m = Matrix<double>.Build.DenseOfArray((new[,] { { p1.Y }, { p2.Y }, { p1.Y1 }, { p2.Y1 }, { p1.Y2 }, { p2.Y2 } }));
        var t = Matrix<double>.Build.DenseOfArray(new[,]
        {
            { 1, p1.X, Math.Pow(p1.X, 2), Math.Pow(p1.X, 3), Math.Pow(p1.X, 4), Math.Pow(p1.X, 5) },
            { 1, p2.X, Math.Pow(p2.X, 2), Math.Pow(p2.X, 3), Math.Pow(p2.X, 4), Math.Pow(p2.X, 5) },
            { 0, 1, 2 * p1.X, 3 * Math.Pow(p1.X, 2), 4 * Math.Pow(p1.X, 3), 5 * Math.Pow(p1.X, 4) },
            { 0, 1, 2 * p2.X, 3 * Math.Pow(p2.X, 2), 4 * Math.Pow(p2.X, 3), 5 * Math.Pow(p2.X, 4) },
            { 0, 0, 2, 6 * p1.X, 12 * Math.Pow(p1.X, 2), 20 * Math.Pow(p1.X, 3)},
            { 0, 0, 2, 6 * p2.X, 12 * Math.Pow(p2.X, 2), 20 * Math.Pow(p2.X, 3)}
        });
        var a = t.Inverse() * m;
        var array = a.ToArray();
        ret.Coefficients[0] = array[0, 0];
        ret.Coefficients[1] = array[1, 0];
        ret.Coefficients[2] = array[2, 0];
        ret.Coefficients[3] = array[3, 0];
        ret.Coefficients[4] = array[4, 0];
        ret.Coefficients[5] = array[5, 0];
        ret.XMax = p2.X - p1.X;
        return ret;
    }
    
    /// <summary>
    /// Calculates the symmetric shift for a motion profile.
    /// </summary>
    /// <param name="ra">The ratio value between 0 and 0.5. Default value is 0.5.</param>
    /// <param name="order">The order of the symmetric acceleration curve. Can be 3 or 4. Default value is 3.</param>
    /// <param name="direction">The direction of the motion. 0 for positive direction, 1 for negative direction. Default value is 0.</param>
    /// <returns>The calculated symmetric shift as a tuple of two CamFixedPoint objects.</returns>
    public static (CamFixedPoint, CamFixedPoint) CalcSymmetricShift(double ra = 0.5, int order = 3, int direction = 0)
    {
        var p1 = new CamFixedPoint();
        var p2 = new CamFixedPoint();
        if (ra is < 0 or > 0.5)
        {
            ra = 0.5;
        }
        if (order is < 3 or > 4)
        {
            order = 3;
        }
        switch (order)
        {
            case 3:
                // t
                p1.X = ra;
                p2.X = 1 - ra;
                // a    
                p1.Y2 = 2 / (1 - ra);
                p2.Y2 = 2 / (1 - ra);
                // v
                p1.Y1 = ra / (1 - ra);
                p2.Y1 = (2 - 3 * ra) / (1 - ra);
                // s
                p1.Y = ra * ra / (3 * (1 - ra));
                p2.Y = ra * ra / (3 * (1 - ra)) + 1 - 2 * ra;
                break;
            case 4:
                // t
                p1.X = ra;
                p2.X = 1 - ra;
                // a
                p1.Y2 = 6 / (3 - 2 * ra);
                p2.Y2 = 6 / (3 - 2 * ra);
                // p1
                p1.Y1 = 4 * ra / (3 - 2 * ra);
                p2.Y1 = (6 - 8 * ra) / (3 - 2 * ra);
                // p2
                p1.Y = 3 * ra * ra / (2 * (3 - 2 * ra));
                p2.Y = (3 * ra * ra / (2 * (3 - 2 * ra))) + 1 - 2 * ra;
                break;
        }
        if (direction == 0) return (p1, p2);
        (p1.Y2, p2.Y2) = (-p1.Y2, -p2.Y2);
        (p1.Y1, p2.Y1) = (p2.Y1, p1.Y1);
        (p1.Y, p2.Y) = (1 - p2.Y, 1 - p1.Y);
        return (p1, p2);
    }
    
    /// <summary>
    /// 以下使用快捷方式获取预定义曲线
    /// </summary>
    /// <param name="ra"></param>
    /// <returns></returns>
    public static (CamFixedPoint, CamFixedPoint) 计算3阶对称加速曲线(double ra)
    {
        return CalcSymmetricShift(ra, 3, 0);
    }

    public static (CamFixedPoint, CamFixedPoint) 计算3阶对称减速曲线(double ra)
    {
        return CalcSymmetricShift(ra, 3, 1);
    }
    
    public static (CamFixedPoint, CamFixedPoint) 计算4阶对称加速曲线(double ra)
    {
        return CalcSymmetricShift(ra, 4, 0);
    }

    public static (CamFixedPoint, CamFixedPoint) 计算4阶对称减速曲线(double ra)
    {
        return CalcSymmetricShift(ra, 4, 1);
    }
}