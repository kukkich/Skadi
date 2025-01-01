using Skadi.FEM.Core;
using Skadi.Geometry._2D;
using Skadi.FiniteElement.Core.Assembling;
using Skadi.FiniteElement.Core.BasisFunctions;

namespace Skadi.Splines._2D.Smooth;

public class SplineLocalAssembler2D : SplineLocalAssembler<Point2D>
{
    public SplineLocalAssembler2D(IBasisFunctionsProvider<IElement, Point2D> basisFunctionsProvider)
        : base(basisFunctionsProvider)
    {
    }

    protected override void FillIndexes(IElement element, StackIndexPermutation indexes)
    {
        var nodes = element.NodeIds.Count;
        for (var i = 0; i < nodes; i++)
        {
            for (var j = 0; j < nodes; j++)
            {
                indexes.Permutation[i * nodes + j] = element.NodeIds[i] * nodes + j;
            }
        }
    }
}