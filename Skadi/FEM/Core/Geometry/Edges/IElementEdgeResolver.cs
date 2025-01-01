namespace Skadi.FEM.Core.Geometry.Edges;

public interface IElementEdgeResolver
{
    public bool HasEdgeWithNode(IElement element, Edge edge);
    public Edge[] GetEdgedNodes(IElement element);
}