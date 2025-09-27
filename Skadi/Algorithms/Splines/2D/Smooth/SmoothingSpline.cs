using Skadi.FEM.Core.BasisFunctions;
using Skadi.FEM.Core.Geometry;
using Skadi.Geometry._2D;
using Skadi.LinearAlgebra.Vectors;

namespace Skadi.Algorithms.Splines._2D.Smooth;

public class SmoothingSpline(
    IBasisFunctionsProvider<IElement, Vector2D> basisFunctionsProvider,
    Grid<Vector2D, IElement> grid,
    Vector qValues)
    : ISpline<Vector2D>
{
    public double Calculate(Vector2D vector)
    {
        var element = grid.Elements.First(e => ElementHas(e, vector));

        var basisFunctions = basisFunctionsProvider.GetFunctions(element);

        var sum = 0d;

        for (var i = 0; i < element.NodeIds.Count; i++)
        {
            for (var j = 0; j < 4; j++)
            {
                sum += qValues[element.NodeIds[i] * 4 + j] * basisFunctions[i * 4 + j].Evaluate(vector);
            }
        }

        return sum;
    }

    private bool ElementHas(IElement element, Vector2D node)
    {
        var leftBottom = grid.Nodes[element.NodeIds[0]];
        var rightTop = grid.Nodes[element.NodeIds[^1]];

        return leftBottom.X <= node.X && node.X <= rightTop.X && 
               leftBottom.Y <= node.Y && node.Y <= rightTop.Y;
    }
}