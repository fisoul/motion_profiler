using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MotionProfile_DemoGUI.ViewModels;
using MotionProfile_DemoGUI.Views;

namespace MotionProfile_DemoGUI;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = new MainWindow();
            var viewModel = new MainWindowViewModel();
            viewModel.ProfileChanged += mainWindow.ViewModel_ProfileChanged;
            desktop.MainWindow = mainWindow;
            mainWindow.DataContext = viewModel;
        }
        base.OnFrameworkInitializationCompleted();
    }
}