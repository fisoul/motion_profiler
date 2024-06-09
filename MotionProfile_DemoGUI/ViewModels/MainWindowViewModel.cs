using System;
using System.Diagnostics;
using System.Reactive;
using MotionProfiler;
using ReactiveUI;
using ScottPlot.Avalonia;

namespace MotionProfile_DemoGUI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public static string Greeting => "Welcome to Avalonia!";

    public ReactiveCommand<Unit, Unit> ExampleCommand { get; }
    
    public MainWindowViewModel()
    {
        ExampleCommand = ReactiveCommand.Create(PerformAction);
    }
    private void PerformAction()
    {
        Plot.Add.Scatter(new double[] { 1, 2, 3, 4 }, new double[] {1, 2, 3, 4});
        Plot.Axes.AutoScale();
    }
    
    public ScottPlot.Plot Plot { get; set; } = new();
}