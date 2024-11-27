using SharpMath.FEM.Core.Areas;

namespace SharpMath.FEM.Geometry._2D.Quad;

public record AreaDefinition(
    int LeftBoundId, 
    int RightBoundId,
    int BottomBoundId,
    int TopBoundId,
    int MaterialId = 0
) : IMaterialArea;