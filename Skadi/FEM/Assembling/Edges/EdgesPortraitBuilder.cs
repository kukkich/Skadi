﻿using Skadi.FEM.Core.Geometry;
using Skadi.FEM.Core.Geometry.Edges;

namespace Skadi.FEM.Assembling.Edges;

public class EdgesPortraitBuilder(IElementEdgeResolver edgeResolver) : IEdgesPortraitBuilder
{
    public EdgesPortrait Build<T>(IEnumerable<T> elements, int nodesCount)
        where T : IElement
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

    private List<SortedSet<int>> BuildAdjacencyList<T>(IEnumerable<T> elements, int nodesCount)
        where T : IElement
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
                    if (currentNode > nodeIndex &&
                        edgeResolver.HasEdgeWithNode(element, new Edge(currentNode, nodeIndex)))
                        adjacencyList[currentNode].Add(nodeIndex);
                }
            }
        }

        return adjacencyList;
    }
}