using Skadi.FEM.Core.BasisFunctions;
using Skadi.Geometry._2D;

namespace Skadi.FEM._2D.BasisFunctions;

public class BasisFunction2D : IBasisFunction<Vector2D>
{
    private readonly Func<double, double> _xFunction;
    private readonly Func<double, double> _yFunction;

    public BasisFunction2D(Func<double, double> xFunction, Func<double, double> yFunction)
    {
        _xFunction = xFunction;
        _yFunction = yFunction;
    }

    public double Evaluate(Vector2D vector)
    {
        return Calculate(vector.X, vector.Y);
    }

    public double Calculate(double x, double y)
    {
        return _xFunction(x) * _yFunction(y);
    }
}