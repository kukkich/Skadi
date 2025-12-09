using Skadi.LinearAlgebra.Matrices.Sparse;
using Skadi.LinearAlgebra.Vectors;

namespace Skadi.EquationsSystem.Solver.Complex;

public class ComplexLocalOptimalSchemeSolver(CommonIterationSLAESolverConfig config) : IObservableSLAESolver<ComplexMatrix>
{
    public Vector Solve(Equation<ComplexMatrix> equation, IProgress<SLAESolverIteration> iteration)
    {
        throw new NotImplementedException();
    }
}