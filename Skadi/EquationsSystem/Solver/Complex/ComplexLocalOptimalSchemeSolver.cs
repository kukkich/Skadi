using Skadi.EquationsSystem.Preconditions.Diagonal;
using Skadi.EquationsSystem.Smoothing;
using Skadi.LinearAlgebra;
using Skadi.LinearAlgebra.Matrices.Sparse;
using Skadi.LinearAlgebra.Vectors;

namespace Skadi.EquationsSystem.Solver.Complex;

public class ComplexLocalOptimalSchemeSolver
(
    CommonIterationSLAESolverConfig config,
    ISmoothing smoothing,
    int threadsCount
) : IObservableSLAESolver<ComplexMatrix>
{
    private DiagonalComplexPreconditioner _preconditioner = null!;

    public Vector Solve(Equation<ComplexMatrix> equation, IProgress<SLAESolverIteration> progress)
    {
        _preconditioner = new DiagonalComplexPreconditioner(equation.Matrix, threadsCount);
        
        var size = equation.RightSide.Count;
        var r = Vector.Create(size);
        var s = Vector.Create(size);
        var a = Vector.Create(size);
        var w = Vector.Create(size);
        
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
        
        _preconditioner.MultiplyOn(r, s);
        var p = s.Copy();
        ParallelLinAl.Multiply(equation.Matrix, p, a);
        var z = a.Copy();
        _preconditioner.MultiplyOn(z, w);

        smoothing.Initialize(equation.Solution, r);
        
        
        var solution = equation.Solution;
        var fNorm = ParallelLinAl.ComplexNorm(equation.RightSide, threadsCount);

        var tmp = Vector.Create(equation.RightSide.Count);

        var i = 1;
        for (; i < config.MaxIteration && ParallelLinAl.ComplexNorm(r, threadsCount) / fNorm >= config.Tolerance; i++)
        {
            var alpha = ParallelLinAl.ComplexPseudoScalarProduct(w, r, threadsCount) 
                        / ParallelLinAl.ComplexPseudoScalarProduct(w, z);

            ParallelLinAl.Sum
            (
                solution,
                ParallelLinAl.Multiply
                (
                    p,
                    alpha,
                    tmp,
                    threadsCount
                ),
                solution,
                threadsCount
            );

            ParallelLinAl.Subtract
            (
                r,
                ParallelLinAl.Multiply(z, alpha, tmp, threadsCount),
                r,
                threadsCount
            );
            ParallelLinAl.Subtract
            (
                s,
                ParallelLinAl.Multiply(w, alpha, tmp, threadsCount),
                s,
                threadsCount
            );
            ParallelLinAl.Multiply
            (
                equation.Matrix,
                s,
                a,
                threadsCount
            );

            var betta = -1d * ParallelLinAl.ComplexPseudoScalarProduct(w, a, threadsCount) 
                        / ParallelLinAl.ComplexPseudoScalarProduct(w, z, threadsCount);

            ParallelLinAl.Sum
            (
                s,
                ParallelLinAl.Multiply(p, betta, tmp, threadsCount),
                p,
                threadsCount
            );
            ParallelLinAl.Sum
            (
                a,
                ParallelLinAl.Multiply(z, betta, tmp, threadsCount),
                z,
                threadsCount
            );

            _preconditioner.MultiplyOn(z, w);

            smoothing.Apply(solution, r);
            var smoothedRelativeNorm = ParallelLinAl.ComplexNorm(smoothing.Residual, threadsCount) 
                                       / fNorm;

            progress.Report(new SLAESolverIteration(i, smoothedRelativeNorm));

            if (smoothedRelativeNorm >= config.Tolerance)
                continue;
            
            smoothing.Solution.CopyTo(solution);
            break;
        }

        return equation.Solution;
    }
}