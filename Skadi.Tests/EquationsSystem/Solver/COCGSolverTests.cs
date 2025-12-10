using Skadi.EquationsSystem;
using Skadi.EquationsSystem.Preconditions.Diagonal;
using Skadi.EquationsSystem.Smoothing;
using Skadi.EquationsSystem.Solver;
using Skadi.EquationsSystem.Solver.Complex;
using Skadi.LinearAlgebra.Matrices.Sparse;
using Skadi.LinearAlgebra.Vectors;

namespace Skadi.Tests.EquationsSystem.Solver;

[TestFixture]
[TestOf(typeof(ComplexConjugateGradientSolver))]
[TestOf(typeof(DiagonalComplexPreconditioner))]
[TestOf(typeof(ResidualSmoothing))]
public class COCGSolverTests
{
    private const double Tolerance = 1e-10;
    private ISmoothing _smoothing = null!;
    private CommonIterationSLAESolverConfig _config;

    [SetUp]
    public void Setup()
    {
        _config = new()
        {
            MaxIteration = 1_000,
            Tolerance = 1e-10
        };
    }

    [Test(Description = """
                        | (1, 2) (8, 4)  (0, 0)  |       | (1, 1) |
                        | (8, 4) (3, 0)  (1, -4) | * x = | (1, 1) |
                        | (0, 0) (1, -4) (5, 4)  |       | (1, 1) |
                        """)]
    public void DefaultSlaeLackSmoothingShouldBeCorrect()
    {
        double[] di = [1, 2, 3, 5, 4];
        double[] gg = [8, 4, 1, -4];
        int[] ig = [0, 0, 1, 2];
        int[] jg = [0, 1];
        int[] idi = [0, 2, 3, 5];
        int[] ijg = [0, 2, 4];
        double[] bVector = [1, 1, 1, 1, 1, 1];

        var matrix = new ComplexMatrix(di, gg, idi, ijg, ig, jg);
        var right = new Vector(bVector);
        var initialSolution = Vector.Create(bVector.Length);

        var equation = new Equation<ComplexMatrix>(matrix, initialSolution, right);

        _smoothing = new ResidualSmoothing();

        ISLAESolver<ComplexMatrix> solver = new ComplexConjugateGradientSolver
        (
            _config,
            _smoothing,
            1
        );

        var result = solver.Solve(equation)
            .ToArray();

        double[] expected =
        [
            75.0 / 1037.0,
            215.0 / 1037.0,
            864.0 / 5185.0,
            -12.0 / 5185.0,
            81.0 / 305.0,
            37.0 / 305.0
        ];

        Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
    }

    [Test(Description = """
                        | (1, 2) (0, 0) (0, 0) |       | (1, 2)   |
                        | (0, 0) (3, 0) (0, 0) | * x = | (3, 3)   |
                        | (0, 0) (0, 0) (5, 4) |       | (41, 82) |
                        """)]
    public void DiagonalSlaeLackSmoothingShouldBeCorrect()
    {
        double[] di = [1, 2, 3, 5, 4];
        double[] gg = [];
        int[] ig = [0, 0, 0, 0];
        int[] jg = [];
        int[] idi = [0, 2, 3, 5];
        int[] ijg = [];
        double[] bVector = [1, 2, 3, 3, 41, 82];

        var matrix = new ComplexMatrix(di, gg, idi, ijg, ig, jg);
        var right = new Vector(bVector);
        var initialSolution = Vector.Create(bVector.Length);

        var equation = new Equation<ComplexMatrix>(matrix, initialSolution, right);

        _smoothing = new ResidualSmoothing();

        ISLAESolver<ComplexMatrix> solver = new ComplexConjugateGradientSolver
        (
            _config,
            _smoothing,
            1
        );

        var result = solver.Solve(equation)
            .ToArray();

        double[] expected =
        [
            1, 0, 1, 1, 13, 6
        ];

        Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
    }

    [Test(Description = """
                        | (1, 2) (8, 4)  (0, 0)  |       | (1, 1) |
                        | (8, 4) (3, 0)  (1, -4) | * x = | (1, 1) |
                        | (0, 0) (1, -4) (5, 4)  |       | (1, 1) |
                        """)]
    public void DefaultSlaeResidualSmoothingShouldBeCorrect()
    {
        double[] di = [1, 2, 3, 5, 4];
        double[] gg = [8, 4, 1, -4];
        int[] ig = [0, 0, 1, 2];
        int[] jg = [0, 1];
        int[] idi = [0, 2, 3, 5];
        int[] ijg = [0, 2, 4];
        double[] bVector = [1, 1, 1, 1, 1, 1];

        var matrix = new ComplexMatrix(di, gg, idi, ijg, ig, jg);
        var right = new Vector(bVector);
        var initialSolution = Vector.Create(bVector.Length);

        var equation = new Equation<ComplexMatrix>(matrix, initialSolution, right);

        _smoothing = new ResidualSmoothing();

        ISLAESolver<ComplexMatrix> solver = new ComplexConjugateGradientSolver
        (
            _config,
            _smoothing,
            1
        );

        var result = solver.Solve(equation)
            .ToArray();

        double[] expected =
        [
            75.0 / 1037.0,
            215.0 / 1037.0,
            864.0 / 5185.0,
            -12.0 / 5185.0,
            81.0 / 305.0,
            37.0 / 305.0
        ];

        Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
    }

    [Test(Description = """
                        | (1, 2) (0, 0) (0, 0) |       | (1, 2)   |
                        | (0, 0) (3, 0) (0, 0) | * x = | (3, 3)   |
                        | (0, 0) (0, 0) (5, 4) |       | (41, 82) |
                        """)]
    public void DiagonalSlaeResidualSmoothingShouldBeCorrect()
    {
        double[] di = [1, 2, 3, 5, 4];
        double[] gg = [];
        int[] ig = [0, 0, 0, 0];
        int[] jg = [];
        int[] idi = [0, 2, 3, 5];
        int[] ijg = [];
        double[] bVector = [1, 2, 3, 3, 41, 82];

        var matrix = new ComplexMatrix(di, gg, idi, ijg, ig, jg);
        var right = new Vector(bVector);
        var initialSolution = Vector.Create(bVector.Length);

        var equation = new Equation<ComplexMatrix>(matrix, initialSolution, right);

        _smoothing = new ResidualSmoothing();

        ISLAESolver<ComplexMatrix> solver = new ComplexConjugateGradientSolver
        (
            _config,
            _smoothing,
            1
        );

        var result = solver.Solve(equation)
            .ToArray();

        double[] expected =
        [
            1, 0, 1, 1, 13, 6
        ];

        Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
    }
}