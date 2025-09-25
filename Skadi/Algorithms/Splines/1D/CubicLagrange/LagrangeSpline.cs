using Skadi.FEM._1D.BasisFunctions;
using Skadi.FEM.Core.Geometry;
using Skadi.LinearAlgebra.Vectors;

namespace Skadi.Algorithms.Splines._1D.CubicLagrange;

public class LagrangeSpline : ISpline<double>
{
    private readonly LagrangeCubicFunction1DProvider _basisFunctionsProvider;
    private readonly Grid<double, IElement> _grid;
    private readonly Vector _weights;

    public LagrangeSpline(LagrangeCubicFunction1DProvider basisFunctionsProvider, 
        Grid<double,IElement> grid, 
        Vector weights
        )
    {
        _basisFunctionsProvider = basisFunctionsProvider;
        _grid = grid;
        _weights = weights;
    }
    public double Calculate(double point)
    {
        var element = _grid.Elements.First(e => ElementHas(e, point));
        var basisFunctions = _basisFunctionsProvider.GetFunctions(element);

        var sum = 0d;

        for (var i = 0; i < 4; i++)
        {
            var weightId = element.NodeIds[0] * 3 + i;
            sum += _weights[weightId] * basisFunctions[i].Evaluate(point);
        }

        return sum;
    }
    
    private bool ElementHas(IElement element, double node)
    {
        var left = _grid.Nodes[element.NodeIds[0]];
        var right = _grid.Nodes[element.NodeIds[1]];
        
        return left <= node && node <= right;
    }
}