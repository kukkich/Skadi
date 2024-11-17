using SharpMath.FEM.Core;

namespace SharpMath.FEM.Geometry;

public interface IElementEdgeResolver
{
    public bool HasEdgeWithNode(IElement element, int node1, int node2);
    public (int MinNode, int MaxNode)[] GetEdgedNodes(IElement element);
}