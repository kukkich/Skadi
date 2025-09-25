using Skadi.EquationsSystem;
using Skadi.EquationsSystem.Solver;
using Skadi.LinearAlgebra;
using Skadi.LinearAlgebra.Matrices.Sparse;
using Skadi.LinearAlgebra.Vectors;

namespace Skadi.Tests.EquationsSystem.Solver;

[TestFixture]
[TestOf(typeof(LUSparseThroughProfileConversion))]
public class LUSparseThroughProfileConversionTest
{
    private ISLAESolver<SparseMatrix> solver;
    private Vector solutionExpected;
    private Equation<SparseMatrix> EquationWithIndefiniteMatrix;
    private Equation<SparseMatrix> EquationWithPositiveDefinedMatrix;

    [SetUp]
    public void Setup()
    {
        var indefiniteMatrix = new SparseMatrix
        (
            [0, 0, 1, 3, 6, 10],
            [0, 0, 1, 0, 1, 2, 0, 1, 2, 3],
            [-9, 1, 1, -6, 12],
            [5, -7, -1, 2, 4, 2, 1, -4, 9, 8],
            [4, -4, 2, 7, -2, -4, 8, 1, -5, 5]
        );
        solutionExpected = new Vector(1, 2, 3, 4, 5);
        EquationWithIndefiniteMatrix = new Equation<SparseMatrix>
        (
            indefiniteMatrix,
            Vector.Create(5),
            LinAl.Multiply(indefiniteMatrix, solutionExpected)
        );
        
        var positiveDefiniteMatrix = new SparseMatrix
        (
            [0, 0, 1, 3, 6, 10],
            [0, 0, 1, 0, 1, 2, 0, 1, 2, 3],
            [32, 21, 23, 10, 2],
            [7, 6, 3, 5, 4, 2, 2, 4, 5, 6],
            [5, 2, 1, 4, 7, 6, 9, 1, 2, 1]
        );
        EquationWithPositiveDefinedMatrix = new Equation<SparseMatrix>
        (
            positiveDefiniteMatrix, 
            Vector.Create(5), 
            LinAl.Multiply(positiveDefiniteMatrix, solutionExpected)
        );

        solver = new LUSparseThroughProfileConversion();
    }
    
    [Test]
    public void TestWithIndefiniteMatrix()
    {
        var solutionActual = solver.Solve(EquationWithIndefiniteMatrix);

        var diff = LinAl.Subtract(solutionActual, solutionExpected);
        
        Assert.That(diff.Norm, Is.LessThanOrEqualTo(1e-14));
    }
    
    [Test]
    public void TestWithPositiveDefinedMatrix()
    {
        var solutionActual = solver.Solve(EquationWithPositiveDefinedMatrix);

        var diff = LinAl.Subtract(solutionActual, solutionExpected);
        
        Assert.That(diff.Norm, Is.LessThanOrEqualTo(1e-15));
    }
}