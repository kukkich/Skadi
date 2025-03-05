using Skadi.FEM.Core.Geometry._2D;
using Skadi.FEM.Core.Geometry._2D.Quad;
using Skadi.FEM.Core.Geometry.Edges;

namespace Skadi.FEM.Assembling.Boundary.RegularGrid;

public class BoundIndexesEvaluator : IBoundIndexesEvaluator
{
    private readonly int[,] _nodeStartIndexes;
    private readonly int _xNodesPerLayer;
    
    public BoundIndexesEvaluator(RegularGridDefinition gridDefinition)
    {
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
    
    public IEnumerable<int> EnumerateNodes(RegularBoundaryCondition condition)
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

    public IEnumerable<Edge> EnumerateEdges(RegularBoundaryCondition condition)
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