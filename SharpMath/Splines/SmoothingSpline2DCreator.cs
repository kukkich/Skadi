using SharpMath.EquationsSystem.Solver;
using SharpMath.FiniteElement._2D;
using SharpMath.FiniteElement._2D.Assembling;
using SharpMath.FiniteElement._2D.BasisFunctions;
using SharpMath.FiniteElement._2D.Elements;
using SharpMath.FiniteElement.Core.Assembling.Boundary.First;
using SharpMath.FiniteElement.Core.Assembling.Boundary.Second;
using SharpMath.FiniteElement.Core.Harmonic;
using SharpMath.FiniteElement.Providers.Density;
using SharpMath.Geometry;
using SharpMath.Geometry._2D;
using SharpMath.Matrices;
using SharpMath.Matrices.Sparse;
using SharpMath.Vectors;

namespace SharpMath.Splines;

public class SmoothingSpline2DCreator : ISplineCreator<ISpline2D<Point>, Point, Element>
{
    private HermiteBasisFunctions2DProvider _basisFunctionsProvider;
    private SplineContext<Point, Element, Matrix> _context;
    private readonly GaussZeidelSolver _slaeSolver;
    private Spline2DEquationAssembler _2dEquationAssembler;
    private bool _allocated;

    public SmoothingSpline2DCreator(GaussZeidelSolver slaeSolver)
    {
        _slaeSolver = slaeSolver;
    }

    public void Allocate(Grid<Point, Element> grid)
    {
        if (_allocated)
        {
            return;
        }
        _context = CreateContext(grid);
        _2dEquationAssembler = CreateAssembler(_context);
        _slaeSolver.Allocate(grid.Nodes.TotalPoints * 4);
        _allocated = true;
    }

    public ISpline2D<Point> CreateSpline(FuncValue[] funcValues, double[] weights, double alpha)
    {
        _context.FunctionValues = funcValues;
        _context.Weights = weights;
        _context.Alpha = alpha;

        _2dEquationAssembler.BuildEquation(_context);

        var solution = _slaeSolver.Solve(_context.Equation.Matrix, _context.Equation.RightSide);

        for (var i = 0; i < _context.Equation.Solution.Length; i++)
        {
            _context.Equation.Solution[i] = solution[i];
        }

        return new SmoothingSpline2D(_basisFunctionsProvider, _context.Grid, _context.Equation.Solution);
    }

    private SplineContext<Point, Element, Matrix> CreateContext(Grid<Point, Element> grid)
    {
        var context = new SplineContext<Point, Element, Matrix>
        {
            Grid = grid,
            Equation = new Equation<Matrix>(
                Matrix: new Matrix(new double[grid.Nodes.TotalPoints * 4, grid.Nodes.TotalPoints * 4]),
                RightSide: Vector.Create(grid.Nodes.TotalPoints * 4),
                Solution: Vector.Create(grid.Nodes.TotalPoints * 4)
            ),
            FunctionValues = null,
            Weights = null,
            Alpha = 0,
            Beta = 0
        };

        return context;
    }

    private Spline2DEquationAssembler CreateAssembler(SplineContext<Point, Element, Matrix> context)
    {
        _basisFunctionsProvider = new HermiteBasisFunctions2DProvider(context);
        return new Spline2DEquationAssembler(
            context,
            new Spline2DLocalAssembler(_basisFunctionsProvider),
            new HermiteLocalAssembler(context),
            new DenseMatrixInserter()
        );
    }
}