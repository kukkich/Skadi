using System.Linq.Expressions;
using System.Numerics;
using Skadi.FEM.Core.Assembling.Boundary;
using Skadi.FEM.Core.Assembling.Boundary.First;
using Skadi.FEM.Core.Assembling.Boundary.Second;
using Skadi.FEM.Core.Assembling.Params;
using Skadi.FEM.Core.Geometry;
using Skadi.FEM.Core.Geometry.Edges;
using Skadi.Geometry._2D;

namespace Skadi.FEM.Assembling.Boundary.RegularGrid.Harmonic;

public class HarmonicRegularBoundaryApplier<TMatrix> : IRegularBoundaryApplier<TMatrix>
{
    private readonly IFirstBoundaryApplier<TMatrix> _firstBoundaryApplier;
    private readonly ISecondBoundaryApplier<TMatrix> _secondBoundaryApplier;
    private readonly IExpressionProvider _expressionProvider;
    private readonly IPointsCollection<Vector2D> _nodes;
    private readonly IBoundIndexesEvaluator _boundIndexesEvaluator;

    public HarmonicRegularBoundaryApplier(
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
            var expression = (Expression<Func<Vector2D, Complex>>) _expressionProvider.GetExpression(condition.ExpressionId);
            var func = expression.Compile();

            foreach (var nodeId in _boundIndexesEvaluator.EnumerateNodes(condition))
            {
                var node = _nodes[nodeId];
                var value = func(node);
                _firstBoundaryApplier.Apply(equation, new FirstCondition(nodeId * 2, value.Real));
                _firstBoundaryApplier.Apply(equation, new FirstCondition(nodeId * 2 + 1, value.Imaginary));
            }
        }
        else if (condition.Type == BoundaryConditionType.Second)
        {
            var expression = (Expression<Func<Vector2D, Complex>>) _expressionProvider.GetExpression(condition.ExpressionId);
            var func = expression.Compile();
            var thetta = new double[2];
            foreach (var edge in _boundIndexesEvaluator.EnumerateEdges(condition))
            {
                thetta[0] = func(_nodes[edge.Begin]).Real;
                thetta[1] = func(_nodes[edge.End]).Real;
                var realEdge = new Edge(edge.Begin * 2, edge.End * 2);
                _secondBoundaryApplier.Apply(equation, new SecondBoundary(realEdge, thetta));
                
                var imaginaryEdge = new Edge(edge.Begin * 2 + 1, edge.End * 2 + 1);
                thetta[0] = func(_nodes[edge.Begin]).Imaginary;
                thetta[1] = func(_nodes[edge.End]).Imaginary;
                _secondBoundaryApplier.Apply(equation, new SecondBoundary(imaginaryEdge, thetta));
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