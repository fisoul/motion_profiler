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

    public string GetString(int n)
    {
        return n switch
        {
            4 => $"x={X:N4}, y={Y:N4}, y1={Y1:N4}, y2={Y2:N4}",
            5 => $"x={X:N5}, y={Y:N5}, y1={Y1:N5}, y2={Y2:N5}",
            6 => $"x={X:N6}, y={Y:N6}, y1={Y1:N6}, y2={Y2:N6}",
            _ => ToString()
        };
    }
}