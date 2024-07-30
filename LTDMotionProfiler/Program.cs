// See https://aka.ms/new-console-template for more information
using MotionProfiler;
using MotionProfiler.BRAutomat;
using ScottPlot;
using ScottPlot.Plottables;
using SkiaSharp;

// Console.WriteLine("LTD of Motion Profiler");
// CamFixedPoint p1 = new(0, 0, 0, 0);
// CamFixedPoint p2 = new(0.1, 177000, 100000, 0);
// var profile = ProfileGen.CalcCamPolynomial(p1, p2, 5);
// Console.WriteLine(profile);

const int n = 5;
const double ra = 0.2;
const double vin = 0;
const double vout = 1;
const double sout = (vin + vout) / 2;

Console.ForegroundColor = ConsoleColor.Red;
Console.WriteLine($"v_in = {vin:N2}, v_out = {vout:N2}, s_out = {sout:N2}");
Console.WriteLine();

var (p1, p2) = ProfileGen.CalcSymmetricShift(ra, vin, vout, 3);
Console.ForegroundColor = ConsoleColor.Magenta;
Console.WriteLine("3阶对称加速");
Console.ResetColor();
Console.WriteLine(p1.GetString(n));
Console.WriteLine(p2.GetString(n));

var (p3, p4) = ProfileGen.CalcSymmetricShift(ra, vout, vin, 3);
Console.ForegroundColor = ConsoleColor.Magenta;
Console.WriteLine("3阶对称减速");
Console.ResetColor();
Console.WriteLine(p3.GetString(n));
Console.WriteLine(p4.GetString(n));

var (p5, p6) = ProfileGen.CalcSymmetricShift(ra, vin, vout, 4);
Console.ForegroundColor = ConsoleColor.DarkBlue;
Console.WriteLine("4阶对称加速");
Console.ResetColor();
Console.WriteLine(p5.GetString(n));
Console.WriteLine(p6.GetString(n));

var (p7, p8) = ProfileGen.CalcSymmetricShift(ra, vout, vin, 4);
Console.ForegroundColor = ConsoleColor.DarkBlue;
Console.WriteLine("4阶对称减速");
Console.ResetColor();
Console.WriteLine(p7.GetString(n));
Console.WriteLine(p8.GetString(n));


// var p0 = new CamFixedPoint(0, 0, 0, 0);
// var poly0 = ProfileGen.CalcCamPolynomial(p0, p1, 5);
// Console.WriteLine(poly0);
//
// var poly1 = ProfileGen.CalcCamPolynomial(p1, p2, 5);
// Console.WriteLine(poly1);
//
// var p3 = new CamFixedPoint(1, 2, 3, 0);
// var poly2 = ProfileGen.CalcCamPolynomial(p2, p3, 5);
// Console.WriteLine(poly2);

// var poly1 = ProfileGen.CalcCamPolynomial(new CamFixedPoint(0, 0, 0, 0), p1);
// var poly2 = ProfileGen.CalcCamPolynomial(p1, p2);
// var poly3 = ProfileGen.CalcCamPolynomial(p2, new CamFixedPoint(1, 1, 2, 0));
//
// Console.WriteLine(poly1.Translate(0, 0));
// Console.WriteLine(poly2.Translate(-0.2, 0));
// Console.WriteLine(poly3.Translate(-0.8, 0));

// Console.WriteLine();
// Console.WriteLine("主轴加速计算");
// var actSpeed = 60;
// var targetSpeed = 180;
// var angleLength = 18000.0f;
// var t = angleLength * 2 / ((targetSpeed + actSpeed) * 600);
// var acc = (targetSpeed - actSpeed) * 600 / t;
// Console.WriteLine($"");
// Console.WriteLine(acc);