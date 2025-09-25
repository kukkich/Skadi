using Skadi.EquationsSystem;

namespace Skadi.FEM.Core.Assembling.Boundary.First;

public interface IFirstBoundaryApplier<TMatrix>
{
    public void Apply(Equation<TMatrix> matrix, FirstCondition condition);
}