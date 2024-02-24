using SharpMath.Geometry;
using SharpMath.Geometry._2D;

namespace SharpMath.FiniteElement.Materials.MaterialSetter;

public interface IMaterialSetterFactory
{
    public IMaterialSetter Create(IPointsCollection<Point> points);
}