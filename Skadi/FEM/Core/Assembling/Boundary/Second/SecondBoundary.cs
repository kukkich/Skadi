using Skadi.FEM.Core.Geometry.Edges;

namespace Skadi.FEM.Core.Assembling.Boundary.Second;

public readonly record struct SecondBoundary(Edge Edge, double[] Thetta);