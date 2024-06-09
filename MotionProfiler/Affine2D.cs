namespace MotionProfiler;

public struct Affine2D (double xScale = 1, double yScale = 1, double xShift = 0, double yShift= 0)
{
    public double XShift = xShift;
    public double YShift = yShift;
    public double XScale = xScale;
    public double YScale = yScale;

    public override string ToString()
    {
        var ret = "y = ";
        if (Math.Abs(YScale - 1) > double.Epsilon)
            ret += $"{YScale:G} * [";

        ret += "f(";

        if (Math.Abs(XScale - 1) > double.Epsilon)
            ret += $"{XScale:G}";

        if (Math.Abs(XShift - 0) > double.Epsilon)
        {
            ret += "(x";
            ret += XShift > 0 ? "-" : "+";
            ret += $"{Math.Abs(XShift):G}))";
        }
        else
        {
            ret += "x)";
        }

        if (Math.Abs(YShift - 0) > double.Epsilon)
        {
            ret += YShift > 0 ? "+" : "-";
            ret += $"{Math.Abs(YShift):G}";
        }

        if (Math.Abs(YScale - 1) > double.Epsilon)
            ret += "]";
        return ret;
    }
}