using Skadi.FEM.Core.Geometry;
using Skadi.FEM.Core.Geometry.Edges;

namespace Skadi.FEM.Assembling;

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
    
    public int[] GetEdgeIdsByElement(int elementId)
    {
        var element = _elements[elementId];
        var edgesNodes = _elementEdgeResolver.GetEdgedNodes(element);
        var edges = edgesNodes
            .Select(GetEdgeId)
            .Order()
            .ToArray();

        return edges;
    }

    public int GetEdgeId(Edge edge)
    {
        if (_rowIndexes.Length <= edge.End)
        {
            throw new ArgumentOutOfRangeException($"No node with id = {edge.End}");
        }
        if (_rowIndexes.Length <= edge.End + 1)
        {
            throw new ArgumentOutOfRangeException($"No node with id = {edge.End + 1}");
        }
        
        var leftColumnIndexesBound = _rowIndexes[edge.End];
        var rightColumnIndexesBound = _rowIndexes[edge.End + 1];

        for (var i = leftColumnIndexesBound; i < rightColumnIndexesBound; i++)
        {
            if (_columnIndexes[i] != edge.Begin)
            {
                continue;
            }
            return i;
        }
        
        throw new InvalidOperationException($"No edge for edge {edge}");
    }

    public bool TryGetEdgeId(Edge edge, out int? edgeId)
    {
        edgeId = null;
        if (_rowIndexes.Length <= edge.End + 1)
        {
            return false;
        }
        
        var leftColumnIndexesBound = _rowIndexes[edge.End];
        var rightColumnIndexesBound = _rowIndexes[edge.End + 1];

        for (var i = leftColumnIndexesBound; i < rightColumnIndexesBound; i++)
        {
            if (_columnIndexes[i] != edge.Begin)
            {
                continue;
            }
            edgeId = i;
            return true;
        }

        return false;
    }

    public Edge GetEdgeById(int edgeId)
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
            ? new Edge(minNode, index) 
            : new Edge(minNode, index - 1);
    }

    public int[] GetElementsByEdgeId(int edgeId)
    {
        var edge = GetEdgeById(edgeId);
        var result = new List<int>(2);

        for (var i = 0; i < _elements.Count; i++)
        {
            var element = _elements[i];
            if (!(element.NodeIds.Contains(edge.Begin) && element.NodeIds.Contains(edge.Begin)))
            {
                continue;
            }
            result.Add(i);
        }
        return result.ToArray();
    }
}