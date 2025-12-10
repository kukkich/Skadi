using Skadi.LinearAlgebra.Vectors;

namespace Skadi.EquationsSystem.Solver;

public interface ISLAESolver<TMatrix>
{
    public Vector Solve(Equation<TMatrix> equation);
}

public interface IObservableSLAESolver<TMatrix> : ISLAESolver<TMatrix>
{
    public Vector Solve(Equation<TMatrix> equation, IProgress<SLAESolverIteration> progress);
    Vector ISLAESolver<TMatrix>.Solve(Equation<TMatrix> equation) => Solve(equation, NullProgress<SLAESolverIteration>.Instance);
}

public record struct SLAESolverIteration(int Iteration, double Residual);