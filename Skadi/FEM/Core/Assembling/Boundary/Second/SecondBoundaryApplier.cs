using Skadi.FEM.Core.Geometry;
using Skadi.Geometry._2D;
using Skadi.Geometry._2D.Shapes;
using Skadi.Matrices;

namespace Skadi.FEM.Core.Assembling.Boundary.Second;

public class SecondBoundaryApplier<TMatrix> : ISecondBoundaryApplier<TMatrix>
{
    private readonly IPointsCollection<Vector2D> _nodes;
    private readonly IStackInserter<TMatrix> _inserter;

    public SecondBoundaryApplier(
        IPointsCollection<Vector2D> nodes,
        IStackInserter<TMatrix> inserter
    )
    {
        _nodes = nodes;
        _inserter = inserter;
    }

    public void Apply(Equation<TMatrix> equation, SecondBoundary condition)
    {
        var edge = condition.Edge;
        var edgeLength = Line2D.GetLength(_nodes[condition.Edge.Begin], _nodes[condition.Edge.End]);
        
        var defaultMass = new MatrixSpan([
            2, 1,
            1, 2
        ], 2);

        var massCoef = edgeLength / 6d;

        LinAl.Multiply(massCoef, defaultMass, defaultMass);

        var conditionImpact = LinAl.Multiply(defaultMass, condition.Thetta, stackalloc double[2]);
        var local = new StackLocalVector(conditionImpact, new StackIndexPermutation([edge.Begin, edge.End]));
        _inserter.InsertVector(equation.RightSide, local);
    }
}