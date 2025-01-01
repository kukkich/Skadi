using Skadi.FEM.Core;

namespace Skadi.FiniteElement.Core.BasisFunctions;

public interface IBasisFunctionsProvider<in TElement, in TPoint> 
    where TElement : IElement
{
    public IBasisFunction<TPoint>[] GetFunctions(TElement element);
}