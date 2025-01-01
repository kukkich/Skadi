using Skadi.FEM.Core.Areas;

namespace Skadi.FEM.Core.Geometry._2D.Quad;

public record AreaDefinition(
    int LeftBoundId, 
    int RightBoundId,
    int BottomBoundId,
    int TopBoundId,
    int MaterialId = 0
) : IMaterialArea;