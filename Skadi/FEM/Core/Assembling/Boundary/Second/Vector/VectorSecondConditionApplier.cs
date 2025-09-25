using Skadi.EquationsSystem;
using Skadi.FEM.Core.Geometry;
using Skadi.FEM.Core.Geometry.Edges;
using Skadi.Geometry._2D;
using Skadi.Geometry._2D.Shapes;
using Skadi.LinearAlgebra;
using Skadi.LinearAlgebra.Matrices;

namespace Skadi.FEM.Core.Assembling.Boundary.Second.Vector;

public class VectorSecondConditionApplier<TMatrix>(
    Grid<Vector2D, IElement> grid,
    IInserter<TMatrix> inserter,
    IEdgeResolver edgeResolver)
    : IVectorSecondConditionApplier<TMatrix>
{
    private readonly IInserter<TMatrix> _inserter = inserter;

    public void Apply(Equation<TMatrix> equation, SecondBoundary condition)
    {
        throw new NotImplementedException();
        var edgeId = edgeResolver.GetEdgeId(condition.Edge);
        var edge = new Line2D(grid.Nodes[condition.Edge.Begin], grid.Nodes[condition.Edge.End]);
        var elementId = edgeResolver.GetElementsByEdgeId(edgeId).Single();
        var nodeIds = grid.Elements[elementId].NodeIds;
        int[] clockwiseNodeIds = [nodeIds[0], nodeIds[1], nodeIds[3], nodeIds[2]];
        Vector2D[] clockwiseNodes = [..clockwiseNodeIds.Select(x => grid.Nodes[x])];

        var begin = clockwiseNodeIds[0];
        for (var i = 1; i < clockwiseNodeIds.Length; i++)
        {
            var end = clockwiseNodeIds[i];
            if (new Edge(begin, end) == condition.Edge)
            {
                
            }
            
        }
        
        
        var direction = edge.End - edge.Start;
        var normal = direction.Orthogonal.Normalize();
        var biNormal = normal.Negate();
        
        var defaultMass = new MatrixSpan([
            2, 1,
            1, 2
        ], 2);

        var massCoef = edge.Length / 6d;

        LinAl.Multiply(massCoef, defaultMass, defaultMass);

        // var conditionImpact = LinAl.Multiply(defaultMass, condition.Thetta, stackalloc double[2]);
        // var local = new StackLocalVector(conditionImpact, new StackIndexPermutation([edge.Begin, edge.End]));
        // _inserter.InsertVector(equation.RightSide, local);
    }
}