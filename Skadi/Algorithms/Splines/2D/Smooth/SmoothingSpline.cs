using Skadi.FEM.Core.BasisFunctions;
using Skadi.FEM.Core.Geometry;
using Skadi.Geometry._2D;
using Skadi.LinearAlgebra.Vectors;

namespace Skadi.Algorithms.Splines._2D.Smooth;

public class SmoothingSpline : ISpline<Vector2D>
{
    private readonly IBasisFunctionsProvider<IElement, Vector2D> _basisFunctionsProvider;
    private readonly Grid<Vector2D, IElement> _grid;
    private readonly Vector _qValues;

    public SmoothingSpline(
        IBasisFunctionsProvider<IElement, Vector2D> basisFunctionsProvider,
        Grid<Vector2D, IElement> grid, 
        Vector qValues
    )
    {
        _basisFunctionsProvider = basisFunctionsProvider;
        _grid = grid;
        _qValues = qValues;
    }

    public double Calculate(Vector2D vector)
    {
        var element = _grid.Elements.First(e => ElementHas(e, vector));

        var basisFunctions = _basisFunctionsProvider.GetFunctions(element);

        var sum = 0d;

        for (var i = 0; i < element.NodeIds.Count; i++)
        {
            for (var j = 0; j < 4; j++)
            {
                sum += _qValues[element.NodeIds[i] * 4 + j] * basisFunctions[i * 4 + j].Evaluate(vector);
            }
        }

        return sum;
    }

    private bool ElementHas(IElement element, Vector2D node)
    {
        var leftBottom = _grid.Nodes[element.NodeIds[0]];
        var rightTop = _grid.Nodes[element.NodeIds[^1]];

        return leftBottom.X <= node.X && node.X <= rightTop.X && 
               leftBottom.Y <= node.Y && node.Y <= rightTop.Y;
    }
}