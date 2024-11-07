using SharpMath.FEM.Core;
using SharpMath.FiniteElement.Core.Assembling;
using SharpMath.FiniteElement.Core.BasisFunctions;

namespace SharpMath.Splines._1D.CubicLagrange;

public class SplineLocalAssembler1D : SplineLocalAssembler<double>
{
    public SplineLocalAssembler1D(IBasisFunctionsProvider<IElement, double> basisFunctionsProvider) 
        : base(basisFunctionsProvider)
    {
    }

    protected override void FillIndexes(IElement element, StackIndexPermutation indexes)
    {
        var leftNodeId = element.NodeIds[0];
        for (var i = 0; i < 4; i++)
        {
            indexes.Permutation[i] = leftNodeId * 3 + i;
        }
    }
}