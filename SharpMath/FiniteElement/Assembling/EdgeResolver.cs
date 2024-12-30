using SharpMath.FEM.Core;
using SharpMath.FEM.Geometry;

namespace SharpMath.FiniteElement.Assembling;

public class EdgeResolver : IEdgeResolver
{
    private readonly IReadOnlyList<IElement> _elements;
    private readonly IElementEdgeResolver _elementEdgeResolver;
    private readonly int[] _columnIndexes;
    private readonly int[] _rowIndexes;

    public EdgeResolver(EdgesPortrait portrait, IReadOnlyList<IElement> elements, IElementEdgeResolver elementEdgeResolver)
    {
        _elements = elements;
        _elementEdgeResolver = elementEdgeResolver;
        _columnIndexes = portrait.ColumnIndexes;
        _rowIndexes = portrait.RowIndexes;
    }
    
    public int[] GetElementEdges(int elementId)
    {
        var element = _elements[elementId];
        var edgesNodes = _elementEdgeResolver.GetEdgedNodes(element);
        var edges = edgesNodes.Select(tuple =>
            {
                var (minNode, maxNode) = tuple;
                return GetEdgeId(minNode, maxNode);
            }).Order()
            .ToArray();

        return edges;
    }

    public int GetEdgeId(int node1, int node2)
    {
        var (minNodeId, maxNodeId) = (int.Min(node1, node2), int.Max(node1, node2));
        if (_rowIndexes.Length <= maxNodeId)
        {
            throw new ArgumentOutOfRangeException($"No node with id = {maxNodeId}");
        }
        if (_rowIndexes.Length <= maxNodeId + 1)
        {
            throw new ArgumentOutOfRangeException($"No node with id = {maxNodeId + 1}");
        }
        
        var leftColumnIndexesBound = _rowIndexes[maxNodeId];
        var rightColumnIndexesBound = _rowIndexes[maxNodeId + 1];

        for (var i = leftColumnIndexesBound; i < rightColumnIndexesBound; i++)
        {
            if (_columnIndexes[i] != minNodeId)
            {
                continue;
            }
            return i;
        }
        
        throw new InvalidOperationException($"No edge for nodes ({minNodeId}, {maxNodeId})");
    }

    public bool TryGetEdge(int node1, int node2, out int? edge)
    {
        edge = null;
        var (minNodeId, maxNodeId) = (int.Min(node1, node2), int.Max(node1, node2));
        if (_rowIndexes.Length <= maxNodeId + 1)
        {
            return false;
        }
        
        var leftColumnIndexesBound = _rowIndexes[maxNodeId];
        var rightColumnIndexesBound = _rowIndexes[maxNodeId + 1];

        for (var i = leftColumnIndexesBound; i < rightColumnIndexesBound; i++)
        {
            if (_columnIndexes[i] != minNodeId)
            {
                continue;
            }
            edge = i;
            return true;
        }

        return false;
    }

    public (int minNode, int maxNode) GetNodesByEdge(int edgeId)
    {
        if (edgeId < 0 || edgeId >= _columnIndexes.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(edgeId));
        }
        var minNode = _columnIndexes[edgeId];
        var index = Array.BinarySearch(_rowIndexes, edgeId);
        index = index switch
        {
            0 => 1,
            < 0 => ~index,
            _ => index
        };
        
        if (index >= _rowIndexes.Length)
        {
            throw new InvalidOperationException($"Cant find nodes for edge {edgeId}");
        }
        
        return _rowIndexes[index] == edgeId 
            ? (minNode, index) 
            : (minNode, index - 1);
    }

    public int[] GetElementsByEdge(int edgeId)
    {
        var (node1, node2) = GetNodesByEdge(edgeId);
        var result = new List<int>(2);

        for (var i = 0; i < _elements.Count; i++)
        {
            var element = _elements[i];
            if (!(element.NodeIds.Contains(node1) && element.NodeIds.Contains(node2)))
            {
                continue;
            }
            result.Add(i);
        }
        return result.ToArray();
    }
}