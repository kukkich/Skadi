using Skadi.LinearAlgebra.Matrices.Sparse;
using Skadi.LinearAlgebra.Vectors;

namespace Skadi.EquationsSystem.Solver.Complex;

public class ComplexConjugateGradientSolver : ISLAESolver<ComplexMatrix>
{
    public Vector Solve(Equation<ComplexMatrix> equation)
    {
        throw new NotImplementedException();
    }
}