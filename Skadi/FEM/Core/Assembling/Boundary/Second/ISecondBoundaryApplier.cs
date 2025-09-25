using Skadi.EquationsSystem;

namespace Skadi.FEM.Core.Assembling.Boundary.Second;

public interface ISecondBoundaryApplier<TMatrix>
{
    public void Apply(Equation<TMatrix> equation, SecondBoundary condition);
}