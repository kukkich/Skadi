using Skadi.Geometry._1D;
using Skadi.Geometry._2D;

namespace Skadi.Integration;

public interface IIntegrator2D
{
    public double Calculate(Func<Vector2D, double> f, Line1D xInterval, Line1D yInterval);
}