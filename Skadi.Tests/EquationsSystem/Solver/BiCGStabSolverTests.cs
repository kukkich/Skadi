using Microsoft.Extensions.Logging.Abstractions;
using Skadi.EquationsSystem;
using Skadi.EquationsSystem.Preconditions.LU;
using Skadi.EquationsSystem.Preconditions.Null;
using Skadi.EquationsSystem.Solver;
using Skadi.LinearAlgebra;
using Skadi.LinearAlgebra.Matrices.Sparse;
using Skadi.LinearAlgebra.Vectors;

namespace Skadi.Tests.EquationsSystem.Solver;

[TestOf(typeof(BiCGStabSolver<>))]
[TestOf(typeof(LUPreconditionerCSR))]
// ReSharper disable once InconsistentNaming
public class BiCGStabSolverTests
{
    private BiCGStabSolver<CSRMatrix> _notPreconditionedSolver;
    private BiCGStabSolver<CSRMatrix> _luPreconditionedSolver;
    private const double Tolerance = 1e-14;
    
    [SetUp]
    public void Setup()
    {
        _notPreconditionedSolver = new BiCGStabSolver<CSRMatrix>
        (
            new NullPreconditionerFactory<CSRMatrix>(),
            new ConjugateGradientSolverConfig(Tolerance, 1000),
            NullLogger.Instance
        );
        _luPreconditionedSolver = new BiCGStabSolver<CSRMatrix>
        (
            new LUPreconditionerCSRFactory(),
            new ConjugateGradientSolverConfig(Tolerance, 1000),
            NullLogger.Instance
        );
    }

    [Test]
    public void CSRNonDefinedSimpleMatrixTest()
    {
        var equation = new Equation<CSRMatrix>
        (
            new CSRMatrix
            (
                [0, 1, 3, 5, 7, 8],
                [1, 0, 2, 1, 3, 2, 4, 3],
                [1, 1, 1, 1, 1, 1, 1, 1]
            ),
            Vector.Create(5),
            new Vector(2, 4, 6, 8, 4)
        );

        var actual = _notPreconditionedSolver.Solve(equation);
        
        var r = LinAl.Subtract(equation.Matrix.MultiplyOn(actual), equation.RightSide);
        var relativeResidual = r.Norm / equation.RightSide.Norm;
        
        Assert.That(relativeResidual, Is.LessThanOrEqualTo(Tolerance));
    }
    
    [Test]
    public void CSRNonDefinedMatrixWithILUPreconditionerTest()
    {
        var matrix = new CSRMatrix
        (
            [0, 4, 7, 10, 13, 17, 23],
            [
                0, 1, 3, 5,
                1, 2, 4,
                0, 2, 5, 
                1, 3, 5,
                0, 1, 2, 4,
                0, 1, 2, 3, 4, 5
            ],
            [
                10, -4, 1, 1,
                -1, 8, 2, 
                4, 7, -3, 
                2, 7, 4,
                5, -1, 2, 8,
                -7, 9, -3, 2, 14, -5
            ]
        );
        var equation = new Equation<CSRMatrix>
        (
            matrix,
            Vector.Create(6),
            new Vector(12, 32, 7, 56, 49, 50)
        );

        var actual = _luPreconditionedSolver.Solve(equation);
        
        var r = LinAl.Subtract(equation.Matrix.MultiplyOn(actual), equation.RightSide);
        var relativeResidual = r.Norm / equation.RightSide.Norm;
        
        Assert.That(relativeResidual, Is.LessThanOrEqualTo(Tolerance));
    }
}