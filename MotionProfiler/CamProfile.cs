namespace MotionProfiler;

/// <summary>
/// Represents a cam profile for motion control.
/// Consists of continuous Cam Polynomials
/// </summary>
public class CamProfile
{
    public int MasterPeriod { get; set; }
    public int SlavePeriod { get; set; }
    // B&R ACP10 Limited to 128 (64, prior to V5.030)
    public int PolynomialNumber => PolynomialData.Count;

    // B&R ACP10 Limited to 128 (64, prior to V5.030)
    public List<CamPolynomial> PolynomialData { get; set; } = [];
    public CamFixedPoint[]? RefPoints { get; set; }

    /// <summary>
    /// Construct from given points
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
        // MasterPeriod is Integer, MasterFactor is Integer, so new MasterPeriod is always Integer
        MasterPeriod = (int)RefPoints[-1].X;
        SlavePeriod = (int)RefPoints[-1].Y;
    }
    
    /// <summary>
    /// Construct from calculated polynomials
    /// </summary>
    public CamProfile(int masterPeriod, int slavePeriod, IEnumerable<CamPolynomial> polynomials)
    {
        MasterPeriod = masterPeriod;
        SlavePeriod = slavePeriod;
        PolynomialData.AddRange(polynomials);
    }

    public override string ToString()
    {
        var ret = string.Empty;
        double interval = 0;
        foreach (var poly in PolynomialData)
        {
            ret += poly;
            ret += $" [{interval:N2} - {interval + poly.XMax:N2}]";
            ret += Environment.NewLine;
            interval += poly.XMax;
        }
        return ret;
    }
    
    /// <summary>
    /// Stretch the Profile and returns a new Profile
    /// </summary>
    /// <param name="xStretch">stretch of x</param>
    /// <param name="yStretch">stretch of y</param>
    /// <returns></returns>
    public CamProfile Stretch(int xStretch, int yStretch)
    {
        List<CamPolynomial> newPoly = [];
        newPoly.AddRange(PolynomialData.Select(poly => poly.Stretch(xStretch, yStretch)));
        return new CamProfile(MasterPeriod * xStretch, SlavePeriod * yStretch, newPoly);
    }
    
    /// <summary>
    /// Evaluates the profile value at given x
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
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
    /// Calculates the derivative of the CamProfile by differentiating each CamPolynomial in the profile.
    /// </summary>
    /// <param name="order">The order of the derivative to calculate. Default is 1.</param>
    /// <returns>A new CamProfile object that is the derivative of the current profile.</returns>
    public CamProfile Differentiate(int order = 1)
    {
        List<CamPolynomial> newPoly = [];
        newPoly.AddRange(PolynomialData.Select(poly => poly.Differentiate(order)));
        return new CamProfile(MasterPeriod, SlavePeriod, newPoly);
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

    /// <summary>
    /// Generates a symmetric speed shift CamProfile.
    /// </summary>
    /// <param name="ra">The ratio between the amplitude of the speed shift and the period (default is 0.5).</param>
    /// <param name="order">The order of the CamPolynomial (default is 3).</param>
    /// <param name="direction">The direction of the speed shift (0 for acceleration, 1 for deceleration) (default is 0).</param>
    /// <returns>The generated CamProfile object representing the symmetric speed shift.</returns>
    public static CamProfile SymmetricSpeedShift(double ra, int order = 3, int direction = 0)
    {
        var (p1, p2) = ProfileGen.CalcSymmetricShift(ra, order, direction);
        CamFixedPoint p0, p3;
        if (direction == 0)
        {
            p0 = new CamFixedPoint(0);
            p3 = new CamFixedPoint(1, 1, 2);
        }
        else
        {
            p0 = new CamFixedPoint(0, 0, 2);
            p3 = new CamFixedPoint(1, 1);
        }
        var poly1 = ProfileGen.CalcCamPolynomial(p0, p1);
        var poly2 = ProfileGen.CalcCamPolynomial(p1, p2);
        var poly3 = ProfileGen.CalcCamPolynomial(p2, p3);
        var ret = new CamProfile(1, 1, [poly1, poly2, poly3]);
        ret.RefPoints = [p1, p2];
        return ret;
    }
}