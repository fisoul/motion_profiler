namespace MotionProfiler;

public struct CamFixedPoint (double x = 0, double y = 0, double y1 = 0, double y2 = 0)
{
    public double X = x;
    public double Y = y;
    public double Y1 = y1;
    public double Y2 = y2;

    public readonly override string ToString()
    {
        return $"x={X:N3}, y={Y:N3}, y1={Y1:N3}, y2={Y2:N3}";
    }
}