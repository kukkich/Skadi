using Skadi.FEM.Core.Geometry.Edges;

namespace Skadi.FEM.Core.Geometry._2D.Quad;

public class QuadElementEdgeResolver : IElementEdgeResolver
{
    public bool HasEdgeWithNode(IElement element, int node1, int node2)
    {
        return !Equal(element.NodeIds[0], element.NodeIds[3], node1, node2)
               && !Equal(element.NodeIds[1], element.NodeIds[2], node1, node2);
    }

    public (int MinNode, int MaxNode)[] GetEdgedNodes(IElement element)
    {
        var result = new (int MinNode, int MaxNode)[4];
        result[0] = (element.NodeIds[0], element.NodeIds[1]);
        result[1] = (element.NodeIds[1], element.NodeIds[3]);
        result[2] = (element.NodeIds[0], element.NodeIds[2]);
        result[3] = (element.NodeIds[2], element.NodeIds[3]);

        return result;
    }

    private static bool Equal(int elementIndex1, int elementIndex2, int node1, int node2)
    {
        return elementIndex1 == node1 && elementIndex2 == node2
               || elementIndex1 == node2 && elementIndex2 == node1;
    }
}