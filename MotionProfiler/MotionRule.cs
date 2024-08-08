namespace MotionProfiler;

public enum MotionTask
{
    Rest,
    ConstantSpeed,
    Reversal,
    Movement
}

public enum MotionRule
{
    StraightLine,
    PolynomialOrder5,
    Even,
    QuadraticParabola,
    SimpleSine,
    SlopingSine,
    ModifiedSine,
    SimpleTrapezoid,
    ModifiedTrapezoid,
    HarmonicCombination
}