using Skadi.Vectors;

namespace Skadi.EquationsSystem.Solver;

public interface ISLAESolver<TMatrix>
{
    public Vector Solve(Equation<TMatrix> equation);
}
