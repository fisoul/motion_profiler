using System;
using System.Diagnostics;
using Avalonia.Controls;
using MotionProfiler;
using MotionProfile_DemoGUI.ViewModels;
using ScottPlot;
using ScottPlot.TickGenerators;

namespace MotionProfile_DemoGUI.Views;

public partial class MainWindow : Window
{
    private MainWindowViewModel? viewModel;
    private const double YRange = 0.15;
    
    public MainWindow()
    {
        InitializeComponent();

        PlotPosition.Plot.Title("Position", 16);   
        PlotVelocity.Plot.Title("Velocity", 16);   
        PlotAcceleration.Plot.Title("Acceleration", 16);
        PlotJerk.Plot.Title("Jerk", 16);
        Plot[] plots = [PlotPosition.Plot, PlotVelocity.Plot, PlotAcceleration.Plot, PlotJerk.Plot];
        foreach (var plot in plots)
        {
            plot.RenderManager.AxisLimitsChanged += AxisLimitsChanged;
        }
    }

    private void AxisLimitsChanged(object? sender, RenderDetails e)
    {
        
    }

    public void ViewModel_ProfileChanged(object? sender, EventArgs e)
    {
        // Get ViewModel
        viewModel = sender as MainWindowViewModel;
        if (viewModel == null) return;
        
        // Get Profile
        var accProfile = new StaticProfile(viewModel.Profile[0], viewModel.MasterAcc, viewModel.SlaveAcc, viewModel.MasterSpeed);
        var uniProfile = new StaticProfile(viewModel.Profile[1], viewModel.MasterUni, viewModel.SlaveUni, viewModel.MasterSpeed);
        var decProfile = new StaticProfile(viewModel.Profile[2], viewModel.MasterDec, viewModel.SlaveDec, viewModel.MasterSpeed);
        var interval = new double[4];
        interval[0] = 0;
        interval[1] = viewModel.MasterAcc / viewModel.MasterSpeed;
        interval[2] = (viewModel.MasterAcc + viewModel.MasterUni) / viewModel.MasterSpeed;
        interval[3] = viewModel.MasterTotal / viewModel.MasterSpeed;
        // Position
        var plot = PlotPosition.Plot;
        plot.PlottableList.Clear();
        // add func plot
        var funcPlot1 = plot.Add.Function(t => accProfile.EvaluatePosition(t));
        var offset1 = accProfile.EvaluatePosition(viewModel.MasterAcc / viewModel.MasterSpeed);
        var funcPlot2 = plot.Add.Function(t => uniProfile.EvaluatePosition(t - viewModel.MasterAcc / viewModel.MasterSpeed) + offset1);
        var offset2 = uniProfile.EvaluatePosition(viewModel.MasterUni / viewModel.MasterSpeed);
        var funcPlot3 = plot.Add.Function(t => decProfile.EvaluatePosition(t - (viewModel.MasterAcc + viewModel.MasterUni) / viewModel.MasterSpeed) + offset1 + offset2);
        // set func plot
        funcPlot1.MinX = 0;
        funcPlot1.MaxX = funcPlot2.MinX = interval[1];
        funcPlot2.MaxX = funcPlot3.MinX = interval[2];
        funcPlot3.MaxX = interval[3];
        funcPlot1.LineWidth = 2;
        funcPlot2.LineWidth = 2;
        funcPlot3.LineWidth = 2;
        plot.Axes.SetLimits(0, interval[3], viewModel.SlaveTotal * -YRange, viewModel.SlaveTotal * (1+YRange));
        PlotPosition.Refresh();

        // Velocity
        plot = PlotVelocity.Plot;
        plot.PlottableList.Clear();
        // add func plot
        funcPlot1 = plot.Add.Function(t => accProfile.EvaluateVelocity(t));
        funcPlot2 = plot.Add.Function(t => uniProfile.EvaluateVelocity(t - viewModel.MasterAcc / viewModel.MasterSpeed));
        funcPlot3 = plot.Add.Function(t => decProfile.EvaluateVelocity(t - (viewModel.MasterAcc + viewModel.MasterUni) / viewModel.MasterSpeed));
        // set func plot
        funcPlot1.MinX = 0;
        funcPlot1.MaxX = funcPlot2.MinX = interval[1];
        funcPlot2.MaxX = funcPlot3.MinX = interval[2];
        funcPlot3.MaxX = interval[3];
        funcPlot1.LineWidth = 2;
        funcPlot2.LineWidth = 2;
        funcPlot3.LineWidth = 2;
        plot.Axes.SetLimits(0, interval[3], uniProfile.EvaluateVelocity(0) * -YRange, uniProfile.EvaluateVelocity(0) * (1+YRange));
        PlotVelocity.Refresh();
        
        // Acceleration
        plot = PlotAcceleration.Plot;
        plot.PlottableList.Clear();
        // add func plot
        funcPlot1 = plot.Add.Function(t => accProfile.EvaluateAcceleration(t));
        funcPlot2 = plot.Add.Function(t => uniProfile.EvaluateAcceleration(t - viewModel.MasterAcc / viewModel.MasterSpeed));
        funcPlot3 = plot.Add.Function(t => decProfile.EvaluateAcceleration(t - (viewModel.MasterAcc + viewModel.MasterUni) / viewModel.MasterSpeed));
        // set func plot
        funcPlot1.MinX = 0;
        funcPlot1.MaxX = funcPlot2.MinX = interval[1];
        funcPlot2.MaxX = funcPlot3.MinX = interval[2];
        funcPlot3.MaxX = interval[3];
        funcPlot1.LineWidth = 2;
        funcPlot2.LineWidth = 2;
        funcPlot3.LineWidth = 2;
        plot.Axes.SetLimits(0,
            interval[3],
            decProfile.EvaluateAcceleration((double)viewModel.MasterDec / 2 / viewModel.MasterSpeed) * (1+YRange),
            accProfile.EvaluateAcceleration((double)viewModel.MasterAcc / 2 / viewModel.MasterSpeed) * (1+YRange));
        PlotAcceleration.Refresh();
        
        // Jerk
        plot = PlotJerk.Plot;
        plot.PlottableList.Clear();
        // add func plot
        funcPlot1 = plot.Add.Function(t => accProfile.EvaluateJerk(t));
        funcPlot2 = plot.Add.Function(t => uniProfile.EvaluateJerk(t - viewModel.MasterAcc / viewModel.MasterSpeed));
        funcPlot3 = plot.Add.Function(t =>
            decProfile.EvaluateJerk(t - (viewModel.MasterAcc + viewModel.MasterUni) / viewModel.MasterSpeed));
        // set func plot
        funcPlot1.MinX = 0;
        funcPlot1.MaxX = funcPlot2.MinX = interval[1];
        funcPlot2.MaxX = funcPlot3.MinX = interval[2];
        funcPlot3.MaxX = interval[3];
        funcPlot1.LineWidth = 2;
        funcPlot2.LineWidth = 2;
        funcPlot3.LineWidth = 2;

        var accJerkMaxAbs = viewModel.OrderAcc == 3
            ? accProfile.EvaluateJerk(viewModel.MasterAcc * viewModel.RaAcc / 2 / viewModel.MasterSpeed)
            : accProfile.EvaluateJerk(0);
        var decJerkMaxAbs = viewModel.OrderDec == 3
            ? decProfile.EvaluateJerk(viewModel.MasterDec * viewModel.RaDec / 2 / viewModel.MasterSpeed)
            : decProfile.EvaluateJerk(0);

        accJerkMaxAbs = Math.Abs(accJerkMaxAbs);
        decJerkMaxAbs = Math.Abs(decJerkMaxAbs);
        var jerkAbsMax = Math.Max(accJerkMaxAbs, decJerkMaxAbs);

        plot.Axes.SetLimits(0,
            interval[3],
            -jerkAbsMax * (1 + YRange),
            jerkAbsMax * (1 + YRange));
        
        PlotJerk.Refresh();
    }
}