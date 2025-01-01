using Skadi.FEM.Core;
using Skadi.FiniteElement.Core.Assembling;
using Skadi.Matrices.Sparse;

namespace Skadi.FiniteElement.Assembling;

public class SymmetricMatrixPortraitBuilder : IMatrixPortraitBuilder<SymmetricSparseMatrix, IElement>
{
    private List<SortedSet<int>> _adjacencyList = null!;

    public SymmetricSparseMatrix Build(IEnumerable<IElement> elements, int nodesCount)
    {
        BuildAdjacencyList(elements, nodesCount);

        var amount = 0;
        var buf = _adjacencyList
            .Select(nodeSet => amount += nodeSet.Count)
            .ToList();
        buf.Insert(0, 0);

        var rowsIndexes = buf.ToArray();
        var columnsIndexes = _adjacencyList
            .SelectMany(nodeList => nodeList)
            .ToArray();

        return new SymmetricSparseMatrix(rowsIndexes, columnsIndexes);
    }

    private void BuildAdjacencyList(IEnumerable<IElement> elements, int nodesCount)
    {
        _adjacencyList = new List<SortedSet<int>>(nodesCount);

        for (var i = 0; i < nodesCount; i++)
        {
            _adjacencyList.Add([]);
        }

        foreach (var element in elements)
        {
            var nodesIndexes = element.NodeIds;

            foreach (var currentNode in nodesIndexes)
            {
                foreach (var nodeIndex in nodesIndexes)
                {
                    if (currentNode > nodeIndex)
                        _adjacencyList[currentNode].Add(nodeIndex);
                }
            }
        }
    }
}