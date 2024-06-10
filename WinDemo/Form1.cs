using MotionProfiler;

namespace WinDemo
{
    public partial class Form1 : Form
    {
        private CamProfile accProfile;
        private CamProfile uniProfile;
        private CamProfile decProfile;

        private int masterTotal, masterAcc, masterDec, masterUni;
        private int slaveTotal, slaveAcc, slaveDec, slaveUni;
        private double accRa, decRa;
        private int accOrder, decOrder;
        
        public Form1()
        {
            InitializeComponent();
            
            accProfile = CamProfile.SymmetricSpeedShift(0.2, 3, 0);
            uniProfile = CamProfile.StraightLine();
            decProfile = CamProfile.SymmetricSpeedShift(0.2, 3, 1);
        }
    }
}
