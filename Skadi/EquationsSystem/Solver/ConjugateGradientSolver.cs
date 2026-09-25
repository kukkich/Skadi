using Microsoft.Extensions.Logging;
using Skadi.EquationsSystem.Preconditions;
using Skadi.LinearAlgebra;
using Skadi.LinearAlgebra.Matrices.Sparse;
using Skadi.LinearAlgebra.Vectors;

namespace Skadi.EquationsSystem.Solver;

public class ConjugateGradientSolver
(
    IPreconditionerFactory<SymmetricRowSparseMatrix> preconditionerFactory,
    ConjugateGradientSolverConfig config,
    ILogger logger
) : Method<ConjugateGradientSolverConfig>(config, logger), ISLAESolver<SymmetricRowSparseMatrix>
{
    private IPreconditioner _preconditioner = null!;
    private Equation<SymmetricRowSparseMatrix> _equation = null!;
    private Vector _r = null!;
    private Vector _z = null!;
    private Vector _rNext = null!;
    private Vector _aByZProduct = null!;

    public Vector Solve(Equation<SymmetricRowSparseMatrix> equation)
    {
        InitializeStartValues(equation);

        IterationProcess();

        return equation.Solution;
    }

    private void IterationProcess()
    {
        var fNorm = _equation.RightSide.Norm;

        for (var i = 1; i < Config.MaxIteration && _r.Norm / fNorm >= Config.Precision; i++)
        {
            var preconditionedRScalarProduct = Vector.ScalarProduct(
                _preconditioner.MultiplyOn(_r, _aByZProduct), // could pass any memory
                _r
            );

            _aByZProduct = _equation.Matrix.MultiplyOn(_z, _aByZProduct);

            var zScalarProduct = Vector.ScalarProduct(
                _aByZProduct,
                _z
            );

            var alpha = preconditionedRScalarProduct / zScalarProduct;

            VectorOps.LinearCombination(
                _equation.Solution, _z,
                1d, alpha,
                destination: _equation.Solution
            );

            _rNext = VectorOps.LinearCombination(
                _r, _aByZProduct,
                1d, -alpha,
                _rNext
            );

            var betta = Vector.ScalarProduct(_preconditioner.MultiplyOn(_rNext), _rNext) /
                        preconditionedRScalarProduct;

            _z = VectorOps.LinearCombination(
                _preconditioner.MultiplyOn(_rNext), _z,
                1d, betta,
                _z
            );

            _r = _rNext;

            if (i % 200 == 0)
            {
                Console.WriteLine($"[{i}]: {_r.Norm / fNorm:E15} / {Config.Precision:E15}");
            }
        }
    }

    private void InitializeStartValues(Equation<SymmetricRowSparseMatrix> equation)
    {
        _preconditioner = preconditionerFactory.CreatePreconditioner(equation.Matrix);

        _equation = equation;
        var AxProduct = equation.Matrix.MultiplyOn(equation.Solution);
        _r = VectorOps.Subtract(
            equation.RightSide,
            AxProduct
        );
        _z = _preconditioner.MultiplyOn(_r);

        _rNext = Vector.Create(equation.RightSide.Count);
        _aByZProduct = Vector.Create(equation.RightSide.Count);
    }
}

public readonly record struct ConjugateGradientSolverConfig(double Precision, int MaxIteration);