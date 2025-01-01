using Skadi.FEM.Core;

namespace Skadi.FiniteElement.Core.Assembling.Boundary.Second;

public readonly record struct SecondBoundary(Edge Edge, double[] Thetta);