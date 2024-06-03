using Avalonia.Controls;

namespace MotionProfile_DemoGUI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        ScottPlot.Avalonia.AvaPlot? plot = this.Find<ScottPlot.Avalonia.AvaPlot>("Plot1");
        double[] x = [1, 2, 3, 4, 5];
        double[] y = [2, 4, 6, 8, 10];
        plot?.Plot.Add.Scatter(x, y);
        plot?.Refresh();
    }
}