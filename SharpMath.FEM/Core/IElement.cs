namespace SharpMath.FEM.Core;

public interface IElement
{
    public IReadOnlyCollection<int> NodeIds { get; }
}