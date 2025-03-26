using Skadi.FEM.Core.Geometry.Edges;

namespace Skadi.FEM.Assembling.Boundary.RegularGrid;

public interface IBoundIndexesEvaluator
{
    public IEnumerable<int> EnumerateNodes(RegularBoundaryCondition condition);
    public IEnumerable<Edge> EnumerateEdges(RegularBoundaryCondition condition);
}