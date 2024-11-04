using SharpMath.FEM.Core;

namespace SharpMath.FiniteElement.Core.BasisFunctions;

public interface IBasisFunctionsProvider<in TElement, in TPoint> 
    where TElement : IElement
{
    public IBasisFunction<TPoint>[] GetFunctions(TElement element);
}