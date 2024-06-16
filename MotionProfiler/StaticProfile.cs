namespace MotionProfiler;

public class StaticProfile
{
    private int masterFactor;
    public int MasterFactor
    {
        get => masterFactor;
        set
        {
            masterFactor = value;
            Update();
        }
    }

    private int slaveFactor;
    public int SlaveFactor
    {
        get => slaveFactor;
        set
        {
            slaveFactor = value;
            Update();
        }
    }

    private double masterVelocity;
    public double MasterVelocity
    {
        get => masterVelocity;
        set
        {
            masterVelocity = value;
            Update();
        }
    }
    
    private CamProfile camProfile;
    private List<CamPolynomial> polyPosition = [];
    private List<CamPolynomial> polyVelocity = [];
    private List<CamPolynomial> polyAcceleration = [];
    private List<CamPolynomial> polyJerk = [];
    
    public StaticProfile(CamProfile camProfile, int masterFactor = 1, int slaveFactor = 1, double masterVelocity = 1)
    {
        this.camProfile = camProfile;
        this.masterFactor = masterFactor;
        this.slaveFactor = slaveFactor;
        this.masterVelocity = masterVelocity;
        Update();
    }

    private void Update()
    {
        polyPosition.Clear();
        foreach (var poly in camProfile.PolynomialData)
        {
            var newPoly = poly.Stretch(masterFactor / masterVelocity, slaveFactor);
            polyPosition.Add(newPoly);
            polyVelocity.Add(newPoly.Differentiate());
            polyAcceleration.Add(newPoly.Differentiate(2));
            polyJerk.Add(newPoly.Differentiate(3));
        }
    }

    public double EvaluatePosition(double t)
    {
        double xMax = 0;
        foreach (var poly in polyPosition)
        {
            xMax += poly.XMax;
            if (t <= xMax)
                return poly.Evaluate(t);
        }
        return 0;
    }
    
    public double EvaluateVelocity(double t)
    {
        double xMax = 0;
        foreach (var poly in polyVelocity)
        {
            xMax += poly.XMax;
            if (t <= xMax)
                return poly.Evaluate(t);
        }
        return 0;
    }
    
    public double EvaluateAcceleration(double t)
    {
        double xMax = 0;
        foreach (var poly in polyAcceleration)
        {
            xMax += poly.XMax;
            if (t <= xMax)
                return poly.Evaluate(t);
        }
        return 0;
    }
    
    public double EvaluateJerk(double t)
    {
        double xMax = 0;
        foreach (var poly in polyJerk)
        {
            xMax += poly.XMax;
            if (t <= xMax)
                return poly.Evaluate(t);
        }
        return 0;
    }
}