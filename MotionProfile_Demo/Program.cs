// See https://aka.ms/new-console-template for more information
using MotionProfiler;
using MotionProfiler.BRAutomat;
using ScottPlot;

Console.WriteLine("B&R CamAutomat Profiler");
//Demo Application Parameters, Acc - Uniform - Dec Portion Calc
const int masterTotal = 18000;
const double masterSpeed = 18000;
const int slaveTotal = 300000;
const int slaveAcc = 100000;
const int slaveDec = 100000;
const int slaveUniform = slaveTotal - slaveAcc - slaveDec;
const int masterAcc = (int)((double)slaveAcc * 2 / (slaveTotal + slaveAcc + slaveDec) * masterTotal);
const int masterDec = (int)((double)slaveDec * 2 / (slaveTotal + slaveAcc + slaveDec) * masterTotal);
const int masterUniform = masterTotal - masterAcc - masterDec;
Console.WriteLine($"主轴 加速段:{masterAcc:d}, 匀速段:{masterUniform:d},  减速段:{masterDec:d}");
Console.WriteLine($"从轴 加速段:{slaveAcc:d}, 匀速段:{slaveUniform:d},  减速段:{slaveDec:d}");

// [Acc - Uni - Dec] Profile Generation
// Acc
var (pAcc1, pAcc2) = ProfileGen.CalcSymmetricShift(0.2);
Console.WriteLine("Acceleration Profile:");
Console.WriteLine(pAcc1);
Console.WriteLine(pAcc2);
Console.WriteLine();
var curveAcc1 = ProfileGen.CalcCamPolynomial(new CamFixedPoint(0, 0, 0, 0), pAcc1);
var curveAcc2 = ProfileGen.CalcCamPolynomial(pAcc1, pAcc2);
var curveAcc3 = ProfileGen.CalcCamPolynomial(pAcc2, new CamFixedPoint(1, 1, 2, 0));
CamProfile accProfile = new(1, 1, [curveAcc1, curveAcc2, curveAcc3]);
// Uniform
var syncProfile = CamProfile.StraightLine();
// Dec
var (pDec1, pDec2) = ProfileGen.CalcSymmetricShift(0.2, 3, 1);
Console.WriteLine("Deceleration Profile:");
Console.WriteLine(pDec1);
Console.WriteLine(pDec2);
Console.WriteLine();
var curveDec1 = ProfileGen.CalcCamPolynomial(new CamFixedPoint(0, 0, 2, 0), pDec1);
var curveDec2 = ProfileGen.CalcCamPolynomial(pDec1, pDec2);
var curveDec3 = ProfileGen.CalcCamPolynomial(pDec2, new CamFixedPoint(1, 1, 0, 0));
CamProfile decProfile = new(1, 1, [curveDec1, curveDec2, curveDec3]);

// Apply B&R Automat - not implemented
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
accProfile = accProfile.Stretch(masterAcc, slaveAcc);
var accFuncPlot = plot.Add.Function(x => accProfile.Evaluate(x * masterSpeed));
accFuncPlot.MinX = 0;
accFuncPlot.MaxX = accProfile.MasterPeriod / masterSpeed;

syncProfile = syncProfile.Stretch(masterUniform, slaveUniform);
var syncFuncPlot = plot.Add.Function(x => syncProfile.Evaluate(x * masterSpeed - masterAcc) + slaveAcc);
syncFuncPlot.MinX = accProfile.MasterPeriod / masterSpeed;
syncFuncPlot.MaxX = (accProfile.MasterPeriod + syncProfile.MasterPeriod) / masterSpeed;

decProfile = decProfile.Stretch(masterDec, slaveDec);
var decFuncPlot = plot.Add.Function(x => decProfile.Evaluate(x * masterSpeed - masterAcc - masterUniform) + slaveAcc + slaveUniform);
decFuncPlot.MinX = (masterAcc + masterUniform) / masterSpeed;
decFuncPlot.MaxX = masterTotal / masterSpeed;


plot.Axes.SetLimits(0, masterTotal / masterSpeed, 0, slaveTotal);
plot.SaveWebp("CamProfile.webp", 800, 600, 100).LaunchInBrowser();
// plot.SavePng("CamProfile.png", 800, 600).LaunchFile();
