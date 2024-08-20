namespace MotionProfiler.BRAutomat;

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