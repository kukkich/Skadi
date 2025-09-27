using Skadi.FEM.Core;
using Skadi.FEM.Core.BasisFunctions;
using Skadi.FEM.Core.Geometry;
using Skadi.FEM.Core.Geometry.Edges;
using Skadi.Geometry._2D;
using Skadi.LinearAlgebra.Vectors;

namespace Skadi.FEM._2D.Solution;

// ReSharper disable once InconsistentNaming
public class VectorFEMSolution2D
(
    IEdgeVectorBasisFunctionsProvider<IEdgeElement, Vector2D> basisFunctionsProvider,
    Grid<Vector2D, IEdgeElement> grid,
    IReadonlyVector<double> weights,
    IEdgeResolver edgeResolver
) : IVectorFEMSolution<Vector2D>
{
    public IReadonlyVector<double> Weights { get; } = weights;

    public Vector2D Calculate(Vector2D point)
    {
        var element = grid.Elements
            .First(x => ElementHas(x, point));

        Span<Vector2D> p = stackalloc Vector2D[4];
        for (var i = 0; i < 4; i++)
        {
            p[i] = grid.Nodes[element.NodeIds[i]];
        }
        Span<Vector2D> edgeCenters = stackalloc Vector2D[element.EdgeIds.Count];
        var j = 0;
        foreach (var edgeCenter in element.EdgeIds
                     .Select(edgeId => edgeResolver.GetEdgeById(edgeId))
                     .Select(edge => (grid.Nodes[edge.Begin] + grid.Nodes[edge.End]) / 2))
        {
            edgeCenters[j] = edgeCenter;
            j++;
        }
        
        var functions = basisFunctionsProvider.GetFunctions(element);
        Span<Vector2D> funcValues = stackalloc Vector2D[functions.Length];
        for (var i = 0; i < functions.Length; i++)
        {
            funcValues[i] = functions[i].Evaluate(point);
        }
        
        var result = Vector2D.AdditiveIdentity;
        for (var i = 0; i < functions.Length; i++)
        {
            var weightId = element.EdgeIds[i];
            var weight = Weights[weightId];
            result += funcValues[i] * weight;
        }
        
        return result;
    }
    
    private bool ElementHas(IElement element, Vector2D vector)
    {
        const double tolerance = 1e-10;

        var nodes = element.NodeIds
            .Select(nodeId => grid.Nodes[nodeId])
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
            return (v1 >= -tolerance && v2 >= -tolerance && v3 >= -tolerance) || 
                   (v1 <= tolerance && v2 <= tolerance && v3 <= tolerance);
        }
    }
}