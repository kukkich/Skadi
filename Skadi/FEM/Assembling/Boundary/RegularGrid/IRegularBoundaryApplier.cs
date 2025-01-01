namespace Skadi.FEM.Assembling.Boundary.RegularGrid;

public interface IRegularBoundaryApplier<TMatrix>
{
    public void Apply(Equation<TMatrix> matrix, RegularBoundaryCondition condition);
}