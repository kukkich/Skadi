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

    public LocalOptimalScheme(LUPreconditioner luPreconditioner, LUSparse luSparse, LocalOptimalSchemeConfig config)
        : base(config)
    {
        _luPreconditioner = luPreconditioner;
        _luSparse = luSparse;
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
        _p = _luSparse.CalcY(_preconditionMatrix, LinAl.Multiply(equation.Matrix,_z));
    }

    public Vector Solve(Equation<SparseMatrix> equation)
    {
        PrepareProcess(equation);
        IterationProcess(equation);
        return equation.Solution;
    }

    private void IterationProcess(Equation<SparseMatrix> equation)
    {
        var residual = Vector.ScalarProduct(_r, _r);
        var residualNext = residual;
        var i = 0;
        for (i = 1; i <= Config.MaxIterations && residualNext > Math.Pow(Config.Eps, 2); i++)
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
            //CourseHolder.GetInfo(i, residualNext);
        }

        Console.WriteLine($"{i} {residualNext:E6}");
    }
}

public class LocalOptimalSchemeConfig
{
    public required double Eps = 1e-15;
    public required int MaxIterations = 1000;
}