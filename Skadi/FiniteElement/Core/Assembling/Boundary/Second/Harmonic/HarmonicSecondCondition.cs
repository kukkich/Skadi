using Skadi.Primitives;
using Skadi.FiniteElement.Core.Harmonic;

namespace Skadi.FiniteElement.Core.Assembling.Boundary.Second.Harmonic;

public record struct HarmonicSecondCondition(
    int ElementId, 
    NonNegative<int> LocalBound,
    double[] Thetta, 
    ComponentType Type
    );