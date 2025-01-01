namespace Skadi.FEM.Core.Geometry;

public interface IElement
{
    public int AreaId { get; }
    public IReadOnlyList<int> NodeIds { get; }
}