using Skadi.FEM.Core.Assembling;
using Skadi.FEM.Core.BasisFunctions;
using Skadi.FEM.Core.Geometry;
using Skadi.Geometry._2D;

namespace Skadi.Algorithms.Splines._2D.Smooth;

public class SplineLocalAssembler2D : SplineLocalAssembler<Vector2D>
{
    public SplineLocalAssembler2D(IBasisFunctionsProvider<IElement, Vector2D> basisFunctionsProvider)
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