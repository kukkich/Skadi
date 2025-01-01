using Skadi.FiniteElement.Core.Harmonic;

namespace Skadi.FiniteElement.Core.Assembling.Boundary.Second.Harmonic;

public record struct HarmonicSecondCondition(
    int ElementId, 
    int LocalBound,
    double[] Thetta, 
    ComponentType Type
    );