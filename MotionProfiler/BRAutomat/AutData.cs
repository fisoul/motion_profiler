namespace MotionProfiler.BRAutomat;



public struct AutDataEvent
{
    public EventAttr Attribute { get; set; }
    public EventType Type { get; set; }
    public uint Action { get; set; }
    public byte NextState { get; set; }
}

public struct AutCompLimits
{
    public double MinMasterCompDistance;
    public double MaxSlaveCompDistance;
    public double MinSlaveCompDistance;
    public double MaxSlaveCompVelocity;
    public double MinSlaveCompVelocity;
    public double MaxSlaveAccelComp1;
    public double MinSlaveAccelComp2;
    public double SlaveCompJoltTime;
}

public struct AutDataState()
{
    public CamProfile? CamProfile;
    public int MasterFactor = 1;
    public int SlaveFactor = 1;
    public CompMode CompMode = CompMode.ncOFF;
    public int MasterCompDistance = 0;
    public int SlaveCompDistance = 0;
    public AutCompLimits CompLimits;
    public double MasterCamLeadIn = 0;
    
    // public AutDataEvent[] Event = new AutDataEvent[5];
    // public ushort RepeatCounterInit = 0;
    // public ushort RepeatCounterSet = 0;
    // ReSharper disable once InconsistentNaming
    // public ushort MasterParID = 0;
    // public ushort ExtendedCompLimits = 0;
}

public class AutData
{
    public AutData()
    {
        for (var i = 0; i < 15; i++)
        {
            State[i] = new AutDataState();
        }
    }
    public AutDataState[] State = new AutDataState[15];
}