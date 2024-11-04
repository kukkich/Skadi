namespace SharpMath.FEM.Core;

public interface IElement
{
    public int AreaId { get; }
    public IReadOnlyCollection<int> NodeIds { get; }
}