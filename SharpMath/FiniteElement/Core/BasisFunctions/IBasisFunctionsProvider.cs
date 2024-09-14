using SharpMath.FiniteElement.Materials;

namespace SharpMath.FiniteElement.Core.BasisFunctions;

public interface IBasisFunctionsProvider<in TPoint>
{
    public IBasisFunction<TPoint> GetFunctions(IFiniteElement element);
}