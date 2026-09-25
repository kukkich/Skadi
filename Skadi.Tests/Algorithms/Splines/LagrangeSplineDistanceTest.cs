using Skadi.Algorithms.Splines._1D.CubicLagrange;
using Skadi.FEM._1D.BasisFunctions;
using Skadi.FEM.Core.Geometry;
using Skadi.Geometry._1D;
using Skadi.Geometry.Splitting;
using Skadi.LinearAlgebra.Vectors;

namespace Skadi.Tests.Algorithms.Splines;

public class LagrangeSplineDistanceTest
{
    private static LagrangeSpline CreateSpline(
        double[] nodes,
        double[] weights)
    {
        var points = new IrregularPointsCollection<double>(nodes);
        var elements = new IElement[nodes.Length - 1];

        for (var i = 0; i < elements.Length; i++)
            elements[i] = new Element(0, [i, i + 1]);

        var grid = new Grid<double, IElement>(points, elements);
        var provider = new LagrangeCubicFunction1DProvider(grid);

        return new LagrangeSpline(provider, grid, new Vector(weights));
    }

    [Test]
    public void DistanceBetween_EqualSplines_ShouldBeZero()
    {
        double[] nodes = [0, 1];
        double[] weights = [1, 2, 3, 4];

        var s1 = CreateSpline(nodes, weights);
        var s2 = CreateSpline(nodes, weights);

        var d = LagrangeSpline.DistanceBetween(s1, s2);

        Assert.That(d, Is.EqualTo(0).Within(1e-14));
    }

    // ------------------------------------------------------------
    // 2) f(x) = 0, g(x) = 1 → расстояние всегда = -1
    // ------------------------------------------------------------
    [Test]
    public void DistanceBetween_ConstantDifference_ShouldMatch()
    {
        double[] nodes = [0, 1];

        double[] w1 = [0, 0, 0, 0]; // f(x) = 0
        double[] w2 = [1, 1, 1, 1]; // g(x) = 1

        var f = CreateSpline(nodes, w1);
        var g = CreateSpline(nodes, w2);

        var d = LagrangeSpline.DistanceBetween(f, g);

        // |f - g| = 1
        Assert.That(d, Is.EqualTo(1).Within(1e-14));
    }

    // ------------------------------------------------------------
    // 3) f(x)=0, g(x)=полином → проверить min(f-g)
    //    g(t) = ψ1(t)  (например, берём один ненулевой вес)
    // ------------------------------------------------------------
    [Test]
    public void DistanceBetween_SingleBasisFunction_ShouldMatchAnalyticalMin()
    {
        double[] nodes = [0, 1];

        
        double[] w1 = [0, 0, 0, 0]; // f(x) = 0
        double[] w2 = [0, 1, 0, 0]; // g(x) = ψ2(t)

        var f = CreateSpline(nodes, w1);
        var g = CreateSpline(nodes, w2);

        var d = LagrangeSpline.DistanceBetween(f, g);

        // Минимум ψ2(t) на [0,1] для Lagrange cubic basis:
        // ψ2(t) = 27/2 * t*(t - 2/3)*(t - 1)
        var brute = double.PositiveInfinity;
        foreach (var x in new UniformSplitter(10000)
                     .EnumerateValues(new Line1D(0, 1)))
        {
            var diff = Math.Abs(f.Calculate(x) - g.Calculate(x));
            brute = double.Min(brute, diff);
        }
        
        Assert.That(d, Is.EqualTo(brute).Within(1e-3));
    }

    // ------------------------------------------------------------
    // 4) Два элемента: проверить, что минимум находится на нужном
    // ------------------------------------------------------------
    [Test]
    public void DistanceBetween_TwoElements_ShouldPickSmallestLocalMin()
    {
        double[] nodes = [0, 1, 2];

        double[] w1 = // f=0
        [
            -1, -1, 2, 1.5,
            1, -7, 3
        ];
        double[] w2 =
        [
            0, 1, 0, 1,
            1, 0, 1
        ];

        var f = CreateSpline(nodes, w1);
        var g = CreateSpline(nodes, w2);

        var d = LagrangeSpline.DistanceBetween(f, g);

        var brute = double.PositiveInfinity;
        foreach (var x in new UniformSplitter(40000)
                     .EnumerateValues(new Line1D(0, 2)))
        {
            var diff = Math.Abs(f.Calculate(x) - g.Calculate(x));
            brute = double.Min(brute, diff);
        }

        Assert.That(d, Is.EqualTo(brute).Within(5e-4));
    }

    // ------------------------------------------------------------
    // 5) Более сложный тест: f и g — разные полиномы
    // ------------------------------------------------------------
    [Test]
    public void DistanceBetween_NonTrivialPolynomials_ShouldBeCorrect()
    {
        double[] nodes = [0, 1];

        // f(t) = t → задаём через примерные веса
        double[] w1 = [0, 1, 1, 1];
        // g(t) = 0.5*t^2 → аппроксимация
        double[] w2 = [0, 0.2, 0.7, 1.0];

        var f = CreateSpline(nodes, w1);
        var g = CreateSpline(nodes, w2);

        var d = LagrangeSpline.DistanceBetween(f, g);

        var brute = double.PositiveInfinity;
        foreach (var x in new UniformSplitter(10000)
                     .EnumerateValues(new Line1D(0, 1)))
        {
            var diff = Math.Abs(f.Calculate(x) - g.Calculate(x));
            brute = double.Min(brute, diff);
        }

        Assert.That(d, Is.EqualTo(brute).Within(5e-4));
    }
}