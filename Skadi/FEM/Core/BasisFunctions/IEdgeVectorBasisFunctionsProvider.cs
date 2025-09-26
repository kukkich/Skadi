using Skadi.FEM.Core.Geometry;

namespace Skadi.FEM.Core.BasisFunctions;

public interface IEdgeVectorBasisFunctionsProvider<in TElement, TPoint> 
    where TElement : IEdgeElement
{
    public IVectorBasicFunction<TPoint>[] GetFunctions(TElement element);
}