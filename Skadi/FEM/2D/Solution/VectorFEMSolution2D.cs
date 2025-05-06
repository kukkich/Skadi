using Skadi.FEM.Core;
using Skadi.FEM.Core.BasisFunctions;
using Skadi.FEM.Core.Geometry;
using Skadi.Geometry._2D;
using Skadi.Vectors;

namespace Skadi.FEM._2D.Solution;

// ReSharper disable once InconsistentNaming
public class VectorFEMSolution2D : IVectorFEMSolution<Vector2D>
{
    public IReadonlyVector<double> Weights { get; }
    
    private const double Epsilon = 1e-10;
    private readonly IEdgeVectorBasisFunctionsProvider<IEdgeElement, Vector2D> _basisFunctionsProvider;
    private readonly Grid<Vector2D, IEdgeElement> _grid;

    public VectorFEMSolution2D
    (
        IEdgeVectorBasisFunctionsProvider<IEdgeElement, Vector2D> basisFunctionsProvider,
        Grid<Vector2D, IEdgeElement> grid,
        IReadonlyVector<double> weights
    )
    {
        _basisFunctionsProvider = basisFunctionsProvider;
        _grid = grid;
        Weights = weights;
    }
    
    public Vector2D Calculate(Vector2D point)
    {
        var element = _grid.Elements
            .First(x => ElementHas(x, point));

        Span<Vector2D> p = stackalloc Vector2D[4];
        for (var i = 0; i < 4; i++)
        {
            p[i] = _grid.Nodes[element.NodeIds[i]];
        }
        Span<Vector2D> edgeCenters = stackalloc Vector2D[4];
        edgeCenters[0] = (p[0] + p[1]) / 2;
        edgeCenters[1] = (p[0] + p[2]) / 2;
        edgeCenters[2] = (p[1] + p[3]) / 2;
        edgeCenters[3] = (p[2] + p[3]) / 2;
        
        var functions = _basisFunctionsProvider.GetFunctions(element);
        Span<Vector2D> funcValues = stackalloc Vector2D[functions.Length];
        for (var i = 0; i < functions.Length; i++)
        {
            funcValues[i] = functions[i].Evaluate(point);
        }
        
        var result = Vector2D.AdditiveIdentity;
        for (var i = 0; i < functions.Length; i++)
        {
            var weightId = element.NodeIds[i];
            var weight = Weights[weightId];
            result += funcValues[i] * weight;
        }
        
        return result;
    }
    
    private bool ElementHas(IElement element, Vector2D vector)
    {
        var nodes = element.NodeIds
            .Select(nodeId => _grid.Nodes[nodeId])
            .ToArray();
        var leftBottom = nodes[0];
        var rightBottom = nodes[1];
        var leftTop = nodes[2];
        var rightTop = nodes[3];

        return IsPointInTriangle(vector, leftBottom, rightBottom, leftTop) ||
               IsPointInTriangle(vector, leftTop, rightBottom, rightTop);

        bool IsPointInTriangle(Vector2D p, Vector2D a, Vector2D b, Vector2D c)
        {
            // Векторные произведения для всех трёх рёбер треугольника
            var v1 = (b - a).X * (p.Y - a.Y) - (b - a).Y * (p.X - a.X);
            var v2 = (c - b).X * (p.Y - b.Y) - (c - b).Y * (p.X - b.X);
            var v3 = (a - c).X * (p.Y - c.Y) - (a - c).Y * (p.X - c.X);

            // Проверяем, что все знаки одинаковы
            return (v1 >= 0 && v2 >= 0 && v3 >= 0) || (v1 <= 0 && v2 <= 0 && v3 <= 0);
        }
    }
}