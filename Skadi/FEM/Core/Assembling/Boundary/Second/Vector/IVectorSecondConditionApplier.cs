using Skadi.EquationsSystem;

namespace Skadi.FEM.Core.Assembling.Boundary.Second.Vector;

public interface IVectorSecondConditionApplier<TMatrix>
{
    public void Apply(Equation<TMatrix> equation, SecondBoundary condition);
}