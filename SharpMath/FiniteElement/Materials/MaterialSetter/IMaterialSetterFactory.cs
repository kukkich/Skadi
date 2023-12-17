using SharpMath.FiniteElement._2D;
using SharpMath.Geometry._2D;

namespace SharpMath.FiniteElement.Materials.MaterialSetter;

public interface IMaterialSetterFactory
{
    public IMaterialSetter Create(Point[] points, IEnumerable<IFiniteElement> elements);
}