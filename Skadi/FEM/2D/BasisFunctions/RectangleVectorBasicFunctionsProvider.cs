using Skadi.FEM.Core.BasisFunctions;
using Skadi.FEM.Core.Geometry;
using Skadi.Geometry._2D;
using Skadi.Geometry._3D;

namespace Skadi.FEM._2D.BasisFunctions;

public class RectangleVectorBasicFunctionsProvider : IEdgeVectorBasisFunctionsProvider<IEdgeElement, Vector2D>
{
    private readonly IPointsCollection<Vector2D> _nodes;

    public RectangleVectorBasicFunctionsProvider(IPointsCollection<Vector2D> nodes)
    {
        _nodes = nodes;
    }

    public IVectorBasicFunction<Vector2D>[] GetFunctions(IEdgeElement element)
    {
        var (xLeft, yBot) = _nodes[element.NodeIds[0]];
        var (xRight, yTop) = _nodes[element.NodeIds[3]];
        var hx = xRight - xLeft;
        var hy = yTop - yBot;

        return
        [
            new VectorBasisFunction<Vector2D>
            (
                p => new Vector2D(0, (xRight - p.X) / hx), 
                _ => new Vector3D(0, 0, -1d / hx)
            ),
            new VectorBasisFunction<Vector2D>
            (
                p => new Vector2D(0, (p.X - xLeft) / hx), 
                _ => new Vector3D(0, 0, 1d / hx)
            ),
            new VectorBasisFunction<Vector2D>
            (
                p => new Vector2D((yTop - p.Y) / hy, 0), 
                _ => new Vector3D(0, 0, 1d / hy)
            ),
            new VectorBasisFunction<Vector2D>
            (
                p => new Vector2D((p.Y - yBot) / hy, 0), 
                _ => new Vector3D(0, 0, -1d / hy)
            ),
        ];
    }
}