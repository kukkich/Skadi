using SharpMath.Geometry._2D;
using SharpMath.Geometry.Splitting;

namespace SharpMath.FEM.Geometry._2D.Quad;

public record RegularGridDefinition
(
    Point2D[,] ControlPoints,
    ICurveSplitter[] XSplitters,
    ICurveSplitter[] YSplitters,
    AreaDefinition[] Areas,
    CurveBoundDefinition[] CurveBounds
);