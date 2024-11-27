using SharpMath.FiniteElement.Core.Harmonic;
using SharpMath.Primitives;

namespace SharpMath.FiniteElement.Core.Assembling.Boundary.Second.Harmonic;

public record struct HarmonicSecondCondition(
    int ElementId, 
    NonNegative<int> LocalBound,
    double[] Thetta, 
    ComponentType Type
    );