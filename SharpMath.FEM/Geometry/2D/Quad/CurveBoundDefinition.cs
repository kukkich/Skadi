using SharpMath.Geometry._2D.Shapes;

namespace SharpMath.FEM.Geometry._2D.Quad;

public record CurveBoundDefinition(
    Orientation Orientation,
    int LineId,
    int IntervalId,
    CurveType2D Type,
    object[] Parameters
);