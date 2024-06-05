// See https://aka.ms/new-console-template for more information

using System.Text.RegularExpressions;
using MotionProfile;
using ScottPlot;
using ScottPlot.DataSources;
using ScottPlot.Plottables;

Console.WriteLine("B&R CamAutomat Profiler");

int bagLength = 300000;
int accLength = 100000;
int decLength = 100000;
int syncLength = bagLength - accLength - decLength;

var (pAcc1, pAcc2) = CamProfiler.CalcSymmetricShift(0.2, 3, 0);
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



