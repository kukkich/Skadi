using Skadi.EquationsSystem;
using Skadi.EquationsSystem.Solver;
using Skadi.FEM._2D.Assembling;
using Skadi.FEM._2D.BasisFunctions;
using Skadi.FEM.Assembling;
using Skadi.FEM.Core.Geometry;
using Skadi.Geometry._2D;
using Skadi.LinearAlgebra.Matrices;
using Skadi.LinearAlgebra.Vectors;

namespace Skadi.Algorithms.Splines._2D.Smooth;

public class SmoothingSplineCreator(GaussZeidelSolver slaeSolver) : ISplineCreator<Vector2D, IElement>
{
    private HermiteBasisFunctions2DProvider _basisFunctionsProvider;
    private SplineContext<Vector2D, IElement, Matrix> _context;
    private SplineEquationAssembler<Vector2D> _equationAssembler;
    private bool _allocated;

    public void Allocate(Grid<Vector2D, IElement> grid)
    {
        if (_allocated)
        {
            return;
        }
        _context = CreateContext(grid);
        slaeSolver.Allocate(grid.Nodes.TotalPoints * 4);
        _allocated = true;
    }

    public ISpline<Vector2D> CreateSpline(FuncValue<Vector2D>[] functionValues, double alpha)
    {
        _equationAssembler = CreateAssembler(_context, alpha);

        _context.FunctionValues = functionValues;
        _context.Weights = CalculateWeights(functionValues);
        _context.Alpha = alpha;

        _equationAssembler.BuildEquation(_context.Equation, _context.FunctionValues, _context.Grid.Elements, _context.Weights);

        var solution = slaeSolver.Solve(_context.Equation.Matrix, _context.Equation.RightSide);

        for (var i = 0; i < _context.Equation.Solution.Count; i++)
        {
            _context.Equation.Solution[i] = solution[i];
        }

        return new SmoothingSpline(_basisFunctionsProvider, _context.Grid, _context.Equation.Solution);
    }

    private SplineContext<Vector2D, IElement, Matrix> CreateContext(Grid<Vector2D, IElement> grid)
    {
        var context = new SplineContext<Vector2D, IElement, Matrix>
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

    private SplineEquationAssembler2D CreateAssembler(SplineContext<Vector2D, IElement, Matrix> context, double alpha)
    {
        _basisFunctionsProvider = new HermiteBasisFunctions2DProvider(context.Grid.Nodes);
        return new SplineEquationAssembler2D
        (
            context.Grid.Nodes,
            new SplineLocalAssembler2D(_basisFunctionsProvider),
            new HermiteLocalAssembler(context.Grid.Nodes, alpha),
            new DenseMatrixInserter()
        );
    }

    private static double[] CalculateWeights(FuncValue<Vector2D>[] funcValues)
    {
        var weights = new double[funcValues.Length];

        for (var i = 0; i < funcValues.Length; i++)
        {
            weights[i] = 1;
        }

        return weights;
    }
}