using Skadi.EquationsSystem;

namespace Skadi.FEM.Assembling.Boundary.RegularGrid;

public interface IRegularBoundaryApplier<TMatrix>
{
    public void Apply(Equation<TMatrix> equation, RegularBoundaryCondition condition);
}