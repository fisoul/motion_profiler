
namespace MotionProfiler.BRAutomat;

public class AutCompensation
{
    private int x;
    private int y;
    private double v_x;
    private double v1;
    private double v2;
    private double v_max;
    private double v_min;
    private double a1_max;
    private double a2_max;

    public CamProfile CalcCompensation(double k1, double k2)
    {
        List<CamPolynomial> polynomials = [];

        v1 = k1 * v_x;
        v2 = k2 * v_x;
        var t = x / v_x;
        
        
        
        return new CamProfile(1, 1, polynomials);
    }
}