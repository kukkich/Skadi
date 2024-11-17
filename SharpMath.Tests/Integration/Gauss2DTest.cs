using Microsoft.Extensions.Logging.Abstractions;
using SharpMath.Geometry._1D;
using SharpMath.Geometry._2D;
using SharpMath.Integration;

namespace SharpMath.Tests.Integration;

public class Gauss2DTest
{
    private Gauss2D integrator2Degree;
    private Gauss2D integrator4Degree;

    [SetUp]
    public void Setup()
    {
        integrator2Degree = new Gauss2D(GaussConfig.Gauss2(1), NullLogger.Instance);
    }

    [Test]
    public void ShouldCalculateWithoutErrorOnUnitInterval()
    {
        var xLine = new Line1D(0, 1);
        var yLine = new Line1D(0, 1);
        Func<Point2D, double> f = p => 5 + 2 * p.X - 7 * p.Y + 3 * p.X * p.Y
            - 3 * p.X * p.X * p.Y + p.X * p.Y * p.Y
            + 11d / 2 * Math.Pow(p.X, 3) * Math.Pow(p.Y, 3);
        //https://ru.symbolab.com/solver/double-integrals-calculator/%5Cint_%7B0%7D%5E%7B1%7D%5Cint_%7B0%7D%5E%7B1%7D%205%20%2B2x-7y%2B3xy%20-3x%5E%7B2%7D%5Ccdot%20y%20%2B%20xy%5E%7B2%7D%20%2B%20%5Cfrac%7B11%7D%7B2%7D%20%5Ccdot%20x%5E%7B3%7Dy%20%5E%7B3%7Ddxdy?or=input
        const double expected = 313d / 96;

        var result = integrator2Degree.Calculate(f, xLine, yLine);

        Assert.That(Math.Abs(expected - result), Is.LessThan(1e-14));
    }

    [Test]
    public void ShouldCalculateWithoutErrorOnAnyInterval()
    {
        var xLine = new Line1D(-3, 4);
        var yLine = new Line1D(7, 11);
        Func<Point2D, double> f = p => 5 + 2 * p.X - 7 * p.Y + 3 * p.X * p.Y
            - 3 * p.X * p.X * p.Y + p.X * p.Y * p.Y
            + 11d / 2 * Math.Pow(p.X, 3) * Math.Pow(p.Y, 3);
        //https://ru.symbolab.com/solver/double-integrals-calculator/%5Cint_%7B7%7D%5E%7B11%7D%5Cint_%7B-3%7D%5E%7B4%7D5%2B2x-7y%2B3xy-3x%5E%7B2%7D%5Ccdot%20%20y%2Bxy%5E%7B2%7D%2B%5Cfrac%7B11%7D%7B2%7D%5Ccdot%20%20x%5E%7B3%7Dy%5E%7B3%7Ddxdy?or=input
        const double expected = 4397827d / 6;

        var result = integrator2Degree.Calculate(f, xLine, yLine);

        Assert.That(Math.Abs(expected - result), Is.LessThan(1e-9));
    }
    
    [Test]
    public void ShouldCalculateWithSomeErrorOnNonPolynomials()
    {
        var xLine = new Line1D(-3, 4);
        var yLine = new Line1D(7, 11);
        Func<Point2D, double> f = p => 
            (5 + 2 * p.X - 7 * p.Y + 3 * p.X * p.Y 
             - 3 * p.X * p.X * p.Y + p.X * p.Y * p.Y) / 
            ((p.X + 4) * (p.Y + 5) - p.X * p.Y);
        const double expected = -74.8631548557;

        var result = integrator2Degree.Calculate(f, xLine, yLine);
        
        Assert.That(Math.Abs(expected - result), Is.LessThan(2));
    }
}