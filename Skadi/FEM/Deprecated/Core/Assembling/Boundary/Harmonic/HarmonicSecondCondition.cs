using Skadi.FEM.Deprecated.Core.Harmonic_OLD;

namespace Skadi.FEM.Deprecated.Core.Assembling.Boundary.Harmonic;

public record struct HarmonicSecondCondition(
    int ElementId, 
    int LocalBound,
    double[] Thetta, 
    ComponentType Type
    );