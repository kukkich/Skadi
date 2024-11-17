using SharpMath.FEM.Core;
using SharpMath.FiniteElement.Core.Assembling;

namespace SharpMath.FiniteElement._2D.Assembling;

public class QuadLinearAssembler2D : ILocalAssembler<IElement>
{
    public LocalMatrix AssembleMatrix(IElement element)
    {
        throw new NotImplementedException();
    }

    public LocalVector AssembleRightSide(IElement element)
    {
        throw new NotImplementedException();
    }
}
