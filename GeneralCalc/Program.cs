// See https://aka.ms/new-console-template for more information
using GeneralCalc;

// 原始多项式
var poly = new Polynomial([0, 1, 2, 3, 4]);
Console.WriteLine("Original Polynomial: " + poly);

// 定义平移和拉伸的变量
const double xTranslate = 10; // x 方向平移
const double yTranslate = 2; // y 方向平移
const double xStretch = 2; // x 方向拉伸系数
const double yStretch = 2; // y 方向拉伸系数

// 测试点
const double testPoint = 3;

// 仅平移
var translated = poly.Translate(xTranslate, yTranslate);
Console.WriteLine("Translated Polynomial: " + translated);
var translatedValue = translated.Evaluate(testPoint + xTranslate);
Console.WriteLine($"f({testPoint}) = {poly.Evaluate(testPoint)}, Translated f({testPoint + xTranslate}) = {translatedValue}");

// 仅拉伸
var stretched = poly.Stretch(xStretch, yStretch);
Console.WriteLine("Stretched Polynomial: " + stretched);
var stretchedValue = stretched.Evaluate(testPoint * xStretch);
Console.WriteLine($"f({testPoint}) = {poly.Evaluate(testPoint)}, Stretched f({testPoint * xStretch}) = {stretchedValue}");

// 拉伸后平移
var transformed = poly.Transform(xTranslate, yTranslate, xStretch, yStretch);
Console.WriteLine("Transformed Polynomial (Stretch then Translate): " + transformed);
var transformedValue = transformed.Evaluate(testPoint * xStretch + xTranslate);
Console.WriteLine(
    $"f({testPoint}) = {poly.Evaluate(testPoint)}, Transformed f({testPoint * xStretch + xTranslate}) = {transformedValue}");

// 先平移后拉伸
var translatedThenStretched = poly.TranslateThenStretch(xTranslate, yTranslate, xStretch, yStretch);
Console.WriteLine("Translated Then Stretched Polynomial (Translate then Stretch): " + translatedThenStretched);
var translatedThenStretchedValue = translatedThenStretched.Evaluate((testPoint + xTranslate) * xStretch);
Console.WriteLine(
    $"f({testPoint}) = {poly.Evaluate(testPoint)}, Translated Then Stretched f({(testPoint + xTranslate) * xStretch}) = {translatedThenStretchedValue}");

// 检查变换后的多项式在计算点处的值是否等于原多项式在测试点处的值
var originalValue = poly.Evaluate(testPoint);
Console.WriteLine($"Original f({testPoint}) = {originalValue}");
Console.WriteLine($"Translated f({testPoint + xTranslate}) = {translatedValue}");
Console.WriteLine($"Stretched f({testPoint * xStretch}) = {stretchedValue}");
Console.WriteLine($"Transformed f({testPoint * xStretch + xTranslate}) = {transformedValue}");
Console.WriteLine($"Translated Then Stretched f({(testPoint + xTranslate) * xStretch}) = {translatedThenStretchedValue}");

Console.WriteLine("Are f(x) and Transformed f(x_transformed) equal?");
Console.WriteLine($"Translated: {Math.Abs(originalValue - translatedValue + yTranslate) < 1e-10}");
Console.WriteLine($"Stretched: {Math.Abs(originalValue * yStretch - stretchedValue) < 1e-10}");
Console.WriteLine($"Transformed (Stretch then Translate): {Math.Abs(originalValue * yStretch - transformedValue + yTranslate) < 1e-10}");
Console.WriteLine(
    $"Translated Then Stretched (Translate then Stretch): {Math.Abs((originalValue + yTranslate) * yStretch - translatedThenStretchedValue) < 1e-10}");