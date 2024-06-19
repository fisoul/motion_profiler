namespace MotionProfiler;

public struct Interval
{
    public double X1 { get; set; }
    public double X2 { get; set; }
    public Interval(double x1, double x2)
    {
        if (x1 >= x2)
        {
            (x1, x2) = (x2, x1);
        }
        X1 = x1;
        X2 = x2;
    }
}

public class Piecewise
{
    public int IntervalCount { get; private set; }
    protected readonly Dictionary<Interval, Func<double, double>> functions = new Dictionary<Interval, Func<double, double>>();

    public void AddSection(Interval interval, Func<double, double> func)
    {
        functions.Add(interval, func);
        IntervalCount++;
    }

    public double Evaluate(double x)
    {
        return functions.First(f => x >= f.Key.X1 && x <= f.Key.X2).Value(x);
    }
}