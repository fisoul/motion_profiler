// See https://aka.ms/new-console-template for more information

using System.Text.RegularExpressions;
using MotionProfile;
using ScottPlot;
using ScottPlot.DataSources;
using ScottPlot.Plottables;

Console.WriteLine("自动化学习助手：运动曲线生成");

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


IFunctionSource fSource = new FunctionSource(CamPolynomial.ToFunction(c1));

Plot plot = new();
var fPlot = plot.Add.Function(CamPolynomial.ToFunction((c1)));
fPlot.MinX = 0;
fPlot.MaxX = p1.X;
plot.Axes.AutoScale();
plot.SavePng("Cam.png", 600, 400).LaunchFile();
plot.SavePng("Cam.png", 600, 400).LaunchInBrowser();

