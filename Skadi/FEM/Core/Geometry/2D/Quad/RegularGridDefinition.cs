using Skadi.Geometry._2D;
using Skadi.Geometry.Splitting;

namespace Skadi.FEM.Core.Geometry._2D.Quad;

public record RegularGridDefinition
(
    Vector2D[,] ControlPoints,
    ICurveSplitter[] XSplitters,
    ICurveSplitter[] YSplitters,
    AreaDefinition[] Areas,
    CurveBoundDefinition[] CurveBounds
);