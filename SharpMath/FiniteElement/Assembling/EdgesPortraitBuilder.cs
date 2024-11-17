using SharpMath.FEM.Core;
using SharpMath.FEM.Geometry;

namespace SharpMath.FiniteElement.Assembling;

public class EdgesPortraitBuilder : IEdgesPortraitBuilder
{
    private readonly IElementEdgeResolver _edgeResolver;

    public EdgesPortraitBuilder(IElementEdgeResolver edgeResolver)
    {
        _edgeResolver = edgeResolver;
    }
    
    public EdgesPortrait Build(IEnumerable<IElement> elements, int nodesCount)
    {
        var adjacencyList = BuildAdjacencyList(elements, nodesCount);

        var amount = 0;
        var rowsIndexes = adjacencyList
            .Select(nodeSet => amount += nodeSet.Count)
            .ToList();
        rowsIndexes.Insert(0, 0);
        
        var columnsIndexes = adjacencyList.SelectMany(nodeList => nodeList).ToArray();

        return new EdgesPortrait
        {
            ColumnIndexes = columnsIndexes,
            RowIndexes = rowsIndexes.ToArray()
        };
    }

    private List<SortedSet<int>> BuildAdjacencyList(IEnumerable<IElement> elements, int nodesCount)
    {
        var adjacencyList = new List<SortedSet<int>>(nodesCount);

        for (var i = 0; i < nodesCount; i++)
        {
            adjacencyList.Add([]);
        }

        foreach (var element in elements)
        {
            var nodesIndexes = element.NodeIds;

            foreach (var currentNode in nodesIndexes)
            {
                foreach (var nodeIndex in nodesIndexes)
                {
                    if (currentNode > nodeIndex && _edgeResolver.HasEdgeWithNode(element, currentNode, nodeIndex))
                        adjacencyList[currentNode].Add(nodeIndex);
                }
            }
        }

        return adjacencyList;
    }
}