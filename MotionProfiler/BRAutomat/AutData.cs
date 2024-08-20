namespace MotionProfiler.BRAutomat;







public class AutData
{
    public AutData()
    {
        for (var i = 0; i < 15; i++)
        {
            State[i] = new AutDataState();
        }
    }
    public AutDataState[] State = new AutDataState[15];
}