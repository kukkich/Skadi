using Skadi.FEM.Core.BasisFunctions;

namespace Skadi.FEM._1D.BasisFunctions;

public class ExplicitFunction1D(Func<double, double> function) : IBasisFunction<double>
{
    public double Evaluate(double point) => function(point);
}