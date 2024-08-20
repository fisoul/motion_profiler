namespace MotionProfiler.AxisModel;

public struct AxisLoad
{
    public int GearIn;
    public int GearOut;
    public int Revolution;
    public bool Direction;
}

public class Axis
{
    public AxisLoad Load;
    public float ActVelocity;
    public int Position;
    public float ActCurrent;
    public float ActTorque;
}