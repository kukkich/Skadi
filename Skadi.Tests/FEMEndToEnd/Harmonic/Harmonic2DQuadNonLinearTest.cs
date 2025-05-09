using System.Linq.Expressions;
using System.Numerics;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
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
using Skadi.Integration;
using Skadi.Matrices;
using Skadi.Matrices.Sparse;
using Vector = Skadi.Vectors.Vector;

// ReSharper disable NullableWarningSuppressionIsUsed

// ReSharper disable InconsistentNaming

namespace Skadi.Tests.FEMEndToEnd.Harmonic;

public class Harmonic2DQuadNonLinearTest
{
    private const double Frequency = Math.PI * 1e-4;
    private const double Lambda = -2;
    private const double Sigma = 5;

    private static readonly Func<Vector2D, double, double> ExpectedSolution = (p, time) =>
        ExpectedSolutionSin!(p) * Math.Sin(time * Frequency) + ExpectedSolutionCos!(p) * Math.Cos(time * Frequency);

    private static readonly Func<Vector2D, double> ExpectedSolutionSin = p =>
        Math.Pow(p.X, 2) - Math.Pow(p.Y, 2);

    private static readonly Func<Vector2D, double> ExpectedSolutionCos = p =>
        -1 * Math.Pow(p.X, 2) * p.Y + Math.Pow(p.Y, 2) + p.X - 1;

    private static readonly Func<Vector2D, double> DensitySin = p =>
        -1 * Frequency * Sigma * ExpectedSolutionCos(p);

    private static readonly Func<Vector2D, double> DensityCos = p =>
        -1 * Lambda * (-2d * p.Y + 2) + Frequency * Sigma * ExpectedSolutionSin(p);

    private HarmonicQuadLinearAssembler localAssembler = null!;
    private HarmonicMatrixPortraitBuilder matrixPortraitBuilder = null!;
    private Grid<Vector2D, IElement> grid = null!;
    private IPointsCollection<Vector2D> testNodes = null!;
    private IStackInserter<SparseMatrix> inserter = null!;
    private ISLAESolver<SparseMatrix> solver = null!;
    private IRegularBoundaryApplier<SparseMatrix> boundaryApplier = null!;
    private RegularBoundaryCondition[] boundaries = null!;
    private HarmonicQuadLinearSolution Solution = null!;
    
    [OneTimeSetUp]
    public void Setup()
    {
        boundaries =
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
            [new UniformSplitter(33)],
            [new UniformSplitter(33)],
            areas,
            []
        );
        grid = new RegularGridBuilder().Build(gridDefinition);
        testNodes = new RegularGridBuilder().Build
        (
            new RegularGridDefinition
            (
                new Vector2D[,]
                {
                    {new(1, 1), new(5, 1)},
                    {new(1, 6), new(5, 6)},
                },
                [new UniformSplitter(2)],
                [new UniformSplitter(2)],
                areas,
                []
            )
        ).Nodes;
        var constantMoq = new Mock<IConstantProvider<double>>();
        constantMoq.Setup(x => x.Get())
            .Returns(() => Frequency);
        localAssembler = new HarmonicQuadLinearAssembler
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

        matrixPortraitBuilder = new HarmonicMatrixPortraitBuilder();
        inserter = new SparseInserter();
        solver = new LUSparseThroughProfileConversion();

        Expression<Func<Vector2D, Complex>> u = p => new Complex(ExpectedSolutionSin(p), ExpectedSolutionCos(p));
        boundaryApplier = new HarmonicRegularBoundaryApplier<SparseMatrix>
        (
            new GaussExcluderSparse(),
            null,
            new ArrayExpressionProvider([u]),
            grid.Nodes,
            new BoundIndexesEvaluator(gridDefinition)
        );

        var weights = Solve();
        Solution = new HarmonicQuadLinearSolution
        (
            new QuadLinearNonScaledFunctions2DProvider(),
            grid,
            weights,
            Frequency
        );
    }

    private Vector Solve()
    {
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
        return weights;
    }
    
    [Test]
    public void SolutionOnTestGridNodesShouldBeCorrect()
    {
        var timeInterval = new Line1D(0, 2 * Math.PI);
        Assert.Multiple(() =>
        {
            foreach (var t in new UniformSplitter(20).EnumerateValues(timeInterval))
            {
                for (var i = 0; i < testNodes.TotalPoints; i++)
                {
                    var point = testNodes[i];
                    var actual = Solution.Calculate(point, t);
                    var expected = ExpectedSolution(point, t);
                    Assert.That(actual, Is.EqualTo(expected).Within(3e-2)); // Большая погрешность
                }
            }
        });
    }

    

    [Test]
    public void SolutionOnNodesShouldBeCorrect()
    {
        var timeInterval = new Line1D(0, 2 * Math.PI);
        Assert.Multiple(() =>
        {
            foreach (var t in new UniformSplitter(20).EnumerateValues(timeInterval))
            {
                for (var i = 0; i < grid.Nodes.TotalPoints; i++)
                {
                    var point = grid.Nodes[i];
                    var actual = Solution.Calculate(point, t);
                    var expected = ExpectedSolution(point, t);
                    Assert.That(actual, Is.EqualTo(expected).Within(1e-12));
                }
            }
        });
    }
}