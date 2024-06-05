// See https://aka.ms/new-console-template for more information

using MotionProfile;
using ScottPlot;

Console.WriteLine("B&R CamAutomat Profiler");

int bagLength = 300000;
int accLength = 100000;
int decLength = 100000;
int syncLength = bagLength - accLength - decLength;

var (pAcc1, pAcc2) = CamProfiler.CalcSymmetricShift(0.2);
Console.WriteLine("Acceleration Profile:");
Console.WriteLine(pAcc1);
Console.WriteLine(pAcc2);
Console.WriteLine();
var curveAcc1 = CamProfiler.CalcPolynomial(new CamFixedPoint(0, 0, 0, 0), pAcc1);
var curveAcc2 = CamProfiler.CalcPolynomial(pAcc1, pAcc2);
var curveAcc3 = CamProfiler.CalcPolynomial(pAcc2, new CamFixedPoint(1, 1, 0, 0));
CamProfile accProfile = new (1, 1, [curveAcc1, curveAcc2, curveAcc3]);

var (pDec1, pDec2) = CamProfiler.CalcSymmetricShift(0.2, 3, 1);
Console.WriteLine("Deceleration Profile:");
Console.WriteLine(pDec1);
Console.WriteLine(pDec2);
Console.WriteLine();
var curveDec1 = CamProfiler.CalcPolynomial(new CamFixedPoint(0, 0, 0, 0), pDec1);
var curveDec2 = CamProfiler.CalcPolynomial(pDec1, pDec2);
var curveDec3 = CamProfiler.CalcPolynomial(pDec2, new CamFixedPoint(1, 1, 0, 0));
CamProfile decProfile = new (1, 1, [curveDec1, curveDec2, curveDec3]);

var syncProfile = CamProfile.PreDefined_FFFF();

// visualization using ScottPlot
Plot plot = new();
List<Func<double, double>> funcList = [];
List<double> xStart = [];
funcList.AddRange(accProfile.PolyNomialData.Select(poly => CamProfiler.GetFunction(poly, 0, 72000, 100000)));
xStart.AddRange(accProfile.PolyNomialData.Select(poly => poly.x_max));
funcList.AddRange(syncProfile.PolyNomialData.Select(poly => CamProfiler.GetFunction(poly, 0, 36000, 100000)));
xStart.AddRange(syncProfile.PolyNomialData.Select(poly => poly.x_max));
funcList.AddRange(decProfile.PolyNomialData.Select(poly => CamProfiler.GetFunction(poly, 0, 72000, 100000)));
xStart.AddRange(decProfile.PolyNomialData.Select(poly => poly.x_max));
for (var i = 1; i < xStart.Count; i++)
{
    xStart[i] += xStart[i - 1];
}
for (var i = 0; i < funcList.Count - 1; i++)
{
    var funcPlot = plot.Add.Function(funcList[i]);
    funcPlot.MinX = xStart[i];
    funcPlot.MaxX = xStart[i + 1];
}

//plot.SavePng("CamProfile.png", 800, 600).LaunchFile();
plot.SaveWebp("CamProfile.webp", 800, 600, 85).LaunchInBrowser();