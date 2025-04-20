using Microsoft.Extensions.Logging.Abstractions;
using Skadi.EquationsSystem.Preconditions.Null;
using Skadi.EquationsSystem.Solver;
using Skadi.Matrices.Sparse;
using Skadi.Vectors;

namespace Skadi.Tests.EquationsSystem.Solver;

[TestOf(typeof(BiCGStabSolver<>))]
// ReSharper disable once InconsistentNaming
public class BiCGStabSolverTests
{
    private BiCGStabSolver<CSRMatrix> _solver;
    private const double Tolerance = 1e-15;
    [SetUp]
    public void Setup()
    {
        _solver = new BiCGStabSolver<CSRMatrix>
        (
            new NullPreconditionerFactory<CSRMatrix>(),
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

        var actual = _solver.Solve(equation);
        
        var r = LinAl.Subtract(equation.Matrix.MultiplyOn(actual), equation.RightSide);
        var relativeResidual = r.Norm / equation.RightSide.Norm;
        
        Assert.That(relativeResidual, Is.LessThanOrEqualTo(Tolerance));
    }
}