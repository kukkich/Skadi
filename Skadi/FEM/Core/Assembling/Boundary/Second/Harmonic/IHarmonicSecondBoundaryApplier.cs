namespace Skadi.FEM.Core.Assembling.Boundary.Second.Harmonic;

public interface IHarmonicSecondBoundaryApplier<TMatrix>
{
    public void Apply(Equation<TMatrix> equation, HarmonicSecondBoundary condition);
}