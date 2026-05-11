using Skadi.EquationsSystem;
using Skadi.EquationsSystem.Solver;
using Skadi.FEM._1D.Assembling;
using Skadi.FEM._1D.BasisFunctions;
using Skadi.FEM.Assembling;
using Skadi.FEM.Core.Geometry;
using Skadi.LinearAlgebra.Matrices;
using Skadi.LinearAlgebra.Vectors;

namespace Skadi.Algorithms.Splines._1D.CubicLagrange;

public class LagrangeSplineCreator(GaussZeidelSolver slaeSolver) : ISplineCreator<double, IElement>
{
    private const int InnerNodes = 2;
    private bool _allocated;
    private Grid<double, IElement> _grid;
    private Equation<Matrix> _equation;
    private LagrangeCubicFunction1DProvider _localFunctionsProvider;

    public void Allocate(Grid<double, IElement> grid)
    {
        _grid = grid;
        var equationSize = grid.Nodes.TotalPoints + grid.Elements.Length * InnerNodes;
        _equation = new Equation<Matrix>(
            Matrix: new Matrix(new double[equationSize, equationSize]),
            RightSide: Vector.Create(equationSize),
            Solution: Vector.Create(equationSize)
        );
        slaeSolver.Allocate(equationSize);
        _localFunctionsProvider = new LagrangeCubicFunction1DProvider(_grid);
        
        _allocated = true;
    }

    ISpline<double> ISplineCreator<double, IElement>.CreateSpline(FuncValue<double>[] functionValues, double alpha) 
        => CreateSpline(functionValues, alpha);
    
    public LagrangeSpline CreateSpline(FuncValue<double>[] functionValues, double alpha)
    {
        EnsureAllocated();
        var equationAssembler = new SplineEquationAssembler1D(
            _grid.Nodes,
            new SplineLocalAssembler1D(_localFunctionsProvider),
            new LagrangeCubicAssembler1D(_grid.Nodes, alpha),
            new DenseMatrixInserter()
        );
        _equation.Matrix.Nullify();
        _equation.RightSide.Nullify();
        _equation.Solution.Nullify();
        equationAssembler.BuildEquation(_equation, functionValues, _grid.Elements);
        var solution = slaeSolver.Solve(_equation.Matrix, _equation.RightSide);

        return new LagrangeSpline(_localFunctionsProvider, _grid, solution.Copy());
    }
    
    private void EnsureAllocated()
    {
        if (!_allocated)
        {
            throw new Exception("Not allocated");
        }
    }
}