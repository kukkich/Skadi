using SharpMath.Geometry._1D;
using SharpMath.Geometry._2D;

namespace SharpMath.Integration;

public interface IIntegrator2D
{
    public double Calculate(Func<Point2D, double> f, Line1D xInterval, Line1D yInterval);
}