using Microsoft.Extensions.Logging;
using Skadi.EquationsSystem.Preconditions;
using Skadi.LinearAlgebra;
using Skadi.LinearAlgebra.Matrices.Sparse;
using Skadi.LinearAlgebra.Vectors;

namespace Skadi.EquationsSystem.Solver;

//TODO для оптимизации можно взять пример с ConjugateGradientSolver
public class LocalOptimalScheme
(
    LUPreconditioner luPreconditioner,
    SparsePartialLUResolver sparseLuResolver,
    LocalOptimalSchemeConfig config,
    ILogger<LocalOptimalScheme> logger
) : Method<LocalOptimalSchemeConfig>(config, logger), ISLAESolver<SparseMatrix>
{
    private readonly IPreconditioner<SparseMatrix> _luPreconditioner = luPreconditioner;
    private SparseMatrix _preconditionMatrix = null!;
    private Vector _r = null!;
    private Vector _z = null!;
    private Vector _p = null!;

    public Vector Solve(Equation<SparseMatrix> equation)
    {
        PrepareProcess(equation);
        IterationProcess(equation);
        return equation.Solution;
    }

    private void PrepareProcess(Equation<SparseMatrix> equation)
    {
        _preconditionMatrix = _luPreconditioner.Decompose(equation.Matrix);
        _r = sparseLuResolver.CalcY(
            _preconditionMatrix,
            LinAl.Subtract(
                equation.RightSide,
                LinAl.Multiply(equation.Matrix, equation.Solution)
            )
        );
        _z = sparseLuResolver.CalcX(_preconditionMatrix, _r);
        _p = sparseLuResolver.CalcY(_preconditionMatrix, LinAl.Multiply(equation.Matrix, _z));
    }

    private void IterationProcess(Equation<SparseMatrix> equation)
    {
        var residual = Vector.ScalarProduct(_r, _r);
        var residualNext = residual;
        var minResidual = Math.Pow(Config.Eps, 2);

        int i;
        for (i = 1; i <= Config.MaxIterations && residualNext > minResidual; i++)
        {
            var scalarPP = Vector.ScalarProduct(_p, _p);

            var alpha = Vector.ScalarProduct(_p, _r) / scalarPP;

            LinAl.Sum(
                equation.Solution, LinAl.Multiply(alpha, _z),
                resultMemory: equation.Solution
            );

            var alphaMultiplyP = LinAl.Multiply(alpha, _p);
            var rNext = LinAl.Subtract(_r, alphaMultiplyP);

            var LAUr = sparseLuResolver.CalcY(
                _preconditionMatrix,
                LinAl.Multiply(equation.Matrix, sparseLuResolver.CalcX(_preconditionMatrix, rNext))
            );

            var beta = -1d * (Vector.ScalarProduct(_p, LAUr) / scalarPP);

            var zNext = LinAl.Sum(sparseLuResolver.CalcX(_preconditionMatrix, rNext), LinAl.Multiply(beta, _z, _z));

            var pNext = LinAl.Sum(LAUr, LinAl.Multiply(beta, _p, _p), LAUr);

            _r = rNext;
            _z = zNext;
            _p = pNext;

            residualNext = Vector.ScalarProduct(_r, _r) / residual;
        }

        if (residualNext > minResidual)
        {
            Logger.LogWarning(
                "LOS run out of iterations. residual: {residual:E}, expected: {min:E}. Max iterations: {iterations}",
                Math.Sqrt(residualNext), Config.Eps, Config.MaxIterations
            );
        }
        Logger.LogDebug(
            "LOS ended: iterations: [{iterations}/{maxIterations}], residual [{residual:E5}/{eps:E5}]", 
            i, Config.MaxIterations, Math.Sqrt(residualNext), Config.Eps
        );
    }
}

public class LocalOptimalSchemeConfig
{
    public double Eps { get; set; } = 1e-15;
    public int MaxIterations { get; set; } = 1000;
}