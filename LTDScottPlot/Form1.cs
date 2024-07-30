using ScottPlot.WinForms;
using ScottPlot;
using MotionProfiler;
namespace LTDScottPlot;

public partial class Form1 : Form
{
    private readonly FormsPlot formsPlot = new();
    private readonly Plot plot;
    
    
    public Form1()
    {
        InitializeComponent();
        Width = 1000;
        Height = 800;
        formsPlot.Dock = DockStyle.Fill;
        Controls.Add(formsPlot);

        plot = formsPlot.Plot;

        CamFixedPoint p1 = new(0, 0, 0, 0);
        CamFixedPoint p2 = new(13680, 19700, 100000, 0);
        var profile = ProfileGen.CalcCamPolynomial(p1, p2, 5);
        Console.WriteLine(profile);
        var fp = plot.Add.Function(x => profile.Evaluate(x));
        fp.MinX = 0;
        fp.MaxX = 13680;
        
        // var fps = plot.Add.Function((x) => 239400000 * Math.Pow(x, 3) - 971240.1055 * Math.Pow(x, 2));
        // fps.MinX = 0;
        // fps.MaxX = 0.1;
        //
        // var fpv = plot.Add.Function(x => 3 * 23940000 * Math.Pow(x, 2) - 2 * 971240.1055 * x);
        // fpv.MinX = 0;
        // fpv.MaxX = 0.1;
        //
        // var fpa = plot.Add.Function(x => 6 * 23940000 * x - 2 * 971240.1055);
        // fpa.MinX = 0;
        // fpa.MaxX = 0.1;
        
        plot.Axes.AutoScale();
    }
}