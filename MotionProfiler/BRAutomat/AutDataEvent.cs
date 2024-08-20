namespace MotionProfiler.BRAutomat;

public struct AutDataEvent
{
    public EventAttr Attribute { get; set; }
    public EventType Type { get; set; }
    public uint Action { get; set; }
    public byte NextState { get; set; }
}