// ReSharper disable InconsistentNaming
namespace MotionProfiler.BRAutomat;

public enum AutEventAttr
{
    ncST_END,
    ncAT_ONCE
}

public enum AutEventTyp
{
    ncOFF,
    ncST_END,
    ncS_START,
    ncS_START_IV,
    ncCOUNT,
    ncTRIGGER,
    ncPAR_ID,
    ncSIGNAL,
    ncAND_N2E
}

public enum AutCompMode
{
    ncOFF,
    ncONLYCOMP,
    ncONLYCOMP_DIRECT,
    ncWITH_CAM,
    ncMA_LATCHPOS,
    ncSL_LATCHPOS,
    ncSL_ABS,
    ncV_COMP_A_SL,
    ncV_COMP_S_MA,
    ncV_COMP_S_SL,
    ncV_COMP_A_CYC
}