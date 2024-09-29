using SharpMath.Geometry._2D;
using SharpMath.Geometry.Shapes;

namespace SharpMath.FEM.Geometry._2D.Quad;

public record CurveBoundDefinition(
    Orientation Orientation,
    int LineId,
    int IntervalId,
    ICurve2D Curve
);