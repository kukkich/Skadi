using Skadi.FEM.Core;
using Skadi.EquationsSystem.Solver;
using Skadi.FiniteElement._1D.Assembling;
using Skadi.FiniteElement._1D.BasisFunctions;
using Skadi.FiniteElement._2D.Assembling;
using Skadi.Matrices;
using Skadi.Vectors;

namespace Skadi.Splines._1D.CubicLagrange;

public class LagrangeSplineCreator : ISplineCreator<double, IElement>
{
    private const int InnerNodes = 2;
    private bool _allocated;
    private readonly GaussZeidelSolver _slaeSolver;
    private Grid<double, IElement> _grid;
    private Equation<Matrix> _equation;

    public LagrangeSplineCreator(GaussZeidelSolver slaeSolver)
    {
        _slaeSolver = slaeSolver;
    }
    
    public void Allocate(Grid<double, IElement> grid)
    {
        if (_allocated)
        {
            return;
        }

        _grid = grid;
        var equationSize = grid.Nodes.TotalPoints + grid.Elements.Length * InnerNodes;
        _equation = new Equation<Matrix>(
            Matrix: new Matrix(new double[equationSize, equationSize]),
            RightSide: Vector.Create(equationSize),
            Solution: Vector.Create(equationSize)
        );
        _slaeSolver.Allocate(equationSize);
        _allocated = true;
    }

    public ISpline<double> CreateSpline(FuncValue<double>[] functionValues, double alpha)
    {
        EnsureAllocated();
        var localFunctionsProvider = new LagrangeCubicFunction1DProvider(_grid);
        var equationAssembler = new SplineEquationAssembler1D(
            _grid.Nodes,
            new SplineLocalAssembler1D(localFunctionsProvider),
            new LagrangeCubicAssembler1D(_grid.Nodes, alpha),
            new DenseMatrixInserter()
        );
        equationAssembler.BuildEquation(_equation, functionValues, _grid.Elements);
        var solution = _slaeSolver.Solve(_equation.Matrix, _equation.RightSide);

        return new LagrangeSpline(localFunctionsProvider, _grid, solution);
    }

    private void EnsureAllocated()
    {
        if (!_allocated)
        {
            throw new Exception("Not allocated");
        }
    }
}