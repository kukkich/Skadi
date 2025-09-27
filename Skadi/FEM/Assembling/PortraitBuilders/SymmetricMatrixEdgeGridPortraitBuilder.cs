using Skadi.FEM.Core.Assembling;
using Skadi.FEM.Core.Geometry;
using Skadi.LinearAlgebra.Matrices.Sparse;

namespace Skadi.FEM.Assembling.PortraitBuilders;

public class SymmetricMatrixEdgeGridPortraitBuilder : IMatrixPortraitBuilder<SymmetricRowSparseMatrix, IEdgeElement>
{
    private List<SortedSet<int>> _adjacencyList = null!;

    public SymmetricRowSparseMatrix Build(IEnumerable<IEdgeElement> elements, int edgesCunt)
    {
        BuildAdjacencyList(elements, edgesCunt);

        var amount = 0;
        var buf = _adjacencyList
            .Select(nodeSet => amount += nodeSet.Count)
            .ToList();
        buf.Insert(0, 0);

        var rowsIndexes = buf.ToArray();
        var columnsIndexes = _adjacencyList
            .SelectMany(nodeList => nodeList)
            .ToArray();

        return new SymmetricRowSparseMatrix(rowsIndexes, columnsIndexes);
    }

    private void BuildAdjacencyList(IEnumerable<IEdgeElement> elements, int edgesCunt)
    {
        _adjacencyList = new List<SortedSet<int>>(edgesCunt);

        for (var i = 0; i < edgesCunt; i++)
        {
            _adjacencyList.Add([]);
        }

        foreach (var element in elements)
        {
            var edges = element.EdgeIds;

            foreach (var currentEdge in edges)
            {
                foreach (var edgeIndex in edges)
                {
                    if (currentEdge > edgeIndex)
                        _adjacencyList[currentEdge].Add(edgeIndex);
                }
            }
        }
    }
}