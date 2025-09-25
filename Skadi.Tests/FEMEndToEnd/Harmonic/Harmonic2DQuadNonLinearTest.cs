using System.Linq.Expressions;
using System.Numerics;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Skadi.Algorithms.Integration;
using Skadi.EquationsSystem;
using Skadi.EquationsSystem.Solver;
using Skadi.FEM._2D.Assembling;
using Skadi.FEM._2D.BasisFunctions;
using Skadi.FEM._2D.Solution;
using Skadi.FEM.Assembling;
using Skadi.FEM.Assembling.Boundary.RegularGrid;
using Skadi.FEM.Assembling.Boundary.RegularGrid.Harmonic;
using Skadi.FEM.Core.Assembling;
using Skadi.FEM.Core.Assembling.Boundary;
using Skadi.FEM.Core.Assembling.Boundary.First;
using Skadi.FEM.Core.Assembling.Params;
using Skadi.FEM.Core.Geometry;
using Skadi.FEM.Core.Geometry._2D.Quad;
using Skadi.FEM.Deprecated._2D.Assembling;
using Skadi.FEM.Materials.HarmonicWithoutChi;
using Skadi.FEM.Materials.Providers;
using Skadi.FEM.Providers.Density;
using Skadi.Geometry._1D;
using Skadi.Geometry._2D;
using Skadi.Geometry.Splitting;
using Skadi.LinearAlgebra.Matrices;
using Skadi.LinearAlgebra.Matrices.Sparse;
using Vector = Skadi.LinearAlgebra.Vectors.Vector;

// ReSharper disable NullableWarningSuppressionIsUsed

// ReSharper disable InconsistentNaming

namespace Skadi.Tests.FEMEndToEnd.Harmonic;

[TestOf(typeof(HarmonicQuadLinearSolution))]
[TestOf(typeof(HarmonicQuadLinearAssembler))]
[TestOf(typeof(QuadLinearNonScaledFunctions2DProvider))]
[TestOf(typeof(HarmonicRegularBoundaryApplier<SparseMatrix>))]
[TestOf(typeof(HarmonicMatrixPortraitBuilder))]
public class Harmonic2DQuadNonLinearTest
{
    private const double Frequency = Math.PI * 1e-1;
    private const double Lambda = -2;
    private const double Sigma = 5;

    private static readonly Func<Vector2D, double, double> ExpectedSolution = (p, time) =>
        ExpectedSolutionSin!(p) * Math.Sin(time * Frequency) + ExpectedSolutionCos!(p) * Math.Cos(time * Frequency);

    private static readonly Func<Vector2D, double> ExpectedSolutionSin = p =>
        Math.Exp(p.X) - Math.Exp(p.Y);

    private static readonly Func<Vector2D, double> ExpectedSolutionCos = p =>
        Math.Exp(p.X) + Math.Exp(p.Y);

    private static readonly Func<Vector2D, double> DensitySin = p =>
        -1 * Lambda * ExpectedSolutionSin(p)
        -1 * Frequency * Sigma * ExpectedSolutionCos(p);

    private static readonly Func<Vector2D, double> DensityCos = p =>
        -1 * Lambda * ExpectedSolutionCos(p) 
        + Frequency * Sigma * ExpectedSolutionSin(p);

    private Grid<Vector2D, IElement> grid = null!;
    private IPointsCollection<Vector2D> testNodes = null!;
    private int[] sizesForTest = [1, 2, 4, 8, 16];
    
    [OneTimeSetUp]
    public void SetUp()
    {
        AreaDefinition[] areas = [new(0, 1, 0, 1, MaterialId: 0)];
        testNodes = new RegularGridBuilder().Build
        (
            new RegularGridDefinition
            (
                new Vector2D[,]
                {
                    {new(1, 1), new(5, 1)},
                    {new(1, 6), new(5, 6)},
                },
                [new UniformSplitter(77)],
                [new UniformSplitter(77)],
                areas,
                []
            )
        ).Nodes;
    }
    
    private HarmonicQuadLinearSolution Solve(int n)
    {
        #region SetUp
        RegularBoundaryCondition[] boundaries =
        [
            new()
            {
                ExpressionId = 0,
                Type = BoundaryConditionType.First,
                LeftBoundId = 0,
                RightBoundId = 1,
                BottomBoundId = 0,
                TopBoundId = 0,
            },
            new()
            {
                ExpressionId = 0,
                Type = BoundaryConditionType.First,
                LeftBoundId = 0,
                RightBoundId = 1,
                BottomBoundId = 1,
                TopBoundId = 1,
            },
            new()
            {
                ExpressionId = 0,
                Type = BoundaryConditionType.First,
                LeftBoundId = 0,
                RightBoundId = 0,
                BottomBoundId = 0,
                TopBoundId = 1,
            },
            new()
            {
                ExpressionId = 0,
                Type = BoundaryConditionType.First,
                LeftBoundId = 1,
                RightBoundId = 1,
                BottomBoundId = 0,
                TopBoundId = 1,
            },
        ];

        AreaDefinition[] areas = [new(0, 1, 0, 1, MaterialId: 0)];
        var gridDefinition = new RegularGridDefinition
        (
            new Vector2D[,]
            {
                {new(1, 1), new(5, 1)},
                {new(1, 6), new(5, 6)},
            },
            [new UniformSplitter(n)],
            [new UniformSplitter(n)],
            areas,
            []
        );
        grid = new RegularGridBuilder().Build(gridDefinition);
        
        var constantMoq = new Mock<IConstantProvider<double>>();
        constantMoq.Setup(x => x.Get())
            .Returns(() => Frequency);
        var localAssembler = new HarmonicQuadLinearAssembler
        (
            grid.Nodes,
            new AreaProvider<AreaDefinition>(areas),
            new Gauss2D(GaussConfig.Gauss4(3), NullLogger.Instance),
            new FromArrayMaterialProvider<Material>([new(Lambda, Sigma)]),
            new QuadLinearNonScaledFunctions2DProvider(),
            new QuadLinearNonScaledFunctions2DProvider(),
            new FuncDensity<Vector2D, Complex>(grid.Nodes, p => new Complex(DensitySin(p), DensityCos(p))),
            constantMoq.Object
        );

        var matrixPortraitBuilder = new HarmonicMatrixPortraitBuilder();
        var inserter = new SparseInserter();
        var solver = new LUSparseThroughProfileConversion();

        Expression<Func<Vector2D, Complex>> u = p => new Complex(ExpectedSolutionSin(p), ExpectedSolutionCos(p));
        var boundaryApplier = new HarmonicRegularBoundaryApplier<SparseMatrix>
        (
            new GaussExcluderSparse(),
            null,
            new ArrayExpressionProvider([u]),
            grid.Nodes,
            new BoundIndexesEvaluator(gridDefinition)
        );
        #endregion

        #region Equation
        var equation = new Equation<SparseMatrix>
        (
            matrixPortraitBuilder.Build(grid.Elements, grid.Nodes.TotalPoints),
            Vector.Create(grid.Nodes.TotalPoints * 2),
            Vector.Create(grid.Nodes.TotalPoints * 2)
        );
        var matrix = new MatrixSpan(stackalloc double[8 * 8], 8);
        Span<double> vector = stackalloc double[8];
        var indexes = new StackIndexPermutation(stackalloc int[8]);

        foreach (var element in grid.Elements)
        {
            localAssembler.AssembleMatrix(element, matrix, indexes);
            var localMatrix = new StackLocalMatrix(matrix, indexes);
            inserter.InsertMatrix(equation.Matrix, localMatrix);

            localAssembler.AssembleRightSide(element, vector, indexes);
            var localRightSide = new StackLocalVector(vector, indexes);
            inserter.InsertVector(equation.RightSide, localRightSide);
        }

        foreach (var boundary in boundaries)
        {
            boundaryApplier.Apply(equation, boundary);
        }

        var weights = solver.Solve(equation);
        #endregion

        return new HarmonicQuadLinearSolution
        (
            new QuadLinearNonScaledFunctions2DProvider(),
            grid,
            weights,
            Frequency
        );
    }

    private static double MaxError(HarmonicQuadLinearSolution solution, IPointsCollection<Vector2D> nodes)
    {
        var maxDiff = -1d;
        var avgError = 0d;
        
        var timeInterval = new Line1D(0, 2 * Math.PI);
        foreach (var t in new UniformSplitter(20).EnumerateValues(timeInterval))
        {
            for (var i = 0; i < nodes.TotalPoints; i++)
            {
                var point = nodes[i];
                var actual = solution.Calculate(point, t);
                var expected = ExpectedSolution(point, t);
                var diff = Math.Abs(actual - expected);
                avgError += diff*diff;
                maxDiff = Math.Max(maxDiff, diff);
            }
        }

        return maxDiff;
    }
    
    [Test]
    public void ErrorShouldDecreaseOnSubDomains()
    {
        var solutions = sizesForTest.Select(Solve).ToArray();
        var errors = solutions.Select(s => MaxError(s, testNodes)).ToArray();
        
        Assert.Multiple(() =>
        {
            for (var i = 1; i < errors.Length; i++)
            {
                var prevError = errors[i - 1];
                var error = errors[i];
                Assert.That(error, Is.LessThan(prevError));
            }
        });
    }
    
    [Test]
    public void ErrorShouldTendToDecreaseBy4TimesWhenSplitBy2Times()
    {
        const int convergenceOrder = 2;
        const int errorShouldDecreaseBy = 1 << convergenceOrder;
        
        var solutions = sizesForTest.Select(Solve).ToArray();
        var errors = solutions.Select(s => MaxError(s, testNodes)).ToArray();
        var errorRatioDeviationFromConvergence = errors
            .Skip(1)
            .Zip(errors, (current, previous) => previous / current)
            .Select(x => Math.Abs(errorShouldDecreaseBy - x))
            .ToArray();
        
        Assert.Multiple(() =>
        {
            for (var i = 1; i < errorRatioDeviationFromConvergence.Length; i++)
            {
                var prevDecrease = errorRatioDeviationFromConvergence[i - 1];
                var decrease = errorRatioDeviationFromConvergence[i];
                Assert.That(decrease, Is.LessThanOrEqualTo(prevDecrease));
            }
        });
    }
}