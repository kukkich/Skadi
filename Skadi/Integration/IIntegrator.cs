using System.Numerics;
using Skadi.Geometry._1D;
using Skadi.Geometry._2D;

namespace Skadi.Integration;

public interface IIntegrator2D
{
    public T Calculate<T>(Func<Vector2D, T> f, Line1D xInterval, Line1D yInterval)
        where T : IAdditiveIdentity<T, T>, IAdditionOperators<T, T, T>, IMultiplyOperators<T, double, T>;
}