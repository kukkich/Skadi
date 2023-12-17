using SharpMath.Geometry._2D;

namespace SharpMath.FiniteElement.Materials.MaterialSetter;

public class NullMaterialSetterFactory : IMaterialSetterFactory
{
    public IMaterialSetter Create(Point[] points, IEnumerable<IFiniteElement> elements)
    {
        return new NullMaterialSetter();
    }
}