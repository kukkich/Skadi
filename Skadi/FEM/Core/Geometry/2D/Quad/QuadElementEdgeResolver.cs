using Skadi.FEM.Core.Geometry.Edges;

namespace Skadi.FEM.Core.Geometry._2D.Quad;

public class QuadElementEdgeResolver : IElementEdgeResolver
{
    public bool HasEdgeWithNode(IElement element, Edge edge)
    {
        return !Equal(element.NodeIds[0], element.NodeIds[3], edge.Begin, edge.End)
               && !Equal(element.NodeIds[1], element.NodeIds[2], edge.Begin, edge.End);
    }

    public Edge[] GetEdgedNodes(IElement element)
    {
        var result = new Edge[4];
        // result[0] = new Edge(element.NodeIds[0], element.NodeIds[1]);
        // result[1] = new Edge(element.NodeIds[1], element.NodeIds[3]);
        // result[2] = new Edge(element.NodeIds[0], element.NodeIds[2]);
        // result[3] = new Edge(element.NodeIds[2], element.NodeIds[3]);

        result[0] = new Edge(element.NodeIds[0], element.NodeIds[2]);
        result[1] = new Edge(element.NodeIds[1], element.NodeIds[3]);
        result[2] = new Edge(element.NodeIds[0], element.NodeIds[1]);
        result[3] = new Edge(element.NodeIds[2], element.NodeIds[3]);
        return result;
    }

    private static bool Equal(int elementIndex1, int elementIndex2, int node1, int node2)
    {
        return elementIndex1 == node1 && elementIndex2 == node2
               || elementIndex1 == node2 && elementIndex2 == node1;
    }
}