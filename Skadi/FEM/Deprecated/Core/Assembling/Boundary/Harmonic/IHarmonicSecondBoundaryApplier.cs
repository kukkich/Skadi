using Skadi.EquationsSystem;

namespace Skadi.FEM.Deprecated.Core.Assembling.Boundary.Harmonic;

public interface IHarmonicSecondBoundaryApplier<TMatrix>
{
    public void Apply(Equation<TMatrix> equation, HarmonicSecondCondition condition);
}