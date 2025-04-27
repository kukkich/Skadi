using Skadi.FEM.Core.Assembling;
using Skadi.FEM.Core.Geometry;
using Skadi.Matrices;

namespace Skadi.FEM._2D.Assembling;

public class VectorLinearLocalAssembler : IStackLocalAssembler<IEdgeElement>
{
    public void AssembleMatrix(IEdgeElement element, MatrixSpan matrixSpan, StackIndexPermutation indexes)
    {
        throw new NotImplementedException();
    }

    public void AssembleRightSide(IEdgeElement element, Span<double> vector, StackIndexPermutation indexes)
    {
        throw new NotImplementedException();
    }
}