using Skadi.EquationsSystem;
using Skadi.FEM.Core.Geometry;
using Skadi.Geometry._2D;
using Skadi.Geometry._2D.Shapes;
using Skadi.LinearAlgebra;
using Skadi.LinearAlgebra.Matrices;

namespace Skadi.FEM.Core.Assembling.Boundary.Second;

public class SecondBoundaryApplier<TMatrix>(
    IPointsCollection<Vector2D> nodes,
    IInserter<TMatrix> inserter
    ) : ISecondBoundaryApplier<TMatrix>
{
    public void Apply(Equation<TMatrix> equation, SecondBoundary condition)
    {
        var edge = condition.Edge;
        var edgeLength = Line2D.GetLength(nodes[condition.Edge.Begin], nodes[condition.Edge.End]);
        
        var defaultMass = new MatrixSpan([
            2, 1,
            1, 2
        ], 2);

        var massCoef = edgeLength / 6d;

        MatrixOps.Scale(massCoef, defaultMass, defaultMass);

        Span<double> conditionImpact = stackalloc double[2];
        MatrixOps.Multiply(defaultMass, condition.Thetta, conditionImpact);
        var local = new StackLocalVector(conditionImpact, new StackIndexPermutation([edge.Begin, edge.End]));
        inserter.InsertVector(equation.RightSide, local);
    }
}