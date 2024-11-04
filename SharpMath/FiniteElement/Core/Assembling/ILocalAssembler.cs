using SharpMath.FEM.Core;
using SharpMath.Matrices;

namespace SharpMath.FiniteElement.Core.Assembling;

public interface ILocalAssembler<in TElement>
    where TElement : IElement
{
    public LocalMatrix AssembleMatrix(TElement element);
    public LocalVector AssembleRightSide(TElement element);
}

public interface IStackLocalAssembler<in TElement>
    where TElement : IElement
{
    public void AssembleMatrix(TElement element, StackMatrix matrix, StackIndexPermutation indexes);
    public void AssembleRightSide(TElement element, Span<double> vector, StackIndexPermutation indexes);
}