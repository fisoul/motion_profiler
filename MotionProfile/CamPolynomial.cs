namespace MotionProfile;

public struct CamPolynomial
{
    public double a0, a1, a2, a3, a4, a5, a6;
    public double x_max;

    public static Func<double, double> ToFunction(CamPolynomial poly, int diffOrder = 0)
    {
        switch (diffOrder)
        {
            case 0:
                return x => poly.a6 * Math.Pow(x, 6) + poly.a5 * Math.Pow(x, 5) + poly.a4 * Math.Pow(x, 4) +
                            poly.a3 * Math.Pow(x, 3) + poly.a2 * Math.Pow(x, 2) + poly.a1 * x + poly.a0;
            case 1:
                return x => 6 * poly.a6 * Math.Pow(x, 5) + 5 * poly.a5 * Math.Pow(x, 4) + 4 * poly.a4 * Math.Pow(x, 3) +
                            3 * poly.a3 * Math.Pow(x, 2) + 2 * poly.a2 * Math.Pow(x, 1) + poly.a1;
            case 2:
                return x => 30 * poly.a6 * Math.Pow(x, 4) + 20 * poly.a5 * Math.Pow(x, 3) + 12 * poly.a4 * Math.Pow(x, 2) +
                            6 * poly.a3 * Math.Pow(x, 1) + 2 * poly.a2;
            case 3:
                return x => 120 * poly.a6 * Math.Pow(x, 3) + 60 * poly.a5 * Math.Pow(x, 2) + 24 * poly.a4 * Math.Pow(x, 1) + 6 * poly.a3;
            case 4:
                return x => 360 * poly.a6 * Math.Pow(x, 2) + 120 * poly.a5 * x + 24 * poly.a4;
            default:
                break;
        }
        return x => 1;
    }

    public readonly override string ToString()
    {
        var result = "";
        if (Math.Abs(a6) > 0)
        {
            result += (a6 > 0 ? " + " : " - ") + Math.Abs(a6).ToString(Math.Abs(a6) < 0.001 ? "0.###E+0" : "0.###") + "x^6";
        }
        if (Math.Abs(a5) > 0)
        {
            result += (a5 > 0 ? " + " : " - ") + Math.Abs(a5).ToString(Math.Abs(a5) < 0.001 ? "0.###E+0" : "0.###") + "x^5";
        }
        if (Math.Abs(a4) > 0)
        {
            result += (a4 > 0 ? " + " : " - ") + Math.Abs(a4).ToString(Math.Abs(a4) < 0.001 ? "0.###E+0" : "0.###") + "x^4";
        }
        if (Math.Abs(a3) > 0)
        {
            result += (a3 > 0 ? " + " : " - ") + Math.Abs(a3).ToString(Math.Abs(a3) < 0.001 ? "0.###E+0" : "0.###") + "x^3";
        }
        if (Math.Abs(a2) > 0)
        {
            result += (a2 > 0 ? " + " : " - ") + Math.Abs(a2).ToString(Math.Abs(a2) < 0.001 ? "0.###E+0" : "0.###") + "x^2";
        }
        if (Math.Abs(a1) > 0)
        {
            result += (a1 > 0 ? " + " : " - ") + Math.Abs(a1).ToString(Math.Abs(a1) < 0.001 ? "0.###E+0" : "0.###") + "x";
        }
        if (Math.Abs(a0) > 0)
        {
            result += (a0 > 0 ? " + " : " - ") + Math.Abs(a0).ToString(Math.Abs(a0) < 0.001 ? "0.###E+0" : "0.###");
        }

        if (result.Length <= 0) return "y = 0";
        if (result[0] != ' ') return "y = " + result;
        if (result[1] == '+')
        {
            return result.Remove(0, 3);
        }
        else
        {
            return result.Remove(2, 1).Remove(0, 1);
        }
    }
}