using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Skadi.EquationsSystem.Preconditions;
using Skadi.EquationsSystem.Preconditions.Diagonal;
using Skadi.EquationsSystem.Preconditions.Hollesky;
using Skadi.EquationsSystem.Preconditions.LDLT;
using Skadi.EquationsSystem.Solver;
using Skadi.Matrices.Sparse;
using Vector = Skadi.Vectors.Vector;

// ReSharper disable InconsistentNaming

namespace Skadi.Tests.EquationsSystem.Solver;

[TestFixture]
[TestOf(typeof(ConjugateGradientSolver))]
public class ConjugateGradientSolverTestTests
{
    private readonly IPreconditionerFactory<SymmetricRowSparseMatrix> diagonalPreconditionerFactory = new DiagonalPreconditionerFactory();
    private readonly IPreconditionerFactory<SymmetricRowSparseMatrix> holesskyPreconditionerFactory = new IncompleteHolleskyPreconditionerFactory();
    private readonly IPreconditionerFactory<SymmetricRowSparseMatrix> LDLTPreconditionerFactory = new IncompleteLDLTPreconditionerFactory();
    private IPreconditionerFactory<SymmetricRowSparseMatrix> UnitPreconditioner = null!;
    private Equation<SymmetricRowSparseMatrix> equation = null!;
    private readonly Vector solutionExpected = new (-10, 9, -8, 7, -6, 5);
    private static readonly ConjugateGradientSolverConfig config = new (1e-14, 7);

    private readonly Func<IPreconditionerFactory<SymmetricRowSparseMatrix>, ConjugateGradientSolver> solverFactory =
        factory => new ConjugateGradientSolver(factory, config, NullLogger.Instance);
    
    [SetUp]
    public void Setup()
    {
        var matrix = new SymmetricRowSparseMatrix(
            [0, 0, 1, 2, 3, 6, 10],
            [0, 0, 1, 0, 1, 3, 0, 1, 2, 4],
            [-4, 3, 2, -3, 2, -7, -1, -3, 3, 5],
            [45, 54, 17, 45, 32, 14]
        );
        var rightSight = new Vector(-497, 513, -151, 375, -168, -1);

        var solution = Vector.Create(6);
        equation = new Equation<SymmetricRowSparseMatrix>(matrix, solution, rightSight);

        var preconditionerMockSetup = new Mock<IPreconditioner>();
        preconditionerMockSetup.Setup(x => x.MultiplyOn(It.IsAny<Vector>(), It.IsAny<Vector>()))
            .Returns((Vector x, Vector? _) => x);
        var preconditionerMock = preconditionerMockSetup.Object;
        
        var preconditionerFactoryMock = new Mock<IPreconditionerFactory<SymmetricRowSparseMatrix>>();
        preconditionerFactoryMock.Setup(x => x.CreatePreconditioner(It.IsAny<SymmetricRowSparseMatrix>()))
            .Returns(() => preconditionerMock);
        UnitPreconditioner = preconditionerFactoryMock.Object;
    }

    [Test]
    public void TestWithDiagonalPreconditioner()
    { 
        var solver = solverFactory(diagonalPreconditionerFactory);
        var solutionActual = solver.Solve(equation);

        var diff = LinAl.Subtract(solutionActual, solutionExpected);
        
        Assert.That(diff.Norm, Is.LessThanOrEqualTo(config.Precision));
    }
    
    [Test]
    public void TestWithIncompleteHolleskyPreconditioner()
    {
        var solver = solverFactory(holesskyPreconditionerFactory);
        var solutionActual = solver.Solve(equation);

        var diff = LinAl.Subtract(solutionActual, solutionExpected);
        
        Assert.That(diff.Norm, Is.LessThanOrEqualTo(config.Precision));
    }
    
    [Test]
    public void TestWithIncompleteLDLTPreconditioner()
    {
        var solver = solverFactory(LDLTPreconditionerFactory);
        var solutionActual = solver.Solve(equation);

        var diff = LinAl.Subtract(solutionActual, solutionExpected);
        
        Assert.That(diff.Norm, Is.LessThanOrEqualTo(config.Precision));
    }
    
    [Test]
    public void TestWithoutPreconditioner()
    {
        var solver = solverFactory(UnitPreconditioner);
        var solutionActual = solver.Solve(equation);

        var diff = LinAl.Subtract(solutionActual, solutionExpected);
        
        Assert.That(diff.Norm, Is.LessThanOrEqualTo(config.Precision));
    }
}