// See https://aka.ms/new-console-template for more information

using MathNet.Numerics;
using MotionProfiler;
using MotionProfiler.BRAutomat;
using ScottPlot;

Console.WriteLine("B&R CamAutomat Profiler");
//Demo Application Parameters, Acc Sync Dec Portion Calc
var masterTotal = 18000;
var slaveTotal = 300000;
var slaveAcc = 80000;
var slaveDec = 80000;
var slaveUniform = slaveTotal - slaveAcc - slaveDec;


var masterAcc = (int)((double)slaveAcc * 2 / (slaveTotal + slaveAcc + slaveDec) * masterTotal);
var masterDec = (int)((double)slaveDec * 2 / (slaveTotal + slaveAcc + slaveDec) * masterTotal);
var masterUniform = masterTotal - masterAcc - masterDec;

Console.WriteLine($"主轴 加速段:{masterAcc:d}, 匀速段:{masterUniform:d},  减速段:{masterDec:d}");
Console.WriteLine($"从轴 加速段:{slaveAcc:d}, 匀速段:{slaveUniform:d},  减速段:{slaveDec:d}");


// Acc / Dec Profile
var (pAcc1, pAcc2) = ProfileGen.CalcSymmetricShift(0.2);
Console.WriteLine("Acceleration Profile:");
Console.WriteLine(pAcc1);
Console.WriteLine(pAcc2);
Console.WriteLine();
var curveAcc1 = ProfileGen.CalcCamPolynomial(new CamFixedPoint(0, 0, 0, 0), pAcc1);
var curveAcc2 = ProfileGen.CalcCamPolynomial(pAcc1, pAcc2);
var curveAcc3 = ProfileGen.CalcCamPolynomial(pAcc2, new CamFixedPoint(1, 1, 2, 0));
CamProfile accProfile = new(1, 1, [curveAcc1, curveAcc2, curveAcc3]);

var (pDec1, pDec2) = ProfileGen.CalcSymmetricShift(0.2, 3, 1);
Console.WriteLine("Deceleration Profile:");
Console.WriteLine(pDec1);
Console.WriteLine(pDec2);
Console.WriteLine();
var curveDec1 = ProfileGen.CalcCamPolynomial(new CamFixedPoint(0, 0, 2, 0), pDec1);
var curveDec2 = ProfileGen.CalcCamPolynomial(pDec1, pDec2);
var curveDec3 = ProfileGen.CalcCamPolynomial(pDec2, new CamFixedPoint(1, 1, 0, 0));
CamProfile decProfile = new(1, 1, [curveDec1, curveDec2, curveDec3]);

var syncProfile = CamProfile.StraightLine();

// Apply B&R Automat
AutData autData = new();
autData.State[2].MasterFactor = masterAcc;
autData.State[2].SlaveFactor = slaveAcc;
autData.State[2].CompMode = AutCompMode.ncOFF;
autData.State[2].Event[0].Type = AutEventTyp.ncST_END;
autData.State[2].Event[0].Attribute = AutEventAttr.ncST_END;
autData.State[2].Event[0].NextState = 3;

autData.State[3].MasterFactor = masterUniform;
autData.State[3].SlaveFactor = slaveUniform;
autData.State[3].CompMode = AutCompMode.ncOFF;
autData.State[3].Event[0].Type = AutEventTyp.ncST_END;
autData.State[3].Event[0].Attribute = AutEventAttr.ncST_END;
autData.State[3].Event[0].NextState = 4;

autData.State[4].MasterFactor = masterDec;
autData.State[4].SlaveFactor = slaveDec;
autData.State[4].CompMode = AutCompMode.ncOFF;
autData.State[4].Event[0].Type = AutEventTyp.ncST_END;
autData.State[4].Event[0].Attribute = AutEventAttr.ncST_END;
autData.State[4].Event[0].NextState = 5;

// visualization using ScottPlot
Plot plot = new();
var accFuncPlot = plot.Add.Function(accProfile.GetFunction(masterAcc, slaveAcc, 1, 0, 0));
accFuncPlot.MinX = 0;
accFuncPlot.MaxX = masterAcc;

var syncFuncPlot = plot.Add.Function(syncProfile.GetFunction(masterUniform, slaveUniform, 1, masterAcc, slaveAcc));
syncFuncPlot.MinX = masterAcc;
syncFuncPlot.MaxX = masterAcc + masterUniform;

var decFuncPlot = plot.Add.Function(decProfile.GetFunction(masterDec, slaveDec, 1, masterAcc + masterUniform, slaveAcc + slaveUniform));
decFuncPlot.MinX = masterAcc + masterUniform;
decFuncPlot.MaxX = masterTotal;

var poly = syncProfile.PolynomialData[0];
var polyS = poly.StretchThenTranslate(masterAcc, slaveAcc, masterAcc, 0);
var polyV = polyS.Differentiate(1);
Console.WriteLine(polyS);
Console.WriteLine(polyV);
Console.WriteLine(polyS.Evaluate(10000));
Console.WriteLine(polyV.Evaluate(10000));

plot.Axes.SetLimits(0, masterTotal, 0, 26);
plot.SaveWebp("CamProfile.webp", 800, 600, 100).LaunchInBrowser();
// plot.SavePng("CamProfile.png", 800, 600).LaunchFile();


static void PolyAffineTest(CamPolynomial poly)
{
    Console.WriteLine("Original Polynomial: " + poly);
    Affine2D affine = new();
    poly.Translate(1, 1);
}