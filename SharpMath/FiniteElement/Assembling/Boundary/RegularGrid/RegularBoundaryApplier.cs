using System.Linq.Expressions;
using SharpMath.FEM.Core;
using SharpMath.FEM.Geometry;
using SharpMath.FEM.Geometry._2D.Quad;
using SharpMath.FiniteElement.Core.Assembling.Boundary;
using SharpMath.FiniteElement.Core.Assembling.Boundary.First;
using SharpMath.FiniteElement.Core.Assembling.Boundary.Second;
using SharpMath.FiniteElement.Core.Assembling.Params;
using SharpMath.Geometry._2D;
using SharpMath.Matrices.Sparse;

namespace SharpMath.FiniteElement.Assembling.Boundary.RegularGrid;

public class RegularBoundaryApplier<TMatrix> : IRegularBoundaryApplier<TMatrix>
{
    private readonly IFirstBoundaryApplier<TMatrix> _firstBoundaryApplier;
    private readonly ISecondBoundaryApplier<TMatrix> _secondBoundaryApplier;
    private readonly IExpressionProvider _expressionProvider;
    private readonly IPointsCollection<Point2D> _nodes;
    private readonly int[,] _nodeStartIndexes;
    private readonly int _xNodesPerLayer;

    public RegularBoundaryApplier(
        IFirstBoundaryApplier<TMatrix> firstBoundaryApplier,
        ISecondBoundaryApplier<TMatrix> secondBoundaryApplier,
        IExpressionProvider expressionProvider,
        IPointsCollection<Point2D> nodes,
        RegularGridDefinition gridDefinition
    )
    {
        _firstBoundaryApplier = firstBoundaryApplier;
        _secondBoundaryApplier = secondBoundaryApplier;
        _expressionProvider = expressionProvider;
        _nodes = nodes;
        _nodeStartIndexes = new int
        [
            gridDefinition.ControlPoints.GetLength(0),
            gridDefinition.ControlPoints.GetLength(1)
        ];

        var xSplitters = gridDefinition.XSplitters;
        var ySplitters = gridDefinition.YSplitters;

        _nodeStartIndexes[0, 0] = 0;
        var xStepsPrefixSum = 0;
        for (var j = 0; j < xSplitters.Length; j++)
        {
            xStepsPrefixSum += xSplitters[j].Steps;
            _nodeStartIndexes[0, j + 1] = xStepsPrefixSum;
        }

        _xNodesPerLayer = xStepsPrefixSum + 1;
        for (var i = 0; i < ySplitters.Length; i++)
        {
            var yCurrentIntervalSteps = ySplitters[i].Steps;
            for (var j = 0; j < xSplitters.Length + 1; j++)
            {
                _nodeStartIndexes[i + 1, j] = _nodeStartIndexes[i, j] + _xNodesPerLayer * yCurrentIntervalSteps;
            }
        }
    }

    public void Apply(Equation<TMatrix> matrix, RegularBoundaryCondition condition)
    {
        if (condition.Type is BoundaryConditionType.None)
        {
            return;
        }

        if (condition.Type == BoundaryConditionType.First)
        {
            var expression = (Expression<Func<Point2D, double>>) _expressionProvider.GetExpression(condition.ExpressionId);
            var func = expression.Compile();

            foreach (var nodeId in EnumerateNodes(condition))
            {
                var node = _nodes[nodeId];
                var value = func(node);
                _firstBoundaryApplier.Apply(matrix, new FirstCondition(nodeId, value));
            }

            _firstBoundaryApplier.Apply(matrix, new FirstCondition());
        }
        else if (condition.Type == BoundaryConditionType.Second)
        {
            var expression = (Expression<Func<Point2D, double>>) _expressionProvider.GetExpression(condition.ExpressionId);
            var func = expression.Compile();
            var thetta = new double[2];
            foreach (var edge in EnumerateEdges(condition))
            {
                thetta[0] = func(_nodes[edge.Begin]);
                thetta[1] = func(_nodes[edge.End]);
                _secondBoundaryApplier.Apply(matrix, new SecondBoundary(edge, thetta));
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

    private IEnumerable<int> EnumerateNodes(RegularBoundaryCondition condition)
    {
        var (startNode, endNode, step) = condition.Orientation switch
        {
            Orientation.Horizontal => (
                _nodeStartIndexes[condition.BottomBoundId, condition.LeftBoundId],
                _nodeStartIndexes[condition.BottomBoundId, condition.RightBoundId],
                1
            ),
            Orientation.Vertical => (
                _nodeStartIndexes[condition.BottomBoundId, condition.LeftBoundId],
                _nodeStartIndexes[condition.TopBoundId, condition.LeftBoundId],
                _xNodesPerLayer
            ),
            _ => throw new ArgumentOutOfRangeException(nameof(condition))
        };

        for (var i = startNode; i <= endNode; i += step)
        {
            yield return i;
        }
    }

    private IEnumerable<Edge> EnumerateEdges(RegularBoundaryCondition condition)
    {
        using var nodesEnumerator = EnumerateNodes(condition).GetEnumerator();

        if (!nodesEnumerator.MoveNext())
        {
            yield break;
        }

        var previousNode = nodesEnumerator.Current;

        while (nodesEnumerator.MoveNext())
        {
            var currentNode = nodesEnumerator.Current;
            yield return new Edge(previousNode, currentNode);
            previousNode = currentNode;
        }
    }
}