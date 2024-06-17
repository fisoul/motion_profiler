namespace LTDMath;

using System;
using System.Collections.Generic;

public class Polynomial
{
    public List<double> Coefficients { get; private set; }

    public Polynomial(List<double> coefficients)
    {
        Coefficients = new List<double>(coefficients);
    }

    // 平移和拉伸多项式沿x轴和y轴
    public Polynomial Transform(double h, double k, double a, double b)
    {
        return Stretch(a, b).Translate(h, k);
    }

    // 先平移后拉伸多项式沿x轴和y轴
    public Polynomial TranslateThenStretch(double h, double k, double a, double b)
    {
        return Translate(h, k).Stretch(a, b);
    }

    // 仅平移多项式沿x轴和y轴
    public Polynomial Translate(double h, double k)
    {
        int degree = Coefficients.Count - 1;
        List<double> newCoefficients = new List<double>(new double[degree + 1]);

        // 处理x轴平移
        for (int i = 0; i <= degree; i++)
        {
            double coeff = Coefficients[i];
            for (int j = 0; j <= i; j++)
            {
                newCoefficients[j] += coeff * BinomialCoefficient(i, j) * Math.Pow(-h, i - j);
            }
        }

        // 处理y轴平移
        newCoefficients[0] += k;

        return new Polynomial(newCoefficients);
    }

    // 仅拉伸多项式沿x轴和y轴
    public Polynomial Stretch(double a, double b)
    {
        int degree = Coefficients.Count - 1;
        List<double> newCoefficients = new List<double>(new double[degree + 1]);

        // 处理x轴拉伸
        for (int i = 0; i <= degree; i++)
        {
            double coeff = Coefficients[i];
            newCoefficients[i] = coeff / Math.Pow(a, i);
        }

        // 处理y轴拉伸
        for (int i = 0; i <= degree; i++)
        {
            newCoefficients[i] *= b;
        }

        return new Polynomial(newCoefficients);
    }

    // 计算二项式系数
    private double BinomialCoefficient(int n, int k)
    {
        if (k > n) return 0;
        if (k == 0 || k == n) return 1;
        double result = 1;
        for (int i = 1; i <= k; i++)
        {
            result *= (n - (k - i));
            result /= i;
        }

        return result;
    }

    // 计算多项式的值
    public double Evaluate(double x)
    {
        double result = 0;
        for (int i = Coefficients.Count - 1; i >= 0; i--)
        {
            result = result * x + Coefficients[i];
        }

        return result;
    }

    public override string ToString()
    {
        string result = "";
        for (int i = Coefficients.Count - 1; i >= 0; i--)
        {
            if (Coefficients[i] != 0)
            {
                if (result != "" && Coefficients[i] > 0)
                {
                    result += " + ";
                }
                else if (result != "" && Coefficients[i] < 0)
                {
                    result += " - ";
                }
                else if (Coefficients[i] < 0)
                {
                    result += "-";
                }

                if (Math.Abs(Coefficients[i]) != 1 || i == 0)
                {
                    result += Math.Abs(Coefficients[i]);
                }

                if (i > 0)
                {
                    result += "x";
                    if (i > 1)
                    {
                        result += "^" + i;
                    }
                }
            }
        }

        return result;
    }
}