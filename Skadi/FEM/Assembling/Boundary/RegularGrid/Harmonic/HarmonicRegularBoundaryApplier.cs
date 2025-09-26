using System.Linq.Expressions;
using System.Numerics;
using Skadi.EquationsSystem;
using Skadi.FEM.Core.Assembling.Boundary;
using Skadi.FEM.Core.Assembling.Boundary.First;
using Skadi.FEM.Core.Assembling.Boundary.Second.Harmonic;
using Skadi.FEM.Core.Assembling.Params;
using Skadi.FEM.Core.Geometry;
using Skadi.Geometry._2D;

namespace Skadi.FEM.Assembling.Boundary.RegularGrid.Harmonic;

public class HarmonicRegularBoundaryApplier<TMatrix>(
    IFirstBoundaryApplier<TMatrix> firstBoundaryApplier,
    IHarmonicSecondBoundaryApplier<TMatrix> secondBoundaryApplier,
    IExpressionProvider expressionProvider,
    IPointsCollection<Vector2D> nodes,
    IBoundIndexesEvaluator boundIndexesEvaluator
    ) : IRegularBoundaryApplier<TMatrix>
{
    public void Apply(Equation<TMatrix> equation, RegularBoundaryCondition condition)
    {
        if (condition.Type is BoundaryConditionType.None)
        {
            return;
        }

        if (condition.Type == BoundaryConditionType.First)
        {
            var expression = (Expression<Func<Vector2D, Complex>>) expressionProvider.GetExpression(condition.ExpressionId);
            var func = expression.Compile();

            foreach (var nodeId in boundIndexesEvaluator.EnumerateNodes(condition))
            {
                var node = nodes[nodeId];
                var value = func(node);
                firstBoundaryApplier.Apply(equation, new FirstCondition(nodeId * 2, value.Real));
                firstBoundaryApplier.Apply(equation, new FirstCondition(nodeId * 2 + 1, value.Imaginary));
            }
        }
        else if (condition.Type == BoundaryConditionType.Second)
        {
            var expression = (Expression<Func<Vector2D, Complex>>) expressionProvider.GetExpression(condition.ExpressionId);
            var func = expression.Compile();
            var thetta = new Complex[2];
            
            foreach (var edge in boundIndexesEvaluator.EnumerateEdges(condition))
            {
                thetta[0] = func(nodes[edge.Begin]);
                thetta[1] = func(nodes[edge.End]);
                
                secondBoundaryApplier.Apply(equation, new HarmonicSecondBoundary(edge, thetta));
            }
        }
        else if (condition.Type == BoundaryConditionType.Third)
        {
            throw new NotImplementedException();
        }
        else
        {
            throw new ArgumentException("Unknown boundary condition type");
        }
    }
}