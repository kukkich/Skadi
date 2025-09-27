using Skadi.FEM._1D.BasisFunctions;
using Skadi.FEM.Core.Geometry;
using Skadi.LinearAlgebra.Vectors;

namespace Skadi.Algorithms.Splines._1D.CubicLagrange;

public class LagrangeSpline
(
    LagrangeCubicFunction1DProvider basisFunctionsProvider,
    Grid<double, IElement> grid,
    Vector weights
) : ISpline<double>
{
    public double Calculate(double point)
    {
        var element = grid.Elements.First(e => ElementHas(e, point));
        var basisFunctions = basisFunctionsProvider.GetFunctions(element);

        var sum = 0d;

        for (var i = 0; i < 4; i++)
        {
            var weightId = element.NodeIds[0] * 3 + i;
            sum += weights[weightId] * basisFunctions[i].Evaluate(point);
        }

        return sum;
    }
    
    private bool ElementHas(IElement element, double node)
    {
        var left = grid.Nodes[element.NodeIds[0]];
        var right = grid.Nodes[element.NodeIds[1]];
        
        return left <= node && node <= right;
    }
}