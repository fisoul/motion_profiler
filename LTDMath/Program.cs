// See https://aka.ms/new-console-template for more information

using MathNet.Symbolics;

// Declare the symbol 'x'
var x = Expression.Symbol("x");
var x1 = SymbolicExpression.Variable("x");

// Define the equation y = 2x^2
var y = 2 * (x1 * x1);

// Differentiate y with respect to x
var yPrime = y.Differentiate(x1);

// Show the derivative 
Console.WriteLine("y' = " + yPrime); // out: y' = 4*x