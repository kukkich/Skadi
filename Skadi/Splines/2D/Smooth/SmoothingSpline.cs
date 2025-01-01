using Skadi.FEM.Core;
using Skadi.Geometry._2D;
using Skadi.FiniteElement.Core.BasisFunctions;
using Skadi.Vectors;

namespace Skadi.Splines._2D.Smooth;

public class SmoothingSpline : ISpline<Point2D>
{
    private readonly IBasisFunctionsProvider<IElement, Point2D> _basisFunctionsProvider;
    private readonly Grid<Point2D, IElement> _grid;
    private readonly Vector _qValues;

    public SmoothingSpline(
        IBasisFunctionsProvider<IElement, Point2D> basisFunctionsProvider,
        Grid<Point2D, IElement> grid, 
        Vector qValues
    )
    {
        _basisFunctionsProvider = basisFunctionsProvider;
        _grid = grid;
        _qValues = qValues;
    }

    public double Calculate(Point2D point)
    {
        var element = _grid.Elements.First(e => ElementHas(e, point));

        var basisFunctions = _basisFunctionsProvider.GetFunctions(element);

        var sum = 0d;

        for (var i = 0; i < element.NodeIds.Count; i++)
        {
            for (var j = 0; j < 4; j++)
            {
                sum += _qValues[element.NodeIds[i] * 4 + j] * basisFunctions[i * 4 + j].Evaluate(point);
            }
        }

        return sum;
    }

    private bool ElementHas(IElement element, Point2D node)
    {
        var leftBottom = _grid.Nodes[element.NodeIds[0]];
        var rightTop = _grid.Nodes[element.NodeIds[^1]];

        return leftBottom.X <= node.X && node.X <= rightTop.X && 
               leftBottom.Y <= node.Y && node.Y <= rightTop.Y;
    }
}