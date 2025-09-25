using System.Linq.Expressions;
using Skadi.EquationsSystem;
using Skadi.FEM.Core.Assembling.Boundary;
using Skadi.FEM.Core.Assembling.Boundary.First;
using Skadi.FEM.Core.Assembling.Params;
using Skadi.FEM.Core.Geometry;
using Skadi.FEM.Core.Geometry.Edges;
using Skadi.Geometry._2D;

namespace Skadi.FEM.Assembling.Boundary.RegularGrid.Vectors;

public class VectorRegularBoundaryApplier<TMatrix> : IRegularBoundaryApplier<TMatrix>
{
    private readonly IFirstBoundaryApplier<TMatrix> _firstBoundaryApplier;
    private readonly IExpressionProvider _expressionProvider;
    private readonly IPointsCollection<Vector2D> _nodes;
    private readonly IBoundIndexesEvaluator _boundIndexesEvaluator;
    private readonly IEdgeResolver _edgeResolver;

    public VectorRegularBoundaryApplier
    (
        IFirstBoundaryApplier<TMatrix> firstBoundaryApplier,
        IExpressionProvider expressionProvider,
        IPointsCollection<Vector2D> nodes,
        IBoundIndexesEvaluator boundIndexesEvaluator,
        IEdgeResolver edgeResolver
    )
    {
        _firstBoundaryApplier = firstBoundaryApplier;
        _expressionProvider = expressionProvider;
        _nodes = nodes;
        _boundIndexesEvaluator = boundIndexesEvaluator;
        _edgeResolver = edgeResolver;
    }

    public void Apply(Equation<TMatrix> equation, RegularBoundaryCondition condition)
    {
        if (condition.Type is BoundaryConditionType.None)
        {
            return;
        }

        if (condition.Type is BoundaryConditionType.First)
        {
            var expression = (Expression<Func<Vector2D, double>>) _expressionProvider.GetExpression(condition.ExpressionId);
            var func = expression.Compile();
            
            foreach (var edge in _boundIndexesEvaluator.EnumerateEdges(condition))
            {
                var edgeId = _edgeResolver.GetEdgeId(edge);
                var value = func((_nodes[edge.Begin] + _nodes[edge.End])/ 2);
                _firstBoundaryApplier.Apply(equation, new FirstCondition(edgeId, value));
            }
        }
        else if (condition.Type == BoundaryConditionType.Second)
        {
            throw new NotImplementedException();
            var expression = (Expression<Func<Vector2D, double>>) _expressionProvider.GetExpression(condition.ExpressionId);
            var func = expression.Compile();
            foreach (var edge in _boundIndexesEvaluator.EnumerateEdges(condition))
            {
                var thetta = func((_nodes[edge.Begin] + _nodes[edge.End])/ 2);
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