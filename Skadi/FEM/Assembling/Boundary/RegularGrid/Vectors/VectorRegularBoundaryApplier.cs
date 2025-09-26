using System.Linq.Expressions;
using Skadi.EquationsSystem;
using Skadi.FEM.Core.Assembling.Boundary;
using Skadi.FEM.Core.Assembling.Boundary.First;
using Skadi.FEM.Core.Assembling.Params;
using Skadi.FEM.Core.Geometry;
using Skadi.FEM.Core.Geometry.Edges;
using Skadi.Geometry._2D;

namespace Skadi.FEM.Assembling.Boundary.RegularGrid.Vectors;

public class VectorRegularBoundaryApplier<TMatrix>
(
    IFirstBoundaryApplier<TMatrix> firstBoundaryApplier,
    IExpressionProvider expressionProvider,
    IPointsCollection<Vector2D> nodes,
    IBoundIndexesEvaluator boundIndexesEvaluator,
    IEdgeResolver edgeResolver
) : IRegularBoundaryApplier<TMatrix>
{
    public void Apply(Equation<TMatrix> equation, RegularBoundaryCondition condition)
    {
        if (condition.Type is BoundaryConditionType.None)
        {
            return;
        }

        if (condition.Type is BoundaryConditionType.First)
        {
            var expression = (Expression<Func<Vector2D, double>>) expressionProvider.GetExpression(condition.ExpressionId);
            var func = expression.Compile();
            
            foreach (var edge in boundIndexesEvaluator.EnumerateEdges(condition))
            {
                var edgeId = edgeResolver.GetEdgeId(edge);
                var value = func((nodes[edge.Begin] + nodes[edge.End])/ 2);
                firstBoundaryApplier.Apply(equation, new FirstCondition(edgeId, value));
            }
        }
        else if (condition.Type == BoundaryConditionType.Second)
        {
            throw new NotImplementedException();
            var expression = (Expression<Func<Vector2D, double>>) expressionProvider.GetExpression(condition.ExpressionId);
            var func = expression.Compile();
            foreach (var edge in boundIndexesEvaluator.EnumerateEdges(condition))
            {
                var thetta = func((nodes[edge.Begin] + nodes[edge.End])/ 2);
            }
        }
        else if (condition.Type == BoundaryConditionType.Third)
        {
            throw new NotImplementedException();
        }
        else
        {
            throw new ArgumentException("Unknown boundary condition type");
        }
    }
}