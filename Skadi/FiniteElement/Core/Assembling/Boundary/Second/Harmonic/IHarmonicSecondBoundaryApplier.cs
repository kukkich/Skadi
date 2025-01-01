namespace Skadi.FiniteElement.Core.Assembling.Boundary.Second.Harmonic;

public interface IHarmonicSecondBoundaryApplier<TMatrix>
{
    public void Apply(Equation<TMatrix> equation, HarmonicSecondCondition condition);
}