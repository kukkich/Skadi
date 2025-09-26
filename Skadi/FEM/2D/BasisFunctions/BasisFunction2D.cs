using Skadi.FEM.Core.BasisFunctions;
using Skadi.Geometry._2D;

namespace Skadi.FEM._2D.BasisFunctions;

public class BasisFunction2D(Func<double, double> xFunction, Func<double, double> yFunction)
    : IBasisFunction<Vector2D>
{
    public double Evaluate(Vector2D vector)
    {
        return Calculate(vector.X, vector.Y);
    }

    public double Calculate(double x, double y)
    {
        return xFunction(x) * yFunction(y);
    }
}