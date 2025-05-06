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
using Skadi.Geometry._1D;
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
    private VectorLinearLocalAssembler localAssembler = null!;
    private SymmetricMatrixEdgeGridPortraitBuilder matrixPortraitBuilder = null!;
    private Grid<Vector2D, IEdgeElement> grid = null!;
    private Grid<Vector2D, IEdgeElement> testGrid = null!;
    private IStackInserter<SymmetricRowSparseMatrix> inserter = null!;
    private ISLAESolver<SymmetricRowSparseMatrix> solver = null!;
    private IRegularBoundaryApplier<SymmetricRowSparseMatrix> boundaryApplier = null!;
    private RegularBoundaryCondition[] boundaries = null!;
    private EdgeResolver edgeResolver = null!;
    private static Vector2D UFunc(Vector2D p) => new(p.Y + 1, 0);
    
    private static Expression<Func<Vector2D, Vector2D>> U = p => UFunc(p);
    private static Expression<Func<Vector2D, double>> Ux = p => UFunc(p).X;
    private static Expression<Func<Vector2D, double>> Uy = p => UFunc(p).Y;
    private static Expression<Func<Vector2D, Vector2D>> ULeft = p => UFunc(new Vector2D(Left, p.Y));
    private static Expression<Func<Vector2D, Vector2D>> URight = p => UFunc(new Vector2D(Right, p.Y));
    private static Expression<Func<Vector2D, Vector2D>> UBottom = p => UFunc(new Vector2D(p.X, Bottom));
    private static Expression<Func<Vector2D, Vector2D>> UTop = p => UFunc(new Vector2D(p.X, Top));
    private Func<Vector2D, Vector2D> ExpectedSolution = U.Compile();
    
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
            [new ProportionalSplitter(2, 1.5d)],
            [new ProportionalSplitter(2, 2d / 3)],
            areas,
            []
        );
        var edgesPortraitBuilder = new EdgesPortraitBuilder(new QuadElementEdgeResolver());
        grid = new RegularEdgeGridBuilder(edgesPortraitBuilder).Build(gridDefinition);
        testGrid = new RegularEdgeGridBuilder(edgesPortraitBuilder).Build
        (
            new RegularGridDefinition
            (
                new Vector2D[,]
                {
                    {new(Left, Bottom), new(Right, Bottom)},
                    {new(Left, Top), new(Right, Top)},
                },
                [new UniformSplitter(2)],
                [new UniformSplitter(2)],
                areas,
                []
            )
        );
        localAssembler = new VectorLinearLocalAssembler
        (
            grid.Nodes,
            new AreaProvider<AreaDefinition>(areas),
            new Gauss2D(GaussConfig.Gauss4(3), NullLogger.Instance),
            new FromArrayMaterialProvider<Material>([new(Lambda, Sigma)]),
            new RectangleVectorBasicFunctionsProvider(grid.Nodes),
            new FuncDensity<Vector2D, Vector2D>(grid.Nodes, p => new(p.Y + 1, 0))
        );

        matrixPortraitBuilder = new SymmetricMatrixEdgeGridPortraitBuilder();
        inserter = new SymmetricInserter();
        solver = new ConjugateGradientSolver
        (
            new IncompleteLDLTPreconditionerFactory(),
            new ConjugateGradientSolverConfig(1e-15, 2000),
            NullLogger.Instance
        );

        edgeResolver = new EdgeResolver
        (
            edgesPortraitBuilder.Build(grid.Elements, grid.Nodes.TotalPoints),
            grid.Elements,
            new QuadElementEdgeResolver()
        );
        boundaryApplier = new VectorRegularBoundaryApplier<SymmetricRowSparseMatrix>
        (
            new GaussExcluderSymmetricSparse(),
            new ArrayExpressionProvider([Ux, Uy]),
            grid.Nodes,
            new BoundIndexesEvaluator(gridDefinition),
            edgeResolver
        );
    }

    [Test]
    public void SolutionOnNodesShouldBeCorrect()
    {
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
        var solution = new VectorFEMSolution2D
        (
            new RectangleVectorBasicFunctionsProvider(grid.Nodes),
            grid,
            weights,
            edgeResolver
        );

        Assert.Multiple(() =>
        {
            for (var i = 0; i < grid.Nodes.TotalPoints; i++)
            {
                var point = grid.Nodes[i];
                var actual = solution.Calculate(point);
                var expected = ExpectedSolution(point);
                var diff = actual - expected;
                Assert.That(diff.Length, Is.LessThanOrEqualTo(1e-13));
            }
        });
    }
}