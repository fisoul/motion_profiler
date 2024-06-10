using System.Security.Cryptography;
using MathNet.Numerics;

namespace MotionProfiler;

public class CamProfile
{
    public int MasterPeriod { get; set; }
    public int SlavePeriod { get; set; }
    // B&R ACP10 Limited to 128 (64, prior to V5.030)
    public int PolynomialNumber => PolynomialData.Count;

    // B&R ACP10 Limited to 128 (64, prior to V5.030)
    public List<CamPolynomial> PolynomialData { get; set; } = [];
    public List<double> Interval { get; set; } = [];
    public CamFixedPoint[]? RefPoints { get; set; }

    /// <summary>
    /// Represents a cam profile for motion control.
    /// </summary>
    public CamProfile(IEnumerable<CamFixedPoint> points)
    {
        RefPoints = points.ToArray();
        Array.Sort(RefPoints, (p1, p2) => (int)(p1.X - p2.X));
        for (var i = 0; i < RefPoints.Length - 1; i++)
        {
            PolynomialData.Add(ProfileGen.CalcCamPolynomial(RefPoints[i], RefPoints[i + 1]));
        }
        // warning here, last [] x,y must be integer
        MasterPeriod = (int)RefPoints[-1].X;
        SlavePeriod = (int)RefPoints[-1].Y;
    }

    /// <summary>
    /// Represents a cam profile for motion control.
    /// </summary>
    public CamProfile(int masterPeriod, int slavePeriod, IEnumerable<CamPolynomial> polynomials)
    {
        MasterPeriod = masterPeriod;
        SlavePeriod = slavePeriod;
        PolynomialData.AddRange(polynomials);
        Interval.Add(0);
        double interval = 0;
        foreach (var poly in PolynomialData)
        {
            interval += poly.XMax;
            Interval.Add(interval);
        }
    }

    /// <summary>
    /// Returns a function that calculates the value of a motion profile at a given position.
    /// </summary>
    /// <param name="masterFactor">The multiplication factor for the master period.</param>
    /// <param name="slaveFactor">The multiplication factor for the slave period.</param>
    /// <param name="order">differential order</param>
    /// <param name="masterShift"></param>
    /// <param name="slaveShift"></param>
    /// <returns>A function that calculates the value of a motion profile at a given position.</returns>
    public Func<double, double> GetFunction(int masterFactor = 1, int slaveFactor = 1, int order = 0, int masterShift = 0, int slaveShift = 0)
    {
        return Ret;
        double Ret(double x)
        {
            for (var i = 0; i < PolynomialNumber; i++)
            {
                CamPolynomial? poly = null;
                if (x >= Interval[i] * masterFactor + masterShift && x <= Interval[i + 1] * masterFactor + masterShift)
                {
                    poly ??= PolynomialData[i].StretchThenTranslate(masterFactor, slaveFactor, masterShift, slaveShift).Differentiate(order);
                    return poly.Evaluate(x);
                }
            }
            return 0;
        }
    }

    public CamProfile StretchThenTranslate(int masterFactor, int slaveFactor, int masterOffset, int slaveOffset)
    {
        List<CamPolynomial> newPoly = [];
        newPoly.AddRange(PolynomialData.Select(poly => poly.StretchThenTranslate(masterFactor, slaveFactor, masterOffset, slaveOffset)));
        return new CamProfile(MasterPeriod * masterFactor, SlavePeriod * slaveFactor, newPoly);
    }
    
    public double Evaluate(double x)
    {
        double xMax = 0;
        foreach (var poly in PolynomialData)
        {
            xMax += poly.XMax;
            if (x <= xMax)
                return poly.Evaluate(x);
        }
        return 0;
    }
    
    /// <summary>
    /// Returns a straight line cam profile with a single polynomial.
    /// </summary>
    /// <returns>A straight line cam profile.</returns>
    public static CamProfile StraightLine()
    {
        CamPolynomial poly = new(2)
        {
            Coefficients =
            {
                [0] = 0,
                [1] = 1
            },
            XMax = 1
        };
        return new CamProfile(1, 1, new []{poly});
    }

    public static CamProfile SymmetricSpeedShift(double ra, int order = 3, int direction = 0)
    {
        var (p1, p2) = ProfileGen.CalcSymmetricShift(ra, order, direction);
        CamFixedPoint p0, p3;
        if (direction == 0)
        {
            p0 = new CamFixedPoint(0, 0, 0, 0);
            p3 = new CamFixedPoint(1, 1, 2, 0);
        }
        else
        {
            p0 = new CamFixedPoint(0, 0, 2, 0);
            p3 = new CamFixedPoint(1, 1, 0, 0);
        }

        var poly1 = ProfileGen.CalcCamPolynomial(p0, p1);
        var poly2 = ProfileGen.CalcCamPolynomial(p1, p2);
        var poly3 = ProfileGen.CalcCamPolynomial(p2, p3);

        return new CamProfile(1, 1, new CamPolynomial[] { poly1, poly2, poly3 });
    }
}