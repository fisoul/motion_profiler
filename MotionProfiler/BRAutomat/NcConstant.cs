// ReSharper disable InconsistentNaming
namespace MotionProfiler.BRAutomat;

public static class NcConstant
{
    public const char ncP_EDGE = (char)0;
    public const char ncN_EDGE = (char)1;
    public const char ncOFF = (char)0;
}

public enum EventAttr
{
    ncAT_ONCE = 0,
    ncST_END = 12
}

public enum EventType
{
    ncOFF = NcConstant.ncOFF,
    ncS_START = 10,
    ncCOUNT = 11,
    ncST_END = 12,
    ncAND_N2E = 15,
    ncPAR_ID1 = 16,
    ncPAR_ID2 = 17,
    ncPAR_ID3 = 18,
    ncPAR_ID4 = 19,
    ncTRIGGER1_P_EDGE = 20 + NcConstant.ncP_EDGE,
    ncTRIGGER1_N_EDGE = 20 + NcConstant.ncN_EDGE,
    ncTRIGGER2_P_EDGE = 22 + NcConstant.ncP_EDGE,
    ncTRIGGER2_N_EDGE = 22 + NcConstant.ncN_EDGE,
    ncS_START_IV1 = 41,
    ncS_START_IV2 = 42,
    ncS_START_IV3 = 43,
    ncS_START_IV4 = 44,
    ncSIGNAL1 = 91,
    ncSIGNAL2 = 92,
    ncSIGNAL3 = 93,
    ncSIGNAL4 = 94
}

public enum CompMode
{
    ncOFF = NcConstant.ncOFF,
    ncONLYCOMP = 30,
    ncWITH_CAM = 31,
    ncMA_LATCHPOS = 32,
    ncSL_ABS = 33,
    ncSL_LATCHPOS = 34,
    ncONLYCOMP_DIRECT = 35,
    ncV_COMP_A_SL = 36,
    ncV_COMP_S_MA = 37,
    ncV_COMP_S_SL = 38,
    ncV_COMP_A_CYC = 39,
    ncMA_SL_ABS = 40
}