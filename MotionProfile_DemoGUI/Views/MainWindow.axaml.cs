using System;
using System.ComponentModel;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MotionProfile_DemoGUI.ViewModels;

namespace MotionProfile_DemoGUI.Views;

public partial class MainWindow : Window
{
    private MainWindowViewModel viewModel;
    
    public MainWindow()
    {
        InitializeComponent();

        PlotPosition.Plot.Title("Position", 16);   
        PlotVelocity.Plot.Title("Velocity", 16);   
        PlotAcceleration.Plot.Title("Acceleration", 16);
        PlotJerk.Plot.Title("Jerk", 16);
    }

    private void Property_Changed(object? sender, PropertyChangedEventArgs e)
    {
        Debug.WriteLine("PropertyChanged");
    }

    private void ViewModel_ProfileChanged(object? sender, EventArgs e)
    {
        var plot = PlotPosition.Plot;
        plot.PlottableList.Clear();
        var funcPlot1 = plot.Add.Function(x => viewModel.ProfileScaled[0].Evaluate(x * viewModel.MasterSpeed));
        var funcPlot2 = plot.Add.Function(x => viewModel.ProfileScaled[1].Evaluate(x * viewModel.MasterSpeed - viewModel.MasterAcc) + viewModel.SlaveAcc);
        var funcPlot3 = plot.Add.Function(x => viewModel.ProfileScaled[2].Evaluate(x * viewModel.MasterSpeed - viewModel.MasterAcc - viewModel.MasterUni) + viewModel.SlaveAcc + viewModel.SlaveUni);
        funcPlot1.MinX = 0;
        funcPlot1.MaxX = viewModel.MasterAcc / viewModel.MasterSpeed;
        funcPlot2.MinX = viewModel.MasterAcc / viewModel.MasterSpeed;
        funcPlot2.MaxX = (viewModel.MasterAcc + viewModel.MasterUni) / viewModel.MasterSpeed;;
        funcPlot3.MinX = (viewModel.MasterAcc + viewModel.MasterUni) / viewModel.MasterSpeed;;
        funcPlot3.MaxX = viewModel.MasterTotal / viewModel.MasterSpeed;;
        plot.Axes.SetLimits(0, viewModel.MasterTotal / viewModel.MasterSpeed, 0, viewModel.SlaveTotal);
        PlotPosition.Refresh();

        plot = PlotVelocity.Plot;
        plot.PlottableList.Clear();
        funcPlot1 = plot.Add.Function(x => viewModel.ProfileScaled[0].Differentiate(1).Evaluate(x * viewModel.MasterSpeed));
        funcPlot2 = plot.Add.Function(x => viewModel.ProfileScaled[1].Differentiate().Evaluate(x * viewModel.MasterSpeed - viewModel.MasterAcc) + viewModel.SlaveAcc);
        funcPlot3 = plot.Add.Function(x => viewModel.ProfileScaled[2].Differentiate().Evaluate(x * viewModel.MasterSpeed - viewModel.MasterAcc - viewModel.MasterUni) + viewModel.SlaveAcc + viewModel.SlaveUni);
        funcPlot1.MinX = 0;
        funcPlot1.MaxX = viewModel.MasterAcc / viewModel.MasterSpeed;
        funcPlot2.MinX = viewModel.MasterAcc / viewModel.MasterSpeed;
        funcPlot2.MaxX = (viewModel.MasterAcc + viewModel.MasterUni) / viewModel.MasterSpeed;;
        funcPlot3.MinX = (viewModel.MasterAcc + viewModel.MasterUni) / viewModel.MasterSpeed;;
        funcPlot3.MaxX = viewModel.MasterTotal / viewModel.MasterSpeed;;
        plot.Axes.SetLimits(0, viewModel.MasterTotal / viewModel.MasterSpeed, 0, viewModel.SlaveTotal);
        PlotVelocity.Refresh();
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        viewModel = DataContext as MainWindowViewModel;
        viewModel.ProfileChanged += ViewModel_ProfileChanged;
    }
}