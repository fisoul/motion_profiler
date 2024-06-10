using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MotionProfile_DemoGUI.ViewModels;

namespace MotionProfile_DemoGUI.Views;

public partial class MainWindow : Window
{
    private readonly MainWindowViewModel viewModel;
    
    public MainWindow()
    {
        InitializeComponent();

        PlotPosition.Plot.Title("Position", 16);   
        PlotVelocity.Plot.Title("Velocity", 16);   
        PlotAcceleration.Plot.Title("Acceleration", 16);   
        PlotJerk.Plot.Title("Jerk", 16);

        viewModel = new MainWindowViewModel();
        DataContext = viewModel;
        viewModel.ProfileChanged += ViewModel_ProfileChanged;
    }

    private void ViewModel_ProfileChanged(object? sender, EventArgs e)
    {
        PlotPosition.Plot.Clear();
    }
}