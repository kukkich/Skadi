using SharpMath.Geometry;
using SharpMath.Geometry._2D;

namespace SharpMath.FiniteElement.Materials.MaterialSetter.Areas;

public class AreasMaterialSetter : IMaterialSetter
{
    private readonly IPointsCollection<Point> _points;
    private readonly IMaterialArea<Point>[] _sections;
    private readonly int _defaultMaterialId;

    public AreasMaterialSetter(IPointsCollection<Point> points, IMaterialArea<Point>[] sections, int defaultMaterialIdId = 0)
    {
        _points = points;
        _sections = sections;
        _defaultMaterialId = defaultMaterialIdId;
    }

    public void SetMaterial(IFiniteElement element)
    {
        var points = element.NodeIndexes.Select(i => _points[i]).ToArray();

        foreach (var section in _sections)
        {
            if (!points.All(section.Contains))
            {
                continue;
            }

            element.MaterialId = section.MaterialId;
            return;
        }

        element.MaterialId = _defaultMaterialId;
    }
}