namespace MotionProfiler.BRAutomat;

public class AutControl
{
    // static mode with constant {MasterSpeed}
    public double MasterSpeed { get; set; }

    private AutData autData = new();
    public AutData Data
    {
        get => autData;
        set
        {
            autData = value;
            InitAutData();
        }
    }
    private List<CamProfile> scaledProfiles = [];

    public AutControl()
    {
        
    }

    public void InitAutData()
    {
        scaledProfiles.Clear();
        foreach (var state in Data.State)
        {
            if (state.CamProfile == null) break;
            scaledProfiles.Add(state.CamProfile.Stretch(state.MasterFactor, state.SlaveFactor));
        }
    }
    
    /// <summary>
    /// s = v * t
    /// </summary>
    /// <param name="x">x is time, time is related to speed</param>
    /// <returns></returns>
    public double Evaluate(double x)
    {
        return 0;
    }
}