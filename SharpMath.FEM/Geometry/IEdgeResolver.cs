namespace SharpMath.FEM.Geometry;

public interface IEdgeResolver
{
    public int[] GetElementEdges(int elementId);
    public int GetEdge(int node1, int node2);
    public bool TryGetEdge(int node1, int node2, out int? edge);
    public (int minNode, int maxNode) GetNodesByEdge(int edgeId);
    public int[] GetElementsByEdge(int edgeId);
}