namespace MotionProfile;

public class CamProfile
{
    public int MasterPeriod { get; set; }
    public int SlavePeriod { get; set; }
    public int PolynomialNumber { get; set; }
    public List<CamPolynomial> PolyNomialData { get; set; } = [];

    public CamProfile(int masterPeriod, int slavePeriod, IEnumerable<CamPolynomial> polynomials)
    {
        MasterPeriod = masterPeriod;
        SlavePeriod = slavePeriod;
        PolyNomialData.AddRange(polynomials);
        PolynomialNumber = PolyNomialData.Count;
    }

    public static CamProfile PreDefined_FFFF()
    {
        var curve11 = new CamPolynomial
        {
            x_max = 1,
            a1 = 1
        };
        return new CamProfile(1, 1, [curve11]);
    }
}