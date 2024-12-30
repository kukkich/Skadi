using SharpMath.FEM.Core;

namespace SharpMath.FiniteElement.Core.Assembling.Boundary.Second;

public readonly record struct SecondBoundary(Edge Edge, double[] Thetta);