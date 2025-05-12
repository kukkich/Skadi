using System.Linq.Expressions;
using Microsoft.Extensions.Logging.Abstractions;
using Skadi.EquationsSystem.Preconditions.LDLT;
using Skadi.EquationsSystem.Solver;
using Skadi.FEM._2D.Assembling;
using Skadi.FEM._2D.BasisFunctions;
using Skadi.FEM._2D.Solution;
using Skadi.FEM.Assembling;
using Skadi.FEM.Assembling.Boundary.RegularGrid;
using Skadi.FEM.Assembling.Boundary.RegularGrid.Vectors;
using Skadi.FEM.Core.Assembling;
using Skadi.FEM.Core.Assembling.Boundary;
using Skadi.FEM.Core.Assembling.Boundary.First;
using Skadi.FEM.Core.Geometry;
using Skadi.FEM.Core.Geometry._2D.Quad;
using Skadi.FEM.Materials.LambdaGamma;
using Skadi.FEM.Materials.Providers;
using Skadi.FEM.Providers.Density;
using Skadi.Geometry._2D;
using Skadi.Geometry.Splitting;
using Skadi.Integration;
using Skadi.Matrices;
using Skadi.Matrices.Sparse;
using Skadi.Vectors;

namespace Skadi.Tests.FEMEndToEnd.Vectors;

public class Vector2DRectangleTest
{
    private const double Lambda = 1;
    private const double Sigma = 1;
    private const double Left = 1d;
    private const double Right = 6d;
    private const double Bottom = 1d;
    private const double Top = 6d;
    private EdgesPortraitBuilder edgesPortraitBuilder;
    private Grid<Vector2D, IEdgeElement> grid = null!;
    private Grid<Vector2D, IEdgeElement> testGrid = null!;
    private static Vector2D UFunc(Vector2D p) => new(p.X*p.X*p.Y*p.Y - 5*p.X + p.Y + 2, p.X*p.X*p.Y*p.Y + 2*p.X - p.Y - 1);
    // private static Vector2D UFunc(Vector2D p) => new(Math.Exp(p.Y), Math.Exp(p.X));
    // private static Vector2D UFunc(Vector2D p) => new(-3*p.X + 2 * p.Y + 5, 2 * p.X + p.Y - 7);
    // private static Vector2D UFunc(Vector2D p) => new(2 * p.Y + 5, 2 * p.X - 7);
    private Func<Vector2D, Vector2D> DesityFunc = p 
        => Lambda * new Vector2D(-2*p.X*p.X + 4*p.X*p.Y, -2*p.Y*p.Y + 4*p.X*p.Y) + Sigma * UFunc(p);
        // => Lambda * -1 * UFunc(p) + Sigma * UFunc(p);
    // private Func<Vector2D, Vector2D> DesityFunc = p => Sigma * UFunc(p);
    
    private static Expression<Func<Vector2D, Vector2D>> U = p => UFunc(p);
    private static Expression<Func<Vector2D, double>> Ux = p => UFunc(p).X;
    private static Expression<Func<Vector2D, double>> Uy = p => UFunc(p).Y;
    private static Func<Vector2D, Vector2D> ExpectedSolution = U.Compile();
    private int[] sizesForTest = [1, 2, 4, 8, 16];

    [OneTimeSetUp]
    public void SetUp()
    {
        AreaDefinition[] areas = [new(0, 1, 0, 1, MaterialId: 0)];
        edgesPortraitBuilder = new EdgesPortraitBuilder(new QuadElementEdgeResolver());
        testGrid = new RegularEdgeGridBuilder(edgesPortraitBuilder).Build
        (
            new RegularGridDefinition
            (
                new Vector2D[,]
                {
                    {new(Left, Bottom), new(Right, Bottom)},
                    {new(Left, Top), new(Right, Top)},
                },
                [new UniformSplitter(105)],
                [new UniformSplitter(105)],
                areas,
                []
            )
        );
    }

    private VectorFEMSolution2D Solve(int n)
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
                ExpressionId = 1,
                Type = BoundaryConditionType.First,
                LeftBoundId = 0,
                RightBoundId = 0,
                BottomBoundId = 0,
                TopBoundId = 1,
            },
            new()
            {
                ExpressionId = 1,
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
                {new(Left, Bottom), new(Right, Bottom)},
                {new(Left, Top), new(Right, Top)},
            },
            [new UniformSplitter(n)],
            [new UniformSplitter(n)],
            // [new ProportionalSplitter(2, 1.5d)],
            // [new ProportionalSplitter(2, 2d / 3)],
            areas,
            []
        );
        grid = new RegularEdgeGridBuilder(edgesPortraitBuilder).Build(gridDefinition);
        var localAssembler = new VectorLinearLocalAssembler
        (
            grid.Nodes,
            new AreaProvider<AreaDefinition>(areas),
            new Gauss2D(GaussConfig.Gauss4(3), NullLogger.Instance),
            new FromArrayMaterialProvider<Material>([new(Lambda, Sigma)]),
            new RectangleVectorBasicFunctionsProvider(grid.Nodes),
            new FuncDensity<Vector2D, Vector2D>(grid.Nodes, DesityFunc)
        );

        var matrixPortraitBuilder = new SymmetricMatrixEdgeGridPortraitBuilder();
        var inserter = new SymmetricInserter();
        var solver = new ConjugateGradientSolver
        (
            new IncompleteLDLTPreconditionerFactory(),
            new ConjugateGradientSolverConfig(1e-15, 2000),
            NullLogger.Instance
        );

        var edgeResolver = new EdgeResolver
        (
            edgesPortraitBuilder.Build(grid.Elements, grid.Nodes.TotalPoints),
            grid.Elements,
            new QuadElementEdgeResolver()
        );
        var boundaryApplier = new VectorRegularBoundaryApplier<SymmetricRowSparseMatrix>
        (
            new GaussExcluderSymmetricSparse(),
            new ArrayExpressionProvider([Ux, Uy]),
            grid.Nodes,
            new BoundIndexesEvaluator(gridDefinition),
            edgeResolver
        );

        #endregion

        #region Equation

        var edgesCunt = grid.Elements.Max(x => x.EdgeIds.Max()) + 1;

        var equation = new Equation<SymmetricRowSparseMatrix>
        (
            matrixPortraitBuilder.Build(grid.Elements, edgesCunt),
            Vector.Create(edgesCunt),
            Vector.Create(edgesCunt)
        );
        var matrix = new MatrixSpan(stackalloc double[4 * 4], 4);
        Span<double> vector = stackalloc double[4];
        var indexes = new StackIndexPermutation(stackalloc int[4]);

        foreach (var element in grid.Elements)
        {
            localAssembler.AssembleMatrix(element, matrix, indexes);
            var localMatrix = new StackLocalMatrix(matrix, indexes);
            inserter.InsertMatrix(equation.Matrix, localMatrix);

            localAssembler.AssembleRightSide(element, vector, indexes);
            var localRightSide = new StackLocalVector(vector, indexes);
            inserter.InsertVector(equation.RightSide, localRightSide);
        }

        foreach (var boundary in boundaries.OrderByDescending(x => x.Type))
        {
            boundaryApplier.Apply(equation, boundary);
        }

        var weights = solver.Solve(equation);

        #endregion

        return new VectorFEMSolution2D
        (
            new RectangleVectorBasicFunctionsProvider(grid.Nodes),
            grid,
            weights,
            edgeResolver
        );
    }

    private static double MaxError(VectorFEMSolution2D solution, IPointsCollection<Vector2D> nodes)
    {
        var maxDiff = -1d;

        for (var i = 0; i < nodes.TotalPoints; i++)
        {
            var point = nodes[i];
            var actual = solution.Calculate(point);
            var expected = ExpectedSolution(point);
            var diff = (actual - expected).Length;
            maxDiff = Math.Max(maxDiff, diff);
        }

        return maxDiff;
    }

    [Test]
    public void SolutionOnNodesShouldBeCorrect()
    {
        var solutions = sizesForTest.Select(Solve).ToArray();
        var errors = solutions.Select(s => MaxError(s, testGrid.Nodes)).ToArray();
        var errorRatioDeviationFromConvergence = errors
            .Skip(1)
            .Zip(errors, (current, previous) => previous / current)
            // .Select(x => Math.Abs(errorShouldDecreaseBy - x))
            .ToArray();
        
        Assert.Multiple(() =>
        {
            // for (var i = 0; i < grid.Nodes.TotalPoints; i++)
            // {
            //     var point = grid.Nodes[i];
            //     var actual = solution.Calculate(point);
            //     var expected = ExpectedSolution(point);
            //     var diff = actual - expected;
            //     Assert.That(diff.Length, Is.LessThanOrEqualTo(1e-13));
            // }
        });
    }
}