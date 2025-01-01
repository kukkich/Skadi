namespace Skadi.FEM.Core;

public interface IElement
{
    public int AreaId { get; }
    public IReadOnlyList<int> NodeIds { get; }
}