using SharpMath.FiniteElement.Core.Harmonic;
using SharpMath.Geometry._2D;
using SharpMath.Primitives;

namespace SharpMath.FiniteElement.Core.Assembling.Boundary.Second;

public record struct SecondCondition(int ElementId, NonNegative<int> Bound, double[] Thetta, ComponentType Type);