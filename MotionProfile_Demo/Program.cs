// See https://aka.ms/new-console-template for more information

using MathNet.Numerics;
using MotionProfiler;
using MotionProfiler.BRAutomat;
using ScottPlot;

Test2();

static void Test1()
{
    
}

static void Test2()
{
    Console.WriteLine("B&R CamAutomat Profiler");

// var poly = new CamPolynomial(new double[] { 0, 2, 2 });
// Console.WriteLine(poly.Evaluate(-1));
// Console.WriteLine(poly.Evaluate(1,1,1,2,0));


//Demo Application Parameters, Acc Sync Dec Portion Calc
    var masterTotal = 30;
    var slaveTotal = 30;
    var slaveAcc = 10;
    var slaveDec = 10;
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
    var accFuncPlot = plot.Add.Function(accProfile.GetFunction(masterAcc, slaveAcc, 0, 0));
    accFuncPlot.MinX = 0;
    accFuncPlot.MaxX = masterAcc;

    var syncFuncPlot = plot.Add.Function(syncProfile.GetFunction(masterUniform, slaveUniform, 0, masterAcc));
    syncFuncPlot.MinX = masterAcc;
    syncFuncPlot.MaxX = masterAcc + masterUniform;

    var decFuncPlot = plot.Add.Function(decProfile.GetFunction(masterDec, slaveDec, 0, masterAcc + masterUniform));
    decFuncPlot.MinX = masterAcc + masterUniform;
    decFuncPlot.MaxX = masterTotal;

    var testFunc = syncProfile.GetFunction(1, 1, 0);
    var testFuncStretched = syncProfile.GetFunction(2, 1, 0);
    for (var i = 0; i < 10; i++)
    {
        Console.WriteLine($"f({i/10.0:N1})=" + testFunc(i / 10.0));
        Console.WriteLine($"g({i:N1})=" + testFuncStretched(i));
    }
    
    plot.Axes.SetLimits(0, masterTotal, 0, slaveTotal);
    //plot.SaveWebp("CamProfile.webp", 800, 600, 100).LaunchInBrowser();
    // plot.SavePng("CamProfile.png", 800, 600).LaunchFile();
}

