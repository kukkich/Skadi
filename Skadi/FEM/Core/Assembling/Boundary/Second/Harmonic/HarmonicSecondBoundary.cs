using System.Numerics;
using Skadi.FEM.Core.Geometry.Edges;

namespace Skadi.FEM.Core.Assembling.Boundary.Second.Harmonic;

public readonly record struct HarmonicSecondBoundary(Edge Edge, Complex[] Thetta);