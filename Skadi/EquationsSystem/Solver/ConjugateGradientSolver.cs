﻿using Skadi.EquationsSystem.Preconditions;
using Skadi.Matrices.Sparse;
using Skadi.Vectors;

namespace Skadi.EquationsSystem.Solver;

public class ConjugateGradientSolver : ISLAESolver<SymmetricRowSparseMatrix>
{
    private readonly IPreconditionerFactory<SymmetricRowSparseMatrix> _preconditionerFactory;
    private readonly double _precision;
    private readonly int _maxIteration;

    private IPreconditioner _preconditioner = null!;
    private Equation<SymmetricRowSparseMatrix> _equation = null!;
    private Vector _r = null!;
    private Vector _z = null!;
    private Vector _rNext = null!;
    private Vector _aByZProduct = null!;

    public ConjugateGradientSolver(IPreconditionerFactory<SymmetricRowSparseMatrix> preconditionerFactory, double precision, int maxIteration)
    {
        _preconditionerFactory = preconditionerFactory;
        _precision = precision;
        _maxIteration = maxIteration;
    }
     
    public Vector Solve(Equation<SymmetricRowSparseMatrix> equation)
    {
        InitializeStartValues(equation);

        IterationProcess();

        return equation.Solution;
    }

    private void IterationProcess()
    {
        var fNorm = _equation.RightSide.Norm;

        for (var i = 1; i < _maxIteration && (_r.Norm / fNorm) >= _precision*_precision; i++)
        {
            var preconditionedRScalarProduct = Vector.ScalarProduct(
                _preconditioner.MultiplyOn(_r, _aByZProduct), // could pass any memory
                _r
            );

            _aByZProduct = LinAl.Multiply(_equation.Matrix, _z, _aByZProduct);

            var zScalarProduct = Vector.ScalarProduct(
                _aByZProduct,
                _z
            );

            var alpha = preconditionedRScalarProduct / zScalarProduct;

            LinAl.LinearCombination(
                _equation.Solution, _z,
                1d, alpha,
                resultMemory: _equation.Solution
            );

            _rNext = LinAl.LinearCombination(
                _r, _aByZProduct,
                1d, -alpha,
                _rNext
            );

            var betta = Vector.ScalarProduct(_preconditioner.MultiplyOn(_rNext), _rNext) /
                        preconditionedRScalarProduct;

            _z = LinAl.LinearCombination(
                _preconditioner.MultiplyOn(_rNext), _z,
                1d, betta,
                _z
            );

            _r = _rNext;

            if (i % 200 == 0)
            {
                Console.WriteLine($"[{i}]: {_r.Norm / fNorm:E15} / {_precision*_precision:E15}");
            }
        }

    }

    private void InitializeStartValues(Equation<SymmetricRowSparseMatrix> equation)
    {
        _preconditioner = _preconditionerFactory.CreatePreconditioner(equation.Matrix);

        _equation = equation;
        var AxProduct = LinAl.Multiply(equation.Matrix, equation.Solution);
        _r = LinAl.Subtract(
            equation.RightSide,
            AxProduct
        );
        _z = _preconditioner.MultiplyOn(_r);

        _rNext = Vector.Create(equation.RightSide.Length);
        _aByZProduct = Vector.Create(equation.RightSide.Length);
    }
}