using Skadi.FEM.Core.Geometry;
using Skadi.Geometry._3D;

namespace Skadi.FEM.Core.BasisFunctions;

public interface IEdgeVectorBasisFunctionsProvider<in TElement, TPoint> 
    where TElement : IEdgeElement
{
    public IVectorBasicFunction<TPoint>[] GetFunctions(TElement element);
}