namespace Skadi.FEM.Core.Geometry;

public interface IEdgeElement : IElement
{
    public IReadOnlyList<int> EdgeIds { get; set; }
}