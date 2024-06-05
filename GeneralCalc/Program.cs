// See https://aka.ms/new-console-template for more information

using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

Console.WriteLine("Hello, World!");

var ret = Integrate.OnClosedInterval((x) => x, 0, 10);
Console.WriteLine(ret);

