using Microsoft.Extensions.Logging;
using SharpMath.EquationsSystem.Preconditions;
using SharpMath.Matrices.Sparse;
using SharpMath.Vectors;

namespace SharpMath.EquationsSystem.Solver;

//TODO для оптимизации можно взять пример с ConjugateGradientSolver
public class LocalOptimalScheme : Method<LocalOptimalSchemeConfig>, ISLAESolver<SparseMatrix>
{
    private readonly IPreconditioner<SparseMatrix> _luPreconditioner;
    private readonly LUSparse _luSparse;
    private SparseMatrix _preconditionMatrix = null!;
    private Vector _r = null!;
    private Vector _z = null!;
    private Vector _p = null!;

    public LocalOptimalScheme(
        LUPreconditioner luPreconditioner, 
        LUSparse luSparse, 
        LocalOptimalSchemeConfig config,
        ILogger logger)
        : base(config, logger)
    {
        _luPreconditioner = luPreconditioner;
        _luSparse = luSparse;
    }

    public Vector Solve(Equation<SparseMatrix> equation)
    {
        PrepareProcess(equation);
        IterationProcess(equation);
        return equation.Solution;
    }

    private void PrepareProcess(Equation<SparseMatrix> equation)
    {
        _preconditionMatrix = _luPreconditioner.Decompose(equation.Matrix);
        _r = _luSparse.CalcY(
            _preconditionMatrix,
            LinAl.Subtract(
                equation.RightSide,
                LinAl.Multiply(equation.Matrix, equation.Solution)
            )
        );
        _z = _luSparse.CalcX(_preconditionMatrix, _r);
        _p = _luSparse.CalcY(_preconditionMatrix, LinAl.Multiply(equation.Matrix, _z));
    }

    private void IterationProcess(Equation<SparseMatrix> equation)
    {
        var residual = Vector.ScalarProduct(_r, _r);
        var residualNext = residual;
        var i = 0;
        var minResidual = Math.Pow(Config.Eps, 2);
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

            var LAUr = _luSparse.CalcY(
                _preconditionMatrix,
                LinAl.Multiply(equation.Matrix, _luSparse.CalcX(_preconditionMatrix, rNext))
            );

            var beta = -1d * (Vector.ScalarProduct(_p, LAUr) / scalarPP);

            var zNext = LinAl.Sum(_luSparse.CalcX(_preconditionMatrix, rNext), LinAl.Multiply(beta, _z, _z));

            var pNext = LinAl.Sum(LAUr, LinAl.Multiply(beta, _p, _p), LAUr);

            _r = rNext;
            _z = zNext;
            _p = pNext;

            residualNext = Vector.ScalarProduct(_r, _r) / residual;

            // Logger.Log();
            //Console.WriteLine($"\t {i} {residualNext:E6}");
        }

        if (residualNext > minResidual)
        {
            Logger.LogWarning(
                "LOS run out of iterations. residual: {residual}, expected: {min}. Max iterations: {iterations}",
                Math.Sqrt(residualNext), Config.Eps, Config.MaxIterations
            );
        }
        Logger.LogDebug(
            "LOS ended: iterations: [{iterations}/{maxIterations}], residual {residual:E5}/{eps}", 
            i, Config.MaxIterations, Math.Sqrt(residualNext), Config.Eps
        );
    }
}

public class LocalOptimalSchemeConfig
{
    public double Eps { get; set; } = 1e-15;
    public int MaxIterations { get; set; } = 1000;
}