namespace SharpMath.FEM.Core._2D;

public interface IElement2D : IElement
{
    public IReadOnlyCollection<int> BoundIds { get; set; }
}