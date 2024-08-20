using MathNet.Numerics.LinearAlgebra;
namespace MotionProfiler;

public static class ProfileGen
{
    public static CamProfile CreateCamProfile(IEnumerable<CamPolynomial> polynomials)
    {

        return null;
    }

    public static CamPolynomial? CalcCamPolynomialGeneric(CamFixedPoint p1, CamFixedPoint p2, MotionRule motionRule = MotionRule.PolynomialOrder5, double lambda = 0.5)
    {
        if (!CheckMotionRule(p1, p2, motionRule)) 
            return null;
        var ret = new CamPolynomial();
        switch (motionRule)
        {
            case MotionRule.StraightLine:
                break;
            case MotionRule.PolynomialOrder5:
                ret = Calc5thPolynomial(p1, p2);
                break;
            case MotionRule.Even:
                break;
            case MotionRule.QuadraticParabola:
                break;
            case MotionRule.SimpleSine:
                break;
            case MotionRule.SlopingSine:
                break;
            case MotionRule.ModifiedSine:
                break;
            case MotionRule.SimpleTrapezoid:
                break;
            case MotionRule.ModifiedTrapezoid:
                break;
            case MotionRule.HarmonicCombination:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(motionRule), motionRule, null);
        }
        return ret;
    }

    private static bool CheckMotionRule(CamFixedPoint p1, CamFixedPoint p2, MotionRule motionRule)
    {

        return true;
    }
    
    /// <summary>
    /// Calculates a CamPolynomial based on two CamFixedPoint objects and an order value.
    /// </summary>
    /// <param name="p1">The first CamFixedPoint object.</param>
    /// <param name="p2">The second CamFixedPoint object.</param>
    /// <param name="order">The order of the polynomial. Can be 3 or 5. Default value is 5.</param>
    /// <returns>A CamPolynomial object representing the calculated polynomial.</returns>
    // ReSharper disable once InconsistentNaming
    public static CamPolynomial Calc5thPolynomial(CamFixedPoint p1, CamFixedPoint p2, int order = 5)
    {
        var ret = new CamPolynomial(6);
        if (p1.Equals(p2))
            return ret;
        switch (order)
        {
            case 5:
                var m5 = Matrix<double>.Build.DenseOfArray(new[,] { { p1.Y }, { p2.Y }, { p1.Y1 }, { p2.Y1 }, { p1.Y2 }, { p2.Y2 } });
                var t5 = Matrix<double>.Build.DenseOfArray(new[,]
                {
                    { 1, p1.X, Math.Pow(p1.X, 2), Math.Pow(p1.X, 3), Math.Pow(p1.X, 4), Math.Pow(p1.X, 5) },
                    { 1, p2.X, Math.Pow(p2.X, 2), Math.Pow(p2.X, 3), Math.Pow(p2.X, 4), Math.Pow(p2.X, 5) },
                    { 0, 1, 2 * p1.X, 3 * Math.Pow(p1.X, 2), 4 * Math.Pow(p1.X, 3), 5 * Math.Pow(p1.X, 4) },
                    { 0, 1, 2 * p2.X, 3 * Math.Pow(p2.X, 2), 4 * Math.Pow(p2.X, 3), 5 * Math.Pow(p2.X, 4) },
                    { 0, 0, 2, 6 * p1.X, 12 * Math.Pow(p1.X, 2), 20 * Math.Pow(p1.X, 3)},
                    { 0, 0, 2, 6 * p2.X, 12 * Math.Pow(p2.X, 2), 20 * Math.Pow(p2.X, 3)}
                });
                var a5 = t5.Inverse() * m5;
                var array5 = a5.ToArray();
                ret.Coefficients[0] = array5[0, 0];
                ret.Coefficients[1] = array5[1, 0];
                ret.Coefficients[2] = array5[2, 0];
                ret.Coefficients[3] = array5[3, 0];
                ret.Coefficients[4] = array5[4, 0];
                ret.Coefficients[5] = array5[5, 0];
                break;
            case 3:
                var m3 = Matrix<double>.Build.DenseOfArray(new[,] { { p1.Y }, { p2.Y }, { p1.Y1 }, { p2.Y1 } });
                var t3 = Matrix<double>.Build.DenseOfArray(new[,]
                {
                    {Math.Pow(p1.X, 3), Math.Pow(p1.X, 2), p1.X, 1},
                    {Math.Pow(p2.X, 3), Math.Pow(p2.X, 2), p2.X, 1},
                    {3 * Math.Pow(p1.X, 2), 2 * p1.X, 1, 0},
                    {3 * Math.Pow(p2.X, 2), 2 * p2.X, 1, 0}
                });
                var a3 = t3.Inverse() * m3;
                var array3 = a3.ToArray();
                ret.Coefficients[0] = array3[0, 0];
                ret.Coefficients[1] = array3[1, 0];
                ret.Coefficients[2] = array3[2, 0];
                ret.Coefficients[3] = array3[3, 0];
                break;
        }
        ret.XMax = p2.X - p1.X;
        return ret;
    }

    public static List<CamPolynomial> CalcTrapezoid(double vIn, double vOut, double sMaster, double sSlave, 
                                                    double vMax, double vMin, double a1Max, double a2Max)
    {
        List<CamPolynomial> ret = [];
        
        return ret;
    }
    
    /// <summary>
    /// Calculates the symmetric shift for a motion profile.
    /// </summary>
    /// <param name="ra">The ratio value between 0 and 0.5. Default value is 0.5.</param>
    /// <param name="order">The order of the symmetric acceleration curve. Can be 3 or 4. Default value is 3.</param>
    /// <param name="vin">entry velocity of the profile, only positive value allowed</param>
    /// <param name="vout">exit velocity of the profile, only positive value allowed</param>
    /// <returns>The calculated symmetric shift as a tuple of two CamFixedPoint objects.</returns>
    public static (CamFixedPoint, CamFixedPoint) CalcSymmetricShift(double ra = 0.5, double vin = 0, double vout = 1, int order = 3)
    {
        var p1 = new CamFixedPoint();
        var p2 = new CamFixedPoint();
        if (ra < 0 || ra > 0.5 || order < 3 || order > 4 || (Math.Abs(vout - vin) < 0.1))
        {
            return (p1, p2);
        }
        var reverse = !(vout > vin);
        if (reverse)
            (vin, vout) = (vout, vin);
        var vDelta = vout - vin;
        var s = (vout + vin) / 2;
        double a;
        switch (order)
        {
            case 3:
                a = vDelta / (1 - ra);
                // t
                p1.X = ra;
                p2.X = 1 - ra;
                // a    
                p1.Y2 = a;
                p2.Y2 = a;
                // v
                p1.Y1 = a * ra / 2 + vin;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          
                p2.Y1 = p1.Y1 + a * (1 - 2 * ra);
                // s
                p1.Y = a * ra * ra / 6 + vin * ra;
                p2.Y = p1.Y + (p1.Y1 + p2.Y1) / 2 * (1 - 2 * ra);
                break;
            case 4:
                a = vDelta / (1 - 2 * ra / 3);
                // t
                p1.X = ra;
                p2.X = 1 - ra;
                // a
                p1.Y2 = a;
                p2.Y2 = a;
                // v
                //p1.Y1 = a * ra / 3 + vin; //开口向下
                p1.Y1 = 2 * a * ra / 3 + vin; //开口向上
                p2.Y1 = p1.Y1 + a * (1 - 2 * ra);
                // s
                //p1.Y = a * ra * ra / 12 + vin * ra; //开口向下
                p1.Y = a * ra * ra / 4 + vin * ra;
                p2.Y = p1.Y + (p1.Y1 + p2.Y1) / 2 * (1 - 2 * ra);
                break;
        }
        if (!reverse) return (p1, p2);
        (p1.Y2, p2.Y2) = (-p1.Y2, -p2.Y2);
        (p1.Y1, p2.Y1) = (p2.Y1, p1.Y1);
        (p1.Y, p2.Y) = (s - p2.Y, s - p1.Y);
        return (p1, p2);
    }
    
    /// <summary>
    /// 以下使用快捷方式获取预定义曲线
    /// </summary>
    /// <param name="ra"></param>
    /// <returns></returns>
    public static (CamFixedPoint, CamFixedPoint) 计算3阶对称加速曲线(double ra)
    {
        return CalcSymmetricShift(ra);
    }

    public static (CamFixedPoint, CamFixedPoint) 计算3阶对称减速曲线(double ra)
    {
        return CalcSymmetricShift(ra);
    }
    
    public static (CamFixedPoint, CamFixedPoint) 计算4阶对称加速曲线(double ra)
    {
        return CalcSymmetricShift(ra, 0, 1, 4);
    }

    public static (CamFixedPoint, CamFixedPoint) 计算4阶对称减速曲线(double ra)
    {
        return CalcSymmetricShift(ra, 0, 1, 4);
    }
}