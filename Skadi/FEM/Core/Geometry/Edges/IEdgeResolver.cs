namespace Skadi.FEM.Core.Geometry.Edges;

public interface IEdgeResolver
{
    public int[] GetEdgeIdsByElement(int elementId);
    public int GetEdgeId(Edge edge);
    public bool TryGetEdgeId(Edge edge, out int? edgeId);
    public Edge GetEdgeById(int edgeId);
    public int[] GetElementsByEdgeId(int edgeId);
}