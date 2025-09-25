using System.Linq.Expressions;
using Skadi.EquationsSystem;
using Skadi.FEM.Core.Assembling.Boundary;
using Skadi.FEM.Core.Assembling.Boundary.First;
using Skadi.FEM.Core.Assembling.Boundary.Second;
using Skadi.FEM.Core.Assembling.Params;
using Skadi.FEM.Core.Geometry;
using Skadi.FEM.Core.Geometry._2D;
using Skadi.FEM.Core.Geometry._2D.Quad;
using Skadi.FEM.Core.Geometry.Edges;
using Skadi.Geometry._2D;

namespace Skadi.FEM.Assembling.Boundary.RegularGrid;

public class RegularBoundaryApplier<TMatrix> : IRegularBoundaryApplier<TMatrix>
{
    private readonly IFirstBoundaryApplier<TMatrix> _firstBoundaryApplier;
    private readonly ISecondBoundaryApplier<TMatrix> _secondBoundaryApplier;
    private readonly IExpressionProvider _expressionProvider;
    private readonly IPointsCollection<Vector2D> _nodes;
    private readonly IBoundIndexesEvaluator _boundIndexesEvaluator;

    public RegularBoundaryApplier(
        IFirstBoundaryApplier<TMatrix> firstBoundaryApplier,
        ISecondBoundaryApplier<TMatrix> secondBoundaryApplier,
        IExpressionProvider expressionProvider,
        IPointsCollection<Vector2D> nodes,
        IBoundIndexesEvaluator boundIndexesEvaluator
    )
    {
        _firstBoundaryApplier = firstBoundaryApplier;
        _secondBoundaryApplier = secondBoundaryApplier;
        _expressionProvider = expressionProvider;
        _nodes = nodes;
        _boundIndexesEvaluator = boundIndexesEvaluator;
    }

    public void Apply(Equation<TMatrix> equation, RegularBoundaryCondition condition)
    {
        if (condition.Type is BoundaryConditionType.None)
        {
            return;
        }

        if (condition.Type == BoundaryConditionType.First)
        {
            var expression = (Expression<Func<Vector2D, double>>) _expressionProvider.GetExpression(condition.ExpressionId);
            var func = expression.Compile();

            foreach (var nodeId in _boundIndexesEvaluator.EnumerateNodes(condition))
            {
                var node = _nodes[nodeId];
                var value = func(node);
                _firstBoundaryApplier.Apply(equation, new FirstCondition(nodeId, value));
            }
        }
        else if (condition.Type == BoundaryConditionType.Second)
        {
            var expression = (Expression<Func<Vector2D, double>>) _expressionProvider.GetExpression(condition.ExpressionId);
            var func = expression.Compile();
            var thetta = new double[2];
            foreach (var edge in _boundIndexesEvaluator.EnumerateEdges(condition))
            {
                thetta[0] = func(_nodes[edge.Begin]);
                thetta[1] = func(_nodes[edge.End]);
                _secondBoundaryApplier.Apply(equation, new SecondBoundary(edge, thetta));
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