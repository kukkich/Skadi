using Skadi.FEM._1D.BasisFunctions;
using Skadi.FEM.Core.Geometry;
using Skadi.Geometry._2D;
using Skadi.Geometry.Shapes;
using Skadi.Geometry.Shapes.Primitives;
using Skadi.LinearAlgebra.Vectors;

namespace Skadi.Algorithms.Splines._1D.CubicLagrange;

public class LagrangeSpline : ISpline<double>, IParametricCurve2D
{
    private readonly LagrangeCubicFunction1DProvider _basisFunctionsProvider;
    private readonly Grid<double, IElement> _grid;
    private readonly Vector _weights;

    public Vector2D Start { get; } 
    public Vector2D End { get; }

    public LagrangeSpline
    (
        LagrangeCubicFunction1DProvider basisFunctionsProvider,
        Grid<double, IElement> grid,
        Vector weights
    )
    {
        _basisFunctionsProvider = basisFunctionsProvider;
        _grid = grid;
        _weights = weights;
        
        var left = grid.Nodes[grid.Elements[0].NodeIds[0]];
        var right = grid.Nodes[grid.Elements[^1].NodeIds[1]];
        
        Start = new Vector2D(left, Calculate(left));
        End = new Vector2D(right, Calculate(right));
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
    
    public Vector2D GetByParameter(CurveParameter t)
    {
        var x = Start.X + (End.X - Start.X) * t;
        var y = Calculate(x);
        return new Vector2D(x, y);
    }
}