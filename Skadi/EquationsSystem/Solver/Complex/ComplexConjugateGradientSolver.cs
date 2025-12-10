using Skadi.EquationsSystem.Preconditions.Diagonal;
using Skadi.EquationsSystem.Smoothing;
using Skadi.LinearAlgebra;
using Skadi.LinearAlgebra.Matrices.Sparse;
using Skadi.LinearAlgebra.Vectors;

namespace Skadi.EquationsSystem.Solver.Complex;

public class ComplexConjugateGradientSolver
(
    CommonIterationSLAESolverConfig config,
    ISmoothing smoothing,
    int threadsCount
) : IObservableSLAESolver<ComplexMatrix>
{
    public Vector Solve(Equation<ComplexMatrix> equation, IProgress<SLAESolverIteration> progress)
    {
        var preconditioner = new DiagonalComplexPreconditioner(equation.Matrix, threadsCount);
        
        var size = equation.RightSide.Count;
        var r = Vector.Create(size);
        var rNext = Vector.Create(size);
        var z = Vector.Create(size);
        var zNext = Vector.Create(size);

        ParallelLinAl.Subtract
        (
            equation.RightSide, 
            ParallelLinAl.Multiply
                (
                    equation.Matrix, 
                    equation.Solution,
                    r, // any tmp vector
                    threadsCount
                ),
            r,
            threadsCount
        );
        preconditioner.MultiplyOn(r, z);
        var p = z.Copy();
        var pNext = Vector.Create(size);
        
        smoothing.Initialize(equation.Solution, r);
        
        var solution = equation.Solution;
        var fNorm = ParallelLinAl.ComplexNorm(equation.RightSide);

        var matrixByP = Vector.Create(size);
        var buffer = Vector.Create(size);
        
        var iteration = 1;
        for (; iteration < config.MaxIteration && ParallelLinAl.ComplexNorm(r) / fNorm >= config.Tolerance; iteration++)
        {
            ParallelLinAl.Multiply(equation.Matrix, p, matrixByP, threadsCount);
            var a = ParallelLinAl.ComplexPseudoScalarProduct(r, z, threadsCount) 
                    / ParallelLinAl.ComplexPseudoScalarProduct(matrixByP, p, threadsCount);

            ParallelLinAl.Sum
            (
                solution,
                ParallelLinAl.Multiply(p, a, buffer, threadsCount),
                solution,
                threadsCount
            );
            
            ParallelLinAl.Subtract
            (
                r, 
                ParallelLinAl.Multiply(
                    matrixByP, 
                    a,
                    buffer,
                    threadsCount
                ),
                rNext,
                threadsCount
            );
            
            preconditioner.MultiplyOn(rNext, zNext);

            var b = ParallelLinAl.ComplexPseudoScalarProduct(rNext, zNext, threadsCount)
                    / ParallelLinAl.ComplexPseudoScalarProduct(r, z, threadsCount);
            
            ParallelLinAl.Sum
            (
                zNext,
                ParallelLinAl.Multiply
                (
                    p,
                    b,
                    buffer,
                    threadsCount
                ),
                pNext,
                threadsCount
            );

            (r, rNext) = (rNext, r);
            (z, zNext) = (zNext, z);
            (p, pNext) = (pNext, p);
            
            smoothing.Apply(solution, r);
            var smoothedRelativeResidual = smoothing.Residual.Norm / fNorm;
            
            progress.Report(new SLAESolverIteration(iteration, smoothedRelativeResidual));

            if (smoothedRelativeResidual >= config.Tolerance)
                continue;
            
            smoothing.Solution.CopyTo(solution);
            break;
        }

        return equation.Solution;
    }
}