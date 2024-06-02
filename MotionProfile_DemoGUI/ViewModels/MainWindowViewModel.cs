using System;
using MotionProfile;

namespace MotionProfile_DemoGUI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public static string Greeting => "Welcome to Avalonia!";

    public CamFixedPoint P1 { get; set; } = new();
    private CamFixedPoint P2 { get; set; } = new();

    private CamPolynomial C1 = new();
    private CamPolynomial C2 = new();
    private CamPolynomial C3 = new();
    
    public void Test()
    {
        
        var (p1, p2) = CamProfiler.CalcSymmetricShift(0.2);

        Console.WriteLine(p1);
        Console.WriteLine(p2);

        Console.WriteLine(CamProfiler.CalcPolynomial(new CamFixedPoint(0,0,0,0), new CamFixedPoint(1,1,0,0)));

        var c1 = CamProfiler.CalcPolynomial(new CamFixedPoint(0, 0, 0, 0), p1);
        var c2 = CamProfiler.CalcPolynomial(p1, p2);
        var c3 = CamProfiler.CalcPolynomial(p2, new CamFixedPoint(1, 1, 0, 0));
        Console.WriteLine(c1);
        Console.WriteLine(c2);
        Console.WriteLine(c3);

    }
}