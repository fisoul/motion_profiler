using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive;
using System.Runtime.CompilerServices;
using MotionProfiler;
using ReactiveUI;

namespace MotionProfile_DemoGUI.ViewModels;

public sealed class MainWindowViewModel : INotifyPropertyChanged
{
    public static string Greeting => "Cam Profile";
    
    private int masterTotal;
    public int MasterTotal 
    { 
        get => masterTotal;
        set { if (SetField(ref masterTotal, value)) UpdateAutomat(); }
    }

    private double masterSpeed;

    public double MasterSpeed
    {
        get => masterSpeed;
        set
        {
            if (SetField(ref masterSpeed, value)) UpdateAutomat();
        }
    }

    private int slaveTotal;
    public int SlaveTotal
    {
        get => slaveTotal;
        set { if (SetField(ref slaveTotal, value)) UpdateAutomat(); }
    }

    private readonly int[] slaveFactor = new int[3];
    private readonly int[] masterFactor = new int[3];
    
    public CamProfile[] Profile { get; } = new CamProfile[3];
    public CamProfile[] ProfileScaled { get; } = new CamProfile[3];


    private double _slaveAccPercent = 0.3;
    public double SlaveAccPercent
    {
        get => _slaveAccPercent;
        set
        {
            _slaveAccPercent = value;
            SlaveAcc = (int)(SlaveTotal * _slaveAccPercent);
        }
    }
    public int SlaveAcc
    {
        get => slaveFactor[0];
        set { if (SetField(ref slaveFactor[0], value)) UpdateAutomat(); }
    }

    public int SlaveUni
    {
        get => slaveFactor[1];
        set { if (SetField(ref slaveFactor[1], value)) UpdateAutomat(); }
    }
    
    private double _slaveDecPercent = 0.3;
    public double SlaveDecPercent
    {
        get => _slaveDecPercent;
        set
        {
            _slaveDecPercent = value;
            SlaveDec = (int)(SlaveTotal * _slaveDecPercent);
        }
    }
    public int SlaveDec
    {
        get => slaveFactor[2];
        set { if (SetField(ref slaveFactor[2], value)) UpdateAutomat(); }
    }

    public int MasterAcc
    {
        get => masterFactor[0];
        set { if (SetField(ref masterFactor[0], value)) UpdateAutomat(); }
    }

    public int MasterUni
    {
        get => masterFactor[1];
        set { if (SetField(ref masterFactor[1], value)) UpdateAutomat(); }
    }
    
    public int MasterDec
    {
        get => masterFactor[2];
        set { if (SetField(ref masterFactor[2], value)) UpdateAutomat(); }
    }

    private double raAcc;
    public double RaAcc
    {
        get => raAcc;
        set
        {
            if (SetField(ref raAcc, value))
            {
                Profile[0] = CamProfile.SymmetricSpeedShift(raAcc, OrderAcc, 0);
                UpdateAutomat();
            }
        }
    }
    private int orderAcc;
    public int OrderAcc
    {
        get => orderAcc;
        set
        {
            if (SetField(ref orderAcc, value))
            {
                Profile[0] = CamProfile.SymmetricSpeedShift(raAcc, OrderAcc, 0);
                UpdateAutomat();
            }
        }
    }

    private double raDec;
    public double RaDec
    {
        get => raDec;
        set
        {
            if (SetField(ref raDec, value))
            {
                Profile[2] = CamProfile.SymmetricSpeedShift(raDec, OrderDec, 1);
                UpdateAutomat();
            }
        }
    }

    private int orderDec;
    public int OrderDec
    {
        get => orderDec;
        set
        {
            if (SetField(ref orderDec, value))
            {
                Profile[2] = CamProfile.SymmetricSpeedShift(raDec, OrderDec, 1);
                UpdateAutomat();
            }
        }
    }
    public bool CalcValid { get; set; }

    public void UpdateAutomat(bool force = false)
    {
        SlaveAcc = (int)(slaveTotal * _slaveAccPercent);
        SlaveDec = (int)(slaveTotal * _slaveDecPercent);
        if (SlaveAcc + SlaveDec > SlaveTotal)
        {
            CalcValid = false;
            return;
        }
        SlaveUni = SlaveTotal - SlaveDec - SlaveAcc;
        MasterAcc = (int)((double)SlaveAcc * 2 / (SlaveTotal + SlaveAcc + SlaveDec) * MasterTotal);
        MasterDec = (int)((double)SlaveDec * 2 / (SlaveTotal + SlaveAcc + SlaveDec) * MasterTotal);
        MasterUni = MasterTotal - MasterAcc - MasterDec;
        CalcValid = true;
        for (var i = 0; i < Profile.Length; i++)
        {
            ProfileScaled[i] = Profile[i].Stretch(masterFactor[i], slaveFactor[i]);
        }
        ProfileChanged?.Invoke(this, EventArgs.Empty);
    }
    
    public MainWindowViewModel()
    {
        ExampleCommand = ReactiveCommand.Create(PerformAction);

        masterTotal = 18000;
        masterSpeed = masterTotal;
        slaveTotal = 300000;
        slaveFactor[0] = 100000;
        slaveFactor[1] = 100000;
        slaveFactor[2] = 100000;
        masterFactor[0] = 7200;
        masterFactor[1] = 3600;
        masterFactor[2] = 7200;
        
        raAcc = raDec = 0.2;
        orderAcc = orderDec = 3;
        Profile[0] = CamProfile.SymmetricSpeedShift(RaAcc, OrderAcc, 0);
        Profile[1] = CamProfile.StraightLine();
        Profile[2] = CamProfile.SymmetricSpeedShift(RaDec, OrderDec, 1);

        ProfileScaled[0] = Profile[0]; // ref
        ProfileScaled[1] = Profile[1];
        ProfileScaled[2] = Profile[2];
        UpdateAutomat(true);
    }

    public event EventHandler? ProfileChanged;
    
    // ICommand
    public ReactiveCommand<Unit, Unit> ExampleCommand { get; }
    private void PerformAction()
    {

    }
    
    // INotifyPropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}