using SharpMath.FEM.Core;

namespace SharpMath.FEM.Geometry._2D;

public interface IElement2D : IElement
{
    public IReadOnlyCollection<int> BoundIds { get; set; }
}