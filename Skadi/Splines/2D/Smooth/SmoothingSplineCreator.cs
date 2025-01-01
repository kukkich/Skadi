using Skadi.FEM.Core;
using Skadi.Geometry._2D;
using Skadi.EquationsSystem.Solver;
using Skadi.FEM._2D.Assembling;
using Skadi.FEM._2D.BasisFunctions;
using Skadi.FEM.Assembling;
using Skadi.FEM.Core.Geometry;
using Skadi.Matrices;
using Skadi.Vectors;

namespace Skadi.Splines._2D.Smooth;

public class SmoothingSplineCreator : ISplineCreator<Point2D, IElement>
{
    private HermiteBasisFunctions2DProvider _basisFunctionsProvider;
    private SplineContext<Point2D, IElement, Matrix> _context;
    private readonly GaussZeidelSolver _slaeSolver;
    private SplineEquationAssembler<Point2D> _equationAssembler;
    private bool _allocated;

    public SmoothingSplineCreator(GaussZeidelSolver slaeSolver)
    {
        _slaeSolver = slaeSolver;
    }

    public void Allocate(Grid<Point2D, IElement> grid)
    {
        if (_allocated)
        {
            return;
        }
        _context = CreateContext(grid);
        _slaeSolver.Allocate(grid.Nodes.TotalPoints * 4);
        _allocated = true;
    }

    public ISpline<Point2D> CreateSpline(FuncValue<Point2D>[] functionValues, double alpha)
    {
        _equationAssembler = CreateAssembler(_context, alpha);

        _context.FunctionValues = functionValues;
        _context.Weights = CalculateWeights(functionValues);
        _context.Alpha = alpha;

        _equationAssembler.BuildEquation(_context.Equation, _context.FunctionValues, _context.Grid.Elements, _context.Weights);

        var solution = _slaeSolver.Solve(_context.Equation.Matrix, _context.Equation.RightSide);

        for (var i = 0; i < _context.Equation.Solution.Length; i++)
        {
            _context.Equation.Solution[i] = solution[i];
        }

        return new SmoothingSpline(_basisFunctionsProvider, _context.Grid, _context.Equation.Solution);
    }

    private SplineContext<Point2D, IElement, Matrix> CreateContext(Grid<Point2D, IElement> grid)
    {
        var context = new SplineContext<Point2D, IElement, Matrix>
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

    private SplineEquationAssembler2D CreateAssembler(SplineContext<Point2D, IElement, Matrix> context, double alpha)
    {
        _basisFunctionsProvider = new HermiteBasisFunctions2DProvider(context);
        return new SplineEquationAssembler2D(
            context.Grid.Nodes,
            new SplineLocalAssembler2D(_basisFunctionsProvider),
            new HermiteLocalAssembler(context.Grid.Nodes, alpha),
            new DenseMatrixInserter()
        );
    }

    private static double[] CalculateWeights(FuncValue<Point2D>[] funcValues)
    {
        var weights = new double[funcValues.Length];

        for (var i = 0; i < funcValues.Length; i++)
        {
            weights[i] = 1;
        }

        return weights;
    }
}