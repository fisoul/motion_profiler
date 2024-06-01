namespace MotionProfile;

public struct CamFixedPoint
{
    public CamFixedPoint() { }
    public CamFixedPoint(double x, double y, double y1, double y2)
    {
        X = x;
        Y = y;
        Y1 = y1;
        Y2 = y2;
    }
    public double X { get; set; }
    public double Y { get; set; }
    public double Y1 { get; set; }
    public double Y2 { get; set; }

    public readonly override string ToString()
    {
        return $"x={X:F3}, y={Y:F3}, y1={Y1:F3}, y2={Y2:F3}";
    }
}