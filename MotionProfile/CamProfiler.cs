using System.Diagnostics;
using MathNet.Numerics.LinearAlgebra;

namespace MotionProfile;

public static class CamProfiler
{
    public const bool DEBUG_PRINT = true;

    public static CamPolynomial CalcPolynomial(CamFixedPoint p1, CamFixedPoint p2)
    {
        CamPolynomial ret = new();
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

        Debug.WriteLineIf(DEBUG_PRINT, t);
        var array = a.ToArray();
        ret.a0 = array[0, 0];
        ret.a1 = array[1, 0];
        ret.a2 = array[2, 0];
        ret.a3 = array[3, 0];
        ret.a4 = array[4, 0];
        ret.a5 = array[5, 0];
        return ret;
    }

    public static (CamFixedPoint, CamFixedPoint) 计算3阶对称加速曲线(double ra)
    {
        return CalcSymmetricShift(ra, 3, 0);
    }

    public static (CamFixedPoint, CamFixedPoint) 计算4阶对称加速曲线(double ra)
    {
        return CalcSymmetricShift(ra, 4, 0);
    }

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
        var a = 2 / (1 - ra); // 2 is the only magic constant
        switch (order)
        {
            case 3:
                // t
                p1.X = ra;
                p2.X = 1 - ra;
                // a    
                p1.Y2 = p2.Y2 = a;
                // p1
                p1.Y1 = a * ra / 2;
                p1.Y = a * ra * ra / 6;
                // p2
                p2.Y1 = p1.Y1 + (1 - 2 * ra) * a;
                p2.Y = (p2.X - p1.X) * (p2.Y1 + p1.Y1) / 2 + p1.Y;
                break;

            case 4:
                // t
                p1.X = ra;
                p2.X = 1 - ra;
                // a
                p1.Y2 = p2.Y2 = 6 / (3 - 2 * ra);
                // p1
                p1.Y1 = 4 * ra / (3 - 2 * ra);
                p1.Y = 3 * ra * ra / (2 * (3 - 2 * ra));
                // p2
                p2.Y1 = (6 - 8 * ra) / (3 - 2 * ra);
                p2.Y = (3 * ra * ra / (2 * (3 - 2 * ra))) + 1 - 2 * ra;
                break;
        }
        return direction == 0 ? (p1, p2) : (p2, p1);
    }
}