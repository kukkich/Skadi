using Skadi.FEM.Core.Geometry;
using Skadi.Matrices;

namespace Skadi.FEM.Core.Assembling;

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