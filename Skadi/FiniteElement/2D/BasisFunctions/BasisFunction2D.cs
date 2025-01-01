using Skadi.Geometry._2D;
using Skadi.FiniteElement.Core.BasisFunctions;

namespace Skadi.FiniteElement._2D.BasisFunctions;

public class BasisFunction2D : IBasisFunction<Point2D>
{
    private readonly Func<double, double> _xFunction;
    private readonly Func<double, double> _yFunction;

    public BasisFunction2D(Func<double, double> xFunction, Func<double, double> yFunction)
    {
        _xFunction = xFunction;
        _yFunction = yFunction;
    }

    public double Evaluate(Point2D point)
    {
        return Calculate(point.X, point.Y);
    }

    public double Calculate(double x, double y)
    {
        return _xFunction(x) * _yFunction(y);
    }
}