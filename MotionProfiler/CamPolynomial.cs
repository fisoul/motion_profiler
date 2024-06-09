using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;

/*
 * Modify from MathNet.Numerics.Polynomial
 */
namespace MotionProfiler;

/// <summary>
/// A single-variable polynomial with real-valued coefficients and non-negative exponents.
/// </summary>
[Serializable]
public class CamPolynomial : IFormattable, IEquatable<CamPolynomial>, ICloneable
{
    /// <summary>Only needed for the ToString method</summary>
    [DataMember(Order = 2)] public string VariableName = "x";

    /// <summary>The coefficients of the polynomial in a</summary>
    [DataMember(Order = 1)]
    [JsonInclude]
    public double[] Coefficients { get; }

    public double XMax { get; set; }

    /// <summary>
    /// Degree of the polynomial, i.e. the largest monomial exponent. For example, the degree of y=x^2+x^5 is 5, for y=3 it is 0.
    /// The null-polynomial returns degree -1 because the correct degree, negative infinity, cannot be represented by integers.
    /// </summary>
    public int Degree => EvaluateDegree(Coefficients);

    /// <summary>
    /// Create a zero-polynomial with a coefficient array of the given length.
    /// An array of length N can support polynomials of a degree of at most N-1.
    /// </summary>
    /// <param name="n">Length of the coefficient array</param>
    public CamPolynomial(int n)
    {
        Coefficients =
            n >= 0 ? new double[n] : throw new ArgumentOutOfRangeException(nameof(n), "n must be non-negative");
    }

    /// <summary>Create a zero-polynomial</summary>
    public CamPolynomial()
    {
        Coefficients = [];
    }

    /// <summary>
    /// Create a constant polynomial.
    /// Example: 3.0 -&gt; "p : x -&gt; 3.0"
    /// </summary>
    /// <param name="coefficient">The coefficient of the "x^0" monomial.</param>
    public CamPolynomial(double coefficient)
    {
        Coefficients = coefficient == 0.0 ? [] : new double[1] { coefficient };
    }

    /// <summary>
    /// Create a polynomial with the provided coefficients (in ascending order, where the index matches the exponent).
    /// Example: {5, 0, 2} -&gt; "p : x -&gt; 5 + 0 x^1 + 2 x^2".
    /// </summary>
    /// <param name="coefficients">CamPolyNew coefficients as array</param>
    public CamPolynomial(params double[] coefficients)
    {
        Coefficients = coefficients;
    }

    /// <summary>
    /// Create a polynomial with the provided coefficients (in ascending order, where the index matches the exponent).
    /// Example: {5, 0, 2} -&gt; "p : x -&gt; 5 + 0 x^1 + 2 x^2".
    /// </summary>
    /// <param name="coefficients">CamPolyNew coefficients as enumerable</param>
    public CamPolynomial(IEnumerable<double> coefficients)
        : this(coefficients.ToArray())
    {
    }

    public static CamPolynomial Zero => new();

    /// <summary>
    /// Least-Squares fitting the points (x,y) to a k-order polynomial y : x -&gt; p0 + p1*x + p2*x^2 + ... + pk*x^k
    /// </summary>
    // public static CamPolyNew Fit(double[] x, double[] y, int order, DirectRegressionMethod method = DirectRegressionMethod.QR)
    // {
    //   return new CamPolyNew(MathNet.Numerics.Fit.CamPolyNew(x, y, order, method));
    // }
    private static int EvaluateDegree(double[] coefficients)
    {
        for (var degree = coefficients.Length - 1; degree >= 0; --degree)
            if (coefficients[degree] != 0.0)
                return degree;
        return -1;
    }

    public CamPolynomial Shift(double xShift, double yShift)
    {
        int degree = Coefficients.Length - 1; // Highest degree
        double[] newCoefficients = new double[Coefficients.Length];

        for (int i = 0; i <= degree; ++i)
        {
            for (int j = i; j <= degree; ++j)
            {
                newCoefficients[j] += Coefficients[i] * Math.Pow(xShift, j - i) * Choose(j, i);
            }
        }

        // Vertical shift
        newCoefficients[0] += yShift;

        return new CamPolynomial(newCoefficients);
    }
    
    public CamPolynomial Scale(double xScale, double yScale)
    {
        //Y Axis scale
        var newCoefficients = new double[Coefficients.Length];
        for (int i = 0; i < newCoefficients.Length; i++)
        {
            newCoefficients[i] = Coefficients[i] * yScale; //simply multiply each coefficient by yScale
        }

        //X Axis scale
        for (int i = 0; i < newCoefficients.Length; i++)
        {
            newCoefficients[i] *= Math.Pow(xScale, i); //divide coefficient by power of xScale 
        }
        //return new polynomial with scaled coefficients
        return new CamPolynomial(newCoefficients); 
    }

    public CamPolynomial Transform(double xScale, double yScale, double xShift, double yShift)
    {
        return Scale(xScale, yScale).Shift(xShift, yShift);
    }
    
    public static long Choose(int n, int k)
    {
        if (k == 0) return 1;
        if (n == 0) return 0;
    
        long result = 1;
        for (int i = 1; i <= k; ++i)
        {
            result *= n--;
            result /= i;
        }
        return result;
    }
    
    
    /// <summary>
    /// Evaluate a polynomial at point x.
    /// Coefficients are ordered ascending by power with power k at index k.
    /// Example: coefficients [3,-1,2] represent y=2x^2-x+3.
    /// </summary>
    /// <param name="z">The location where to evaluate the polynomial at.</param>
    /// <param name="yShift"></param>
    /// <param name="coefficients">The coefficients of the polynomial, coefficient for power k at index k.</param>
    /// <param name="xStretch"></param>
    /// <param name="yStretch"></param>
    /// <param name="xShift"></param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="coefficients" /> is a null reference.
    /// </exception>
    public static double Evaluate(double z, double xStretch = 1, double yStretch = 1, double xShift = 0, double yShift = 0,  params double[]? coefficients)
    {
        var num1 = coefficients?.Length ?? throw new ArgumentNullException(nameof(coefficients));
        if (num1 == 0)
            return 0.0;
        var num2 = coefficients[num1 - 1] * yStretch;
        var transformedZ = xStretch * (z - xShift);
        for (var index = num1 - 2; index >= 0; --index)
            num2 = num2 * transformedZ + coefficients[index] * yStretch;
        return num2 + yShift;
    }

    /// <summary>Evaluate a polynomial at point x.</summary>
    /// <param name="z">The location where to evaluate the polynomial at.</param>
    /// <param name="xStretch"></param>
    /// <param name="yStretch"></param>
    /// <param name="xShift"></param>
    /// <param name="yShift"></param>
    public double Evaluate(double z, double xStretch = 1, double yStretch = 1, double xShift = 0, double yShift = 0 )
    {
        return Evaluate(z, xStretch, yStretch, xShift, yShift, Coefficients);
    }
    
    public CamPolynomial Differentiate(int order = 1)
    {
        if (order == 0) return Clone();
        if (order > 1) return Differentiate().Differentiate(order - 1);
        var degree = Degree;
        if (degree < 0)
            return this;
        if (degree == 0)
            return Zero;
        var numArray = new double[degree];
        for (var index = 0; index < numArray.Length; ++index)
            numArray[index] = Coefficients[index + 1] * (index + 1);
        return new CamPolynomial(numArray);
    }

    public CamPolynomial Integrate(int order = 1)
    {
        if (order > 1) return Integrate().Integrate(order - 1);
        var degree = Degree;
        if (degree < 0)
            return this;
        var numArray = new double[degree + 2];
        for (var index = 1; index < numArray.Length; ++index)
            numArray[index] = Coefficients[index - 1] / index;
        return new CamPolynomial(numArray);
    }

    /// <summary>Addition of two CamPolyNews (point-wise).</summary>
    /// <param name="a">Left CamPolyNew</param>
    /// <param name="b">Right CamPolyNew</param>
    /// <returns>Resulting CamPolyNew</returns>
    public static CamPolynomial Add(CamPolynomial a, CamPolynomial b)
    {
        var coefficients1 = a.Coefficients;
        var coefficients2 = b.Coefficients;
        var numArray = new double[Math.Max(a.Degree, b.Degree) + 1];
        var num1 = Math.Min(Math.Min(coefficients1.Length, coefficients2.Length), numArray.Length);
        for (var index = 0; index < num1; ++index)
            numArray[index] = coefficients1[index] + coefficients2[index];
        var num2 = Math.Min(coefficients1.Length, numArray.Length);
        for (var index = num1; index < num2; ++index)
            numArray[index] = coefficients1[index];
        var num3 = Math.Min(coefficients2.Length, numArray.Length);
        for (var index = num1; index < num3; ++index)
            numArray[index] = coefficients2[index];
        return new CamPolynomial(numArray);
    }

    /// <summary>Addition of a polynomial and a scalar.</summary>
    public static CamPolynomial Add(CamPolynomial a, double b)
    {
        var coefficients = a.Coefficients;
        var numArray = new double[Math.Max(a.Degree, 0) + 1];
        var num = Math.Min(coefficients.Length, numArray.Length);
        for (var index = 0; index < num; ++index)
            numArray[index] = coefficients[index];
        numArray[0] += b;
        return new CamPolynomial(numArray);
    }

    /// <summary>Subtraction of two CamPolyNews (point-wise).</summary>
    /// <param name="a">Left CamPolyNew</param>
    /// <param name="b">Right CamPolyNew</param>
    /// <returns>Resulting CamPolyNew</returns>
    public static CamPolynomial Subtract(CamPolynomial a, CamPolynomial b)
    {
        var coefficients1 = a.Coefficients;
        var coefficients2 = b.Coefficients;
        var numArray = new double[Math.Max(a.Degree, b.Degree) + 1];
        var num1 = Math.Min(Math.Min(coefficients1.Length, coefficients2.Length), numArray.Length);
        for (var index = 0; index < num1; ++index)
            numArray[index] = coefficients1[index] - coefficients2[index];
        var num2 = Math.Min(coefficients1.Length, numArray.Length);
        for (var index = num1; index < num2; ++index)
            numArray[index] = coefficients1[index];
        var num3 = Math.Min(coefficients2.Length, numArray.Length);
        for (var index = num1; index < num3; ++index)
            numArray[index] = -coefficients2[index];
        return new CamPolynomial(numArray);
    }

    /// <summary>Addition of a scalar from a polynomial.</summary>
    public static CamPolynomial Subtract(CamPolynomial a, double b)
    {
        return Add(a, -b);
    }

    /// <summary>Addition of a polynomial from a scalar.</summary>
    public static CamPolynomial Subtract(double b, CamPolynomial a)
    {
        var coefficients = a.Coefficients;
        var numArray = new double[Math.Max(a.Degree, 0) + 1];
        var num = Math.Min(coefficients.Length, numArray.Length);
        for (var index = 0; index < num; ++index)
            numArray[index] = -coefficients[index];
        numArray[0] += b;
        return new CamPolynomial(numArray);
    }

    /// <summary>Negation of a polynomial.</summary>
    public static CamPolynomial Negate(CamPolynomial a)
    {
        var coefficients = a.Coefficients;
        var numArray = new double[a.Degree + 1];
        for (var index = 0; index < numArray.Length; ++index)
            numArray[index] = -coefficients[index];
        return new CamPolynomial(numArray);
    }

    /// <summary>Multiplies a polynomial by a polynomial (convolution)</summary>
    /// <param name="a">Left polynomial</param>
    /// <param name="b">Right polynomial</param>
    /// <returns>Resulting CamPolyNew</returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="a" /> or <paramref name="b" /> is a null reference.
    /// </exception>
    public static CamPolynomial Multiply(CamPolynomial a, CamPolynomial b)
    {
        if (a == null)
            throw new ArgumentNullException(nameof(a));
        if (b == null)
            throw new ArgumentNullException(nameof(b));
        var degree1 = a.Degree;
        var degree2 = b.Degree;
        if (degree1 < 0 || degree2 < 0)
            return Zero;
        var coefficients1 = a.Coefficients;
        var coefficients2 = b.Coefficients;
        var numArray = new double[degree1 + degree2 + 1];
        for (var index1 = 0; index1 <= degree1; ++index1)
        for (var index2 = 0; index2 <= degree2; ++index2)
            numArray[index1 + index2] += coefficients1[index1] * coefficients2[index2];
        return new CamPolynomial(numArray);
    }

    /// <summary>Scales a polynomial by a scalar</summary>
    /// <param name="a">CamPolyNew</param>
    /// <param name="k">Scalar value</param>
    /// <returns>Resulting CamPolyNew</returns>
    public static CamPolynomial Multiply(CamPolynomial a, double k)
    {
        var coefficients = a.Coefficients;
        var numArray = new double[a.Degree + 1];
        for (var index = 0; index < numArray.Length; ++index)
            numArray[index] = coefficients[index] * k;
        return new CamPolynomial(numArray);
    }

    /// <summary>Scales a polynomial by division by a scalar</summary>
    /// <param name="a">CamPolyNew</param>
    /// <param name="k">Scalar value</param>
    /// <returns>Resulting CamPolyNew</returns>
    public static CamPolynomial Divide(CamPolynomial a, double k)
    {
        var coefficients = a.Coefficients;
        var numArray = new double[a.Degree + 1];
        for (var index = 0; index < numArray.Length; ++index)
            numArray[index] = coefficients[index] / k;
        return new CamPolynomial(numArray);
    }

    /// <summary>
    /// Euclidean long division of two polynomials, returning the quotient q and remainder r of the two polynomials a and b such that a = q*b + r
    /// </summary>
    /// <param name="a">Left polynomial</param>
    /// <param name="b">Right polynomial</param>
    /// <returns>A tuple holding quotient in first and remainder in second</returns>
    public static (CamPolynomial, CamPolynomial) DivideRemainder(CamPolynomial a, CamPolynomial b)
    {
        var degree1 = b.Degree;
        if (degree1 < 0)
            throw new DivideByZeroException("b polynomial ends with zero");
        var degree2 = a.Degree;
        if (degree2 < 0)
            return (a, a);
        if (degree1 == 0)
            return (Divide(a, b.Coefficients[0]), Zero);
        if (degree2 < degree1)
            return (Zero, a);
        var array1 = a.Coefficients.ToArray();
        var array2 = b.Coefficients.ToArray();
        var num1 = array2[degree1];
        var numArray1 = new double[degree1];
        for (var index = 0; index < numArray1.Length; ++index)
            numArray1[index] = array2[index] / num1;
        var num2 = degree2 - degree1;
        var index1 = degree2;
        while (num2 >= 0)
        {
            var num3 = array1[index1];
            for (var index2 = num2; index2 < index1; ++index2)
                array1[index2] -= numArray1[index2 - num2] * num3;
            --num2;
            --index1;
        }

        var length1 = index1 + 1;
        var length2 = degree2 - index1;
        var numArray2 = new double[length2];
        for (var index3 = 0; index3 < length2; ++index3)
            numArray2[index3] = array1[index3 + length1] / num1;
        var numArray3 = new double[length1];
        for (var index4 = 0; index4 < length1; ++index4)
            numArray3[index4] = array1[index4];
        return (new CamPolynomial(numArray2), new CamPolynomial(numArray3));
    }

    /// <summary>Point-wise division of two CamPolyNews</summary>
    /// <param name="a">Left CamPolyNew</param>
    /// <param name="b">Right CamPolyNew</param>
    /// <returns>Resulting CamPolyNew</returns>
    public static CamPolynomial PointwiseDivide(CamPolynomial a, CamPolynomial b)
    {
        var coefficients1 = a.Coefficients;
        var coefficients2 = b.Coefficients;
        var numArray = new double[a.Degree + 1];
        var num = Math.Min(Math.Min(coefficients1.Length, coefficients2.Length), numArray.Length);
        for (var index = 0; index < num; ++index)
            numArray[index] = coefficients1[index] / coefficients2[index];
        for (var index = num; index < numArray.Length; ++index)
            numArray[index] = coefficients1[index] / 0.0;
        return new CamPolynomial(numArray);
    }

    /// <summary>Point-wise multiplication of two CamPolyNews</summary>
    /// <param name="a">Left CamPolyNew</param>
    /// <param name="b">Right CamPolyNew</param>
    /// <returns>Resulting CamPolyNew</returns>
    public static CamPolynomial PointwiseMultiply(CamPolynomial a, CamPolynomial b)
    {
        var coefficients1 = a.Coefficients;
        var coefficients2 = b.Coefficients;
        var numArray = new double[Math.Min(a.Degree, b.Degree) + 1];
        for (var index = 0; index < numArray.Length; ++index)
            numArray[index] = coefficients1[index] * coefficients2[index];
        return new CamPolynomial(numArray);
    }

    /// <summary>
    /// Division of two polynomials returning the quotient-with-remainder of the two polynomials given
    /// </summary>
    /// <param name="b">Right polynomial</param>
    /// <returns>A tuple holding quotient in first and remainder in second</returns>
    public (CamPolynomial, CamPolynomial) DivideRemainder(CamPolynomial b)
    {
        return DivideRemainder(this, b);
    }

    /// <summary>Addition of two CamPolyNews (piecewise)</summary>
    /// <param name="a">Left polynomial</param>
    /// <param name="b">Right polynomial</param>
    /// <returns>Resulting CamPolyNew</returns>
    public static CamPolynomial operator +(CamPolynomial a, CamPolynomial b)
    {
        return Add(a, b);
    }

    /// <summary>adds a scalar to a polynomial.</summary>
    /// <param name="a">CamPolyNew</param>
    /// <param name="k">Scalar value</param>
    /// <returns>Resulting CamPolyNew</returns>
    public static CamPolynomial operator +(CamPolynomial a, double k)
    {
        return Add(a, k);
    }

    /// <summary>adds a scalar to a polynomial.</summary>
    /// <param name="k">Scalar value</param>
    /// <param name="a">CamPolyNew</param>
    /// <returns>Resulting CamPolyNew</returns>
    public static CamPolynomial operator +(double k, CamPolynomial a)
    {
        return Add(a, k);
    }

    /// <summary>Subtraction of two polynomial.</summary>
    /// <param name="a">Left polynomial</param>
    /// <param name="b">Right polynomial</param>
    /// <returns>Resulting CamPolyNew</returns>
    public static CamPolynomial operator -(CamPolynomial a, CamPolynomial b)
    {
        return Subtract(a, b);
    }

    /// <summary>Subtracts a scalar from a polynomial.</summary>
    /// <param name="a">CamPolyNew</param>
    /// <param name="k">Scalar value</param>
    /// <returns>Resulting CamPolyNew</returns>
    public static CamPolynomial operator -(CamPolynomial a, double k)
    {
        return Subtract(a, k);
    }

    /// <summary>Subtracts a polynomial from a scalar.</summary>
    /// <param name="k">Scalar value</param>
    /// <param name="a">CamPolyNew</param>
    /// <returns>Resulting CamPolyNew</returns>
    public static CamPolynomial operator -(double k, CamPolynomial a)
    {
        return Subtract(k, a);
    }

    /// <summary>Negates a polynomial.</summary>
    /// <param name="a">CamPolyNew</param>
    /// <returns>Resulting CamPolyNew</returns>
    public static CamPolynomial operator -(CamPolynomial a)
    {
        return Negate(a);
    }

    /// <summary>Multiplies a polynomial by a polynomial (convolution).</summary>
    /// <param name="a">Left polynomial</param>
    /// <param name="b">Right polynomial</param>
    /// <returns>resulting CamPolyNew</returns>
    public static CamPolynomial operator *(CamPolynomial a, CamPolynomial b)
    {
        return Multiply(a, b);
    }

    /// <summary>Multiplies a polynomial by a scalar.</summary>
    /// <param name="a">CamPolyNew</param>
    /// <param name="k">Scalar value</param>
    /// <returns>Resulting CamPolyNew</returns>
    public static CamPolynomial operator *(CamPolynomial a, double k)
    {
        return Multiply(a, k);
    }

    /// <summary>Multiplies a polynomial by a scalar.</summary>
    /// <param name="k">Scalar value</param>
    /// <param name="a">CamPolyNew</param>
    /// <returns>Resulting CamPolyNew</returns>
    public static CamPolynomial operator *(double k, CamPolynomial a)
    {
        return Multiply(a, k);
    }

    /// <summary>Divides a polynomial by scalar value.</summary>
    /// <param name="a">CamPolyNew</param>
    /// <param name="k">Scalar value</param>
    /// <returns>Resulting CamPolyNew</returns>
    public static CamPolynomial operator /(CamPolynomial a, double k)
    {
        return Divide(a, k);
    }

    /// <summary>
    /// Format the polynomial in ascending order, e.g. "4.3 + 2.0x^2 - x^3".
    /// </summary>
    public override string ToString()
    {
        return ToString("G", CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Format the polynomial in descending order, e.g. "x^3 + 2.0x^2 - 4.3".
    /// </summary>
    public string ToStringDescending()
    {
        return ToStringDescending("G", CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Format the polynomial in ascending order, e.g. "4.3 + 2.0x^2 - x^3".
    /// </summary>
    public string ToString(string format)
    {
        return ToString(format, CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Format the polynomial in descending order, e.g. "x^3 + 2.0x^2 - 4.3".
    /// </summary>
    public string ToStringDescending(string format)
    {
        return ToStringDescending(format, CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Format the polynomial in ascending order, e.g. "4.3 + 2.0x^2 - x^3".
    /// </summary>
    public string ToString(IFormatProvider formatProvider)
    {
        return ToString("G", formatProvider);
    }

    /// <summary>
    /// Format the polynomial in descending order, e.g. "x^3 + 2.0x^2 - 4.3".
    /// </summary>
    public string ToStringDescending(IFormatProvider formatProvider)
    {
        return ToStringDescending("G", formatProvider);
    }

    /// <summary>
    /// Format the polynomial in ascending order, e.g. "4.3 + 2.0x^2 - x^3".
    /// </summary>
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        if (Degree < 0)
            return "0";
        var stringBuilder = new StringBuilder();
        var flag = true;
        for (var index = 0; index < Coefficients.Length; ++index)
        {
            var coefficient = Coefficients[index];
            if (coefficient != 0.0)
            {
                if (flag)
                {
                    stringBuilder.Append(coefficient.ToString(format, formatProvider));
                    if (index > 0)
                        stringBuilder.Append(VariableName);
                    if (index > 1)
                    {
                        stringBuilder.Append("^");
                        stringBuilder.Append(index);
                    }

                    flag = false;
                }
                else
                {
                    if (coefficient < 0.0)
                    {
                        stringBuilder.Append(" - ");
                        stringBuilder.Append((-coefficient).ToString(format, formatProvider));
                    }
                    else
                    {
                        stringBuilder.Append(" + ");
                        stringBuilder.Append(coefficient.ToString(format, formatProvider));
                    }

                    if (index > 0)
                        stringBuilder.Append(VariableName);
                    if (index > 1)
                    {
                        stringBuilder.Append("^");
                        stringBuilder.Append(index);
                    }
                }
            }
        }

        return stringBuilder.ToString();
    }

    /// <summary>
    /// Format the polynomial in descending order, e.g. "x^3 + 2.0x^2 - 4.3".
    /// </summary>
    public string ToStringDescending(string format, IFormatProvider formatProvider)
    {
        if (Degree < 0)
            return "0";
        var stringBuilder = new StringBuilder();
        var flag = true;
        for (var index = Coefficients.Length - 1; index >= 0; --index)
        {
            var coefficient = Coefficients[index];
            if (coefficient != 0.0)
            {
                if (flag)
                {
                    stringBuilder.Append(coefficient.ToString(format, formatProvider));
                    if (index > 0)
                        stringBuilder.Append(VariableName);
                    if (index > 1)
                    {
                        stringBuilder.Append("^");
                        stringBuilder.Append(index);
                    }

                    flag = false;
                }
                else
                {
                    if (coefficient < 0.0)
                    {
                        stringBuilder.Append(" - ");
                        stringBuilder.Append((-coefficient).ToString(format, formatProvider));
                    }
                    else
                    {
                        stringBuilder.Append(" + ");
                        stringBuilder.Append(coefficient.ToString(format, formatProvider));
                    }

                    if (index > 0)
                        stringBuilder.Append(VariableName);
                    if (index > 1)
                    {
                        stringBuilder.Append("^");
                        stringBuilder.Append(index);
                    }
                }
            }
        }

        return stringBuilder.ToString();
    }

    public bool Equals(CamPolynomial? other)
    {
        if (other == null)
            return false;
        // ReSharper disable once PossibleUnintendedReferenceComparison
        if (this == other)
            return true;
        var degree = Degree;
        if (degree != other.Degree)
            return false;
        for (var index = 0; index <= degree; ++index)
            if (!Coefficients[index].Equals(other.Coefficients[index]))
                return false;
        return true;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null)
            return false;
        if (this == obj)
            return true;
        return !(obj.GetType() != typeof(CamPolynomial)) && Equals((CamPolynomial)obj);
    }

    public override int GetHashCode()
    {
        var num = Math.Min(Degree + 1, 25);
        var hashCode = 17;
        for (var index = 0; index < num; ++index)
            hashCode = hashCode * 31 + Coefficients[index].GetHashCode();
        return hashCode;
    }

    public CamPolynomial Clone()
    {
        var destinationArray = new double[EvaluateDegree(Coefficients) + 1];
        Array.Copy(Coefficients, destinationArray, destinationArray.Length);
        return new CamPolynomial(destinationArray);
    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    object ICloneable.Clone()
    {
        return Clone();
    }
}