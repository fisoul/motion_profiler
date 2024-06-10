using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive;
using System.Runtime.CompilerServices;
using MotionProfiler;
using ReactiveUI;
using ScottPlot.Avalonia;

namespace MotionProfile_DemoGUI.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    public static string Greeting => "Cam Profile";

    private int masterTotal;
    public int MasterTotal 
    { 
        get => masterTotal;
        set { if (SetField(ref masterTotal, value)) UpdateAutomat(); }
    }

    private int slaveTotal;
    public int SlaveTotal
    {
        get => slaveTotal;
        set { if (SetField(ref slaveTotal, value)) UpdateAutomat(); }
    }
    
    private int slaveAcc;
    public int SlaveAcc
    {
        get => slaveAcc;
        set { if (SetField(ref slaveAcc, value)) UpdateAutomat(); }
    }

    private int slaveDec;
    public int SlaveDec
    {
        get => slaveDec;
        set { if (SetField(ref slaveDec, value)) UpdateAutomat(); }
    }

    private int slaveUni;
    public int SlaveUni
    {
        get => slaveUni;
        set { if (SetField(ref slaveUni, value)) UpdateAutomat(); }
    }

    private int masterAcc;
    public int MasterAcc
    {
        get => masterAcc;
        set { if (SetField(ref masterAcc, value)) UpdateAutomat(); }
    }

    private int masterDec;
    public int MasterDec
    {
        get => masterDec;
        set { if (SetField(ref masterDec, value)) UpdateAutomat(); }
    }

    private int masterUni;
    public int MasterUni
    {
        get => masterUni;
        set { if (SetField(ref masterUni, value)) UpdateAutomat(); }
    }

    private CamPolynomial[] PolynomialsAcc = [];
    public CamProfile ProfileAcc { get; private set; }
    public CamProfile ProfileUni { get; private set; } = CamProfile.StraightLine();
    public CamProfile ProfileDec { get; private set; }

    private double raAcc;
    public double RaAcc
    {
        get => raAcc;
        set
        {
            if (SetField(ref raAcc, value)) ProfileAcc = CamProfile.SymmetricSpeedShift(RaAcc, OrderAcc, 0);
        }
    }
    public int OrderAcc;

    private double raDec;
    public double RaDec
    {
        get => raDec;
        set
        {
            if (SetField(ref raDec, value)) ProfileDec = CamProfile.SymmetricSpeedShift(RaDec, OrderDec, 1);
        }
    }
    public int OrderDec;
    public bool CalcValid { get; set; }

    
    private void UpdateAutomat()
    {
        if (SlaveAcc + SlaveDec >= SlaveTotal)
        {
            CalcValid = false;
            return;
        }

        SlaveUni = SlaveTotal - SlaveDec;
        MasterAcc = (int)((double)SlaveAcc * 2 / (SlaveTotal + SlaveAcc + SlaveDec) * MasterTotal);
        MasterDec = (int)((double)SlaveDec * 2 / (SlaveTotal + SlaveAcc + SlaveDec) * MasterTotal);
        MasterUni = MasterTotal - MasterAcc - MasterDec;
        CalcValid = true;

        ProfileChanged?.Invoke(this, EventArgs.Empty);
    }
    
    public MainWindowViewModel()
    {
        ExampleCommand = ReactiveCommand.Create(PerformAction);

        RaAcc = RaDec = 0.2;
        OrderAcc = OrderDec = 3;
        ProfileAcc = CamProfile.SymmetricSpeedShift(RaAcc, OrderAcc, 0);
        ProfileDec = CamProfile.SymmetricSpeedShift(RaDec, OrderDec, 1);
    }

    public event EventHandler? ProfileChanged;
    
    // ICommand
    public ReactiveCommand<Unit, Unit> ExampleCommand { get; }
    private void PerformAction()
    {

    }
    
    // INotifyPropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}