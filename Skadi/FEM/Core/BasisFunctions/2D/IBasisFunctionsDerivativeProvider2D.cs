using Skadi.FEM.Core.Geometry;
using Skadi.Geometry._2D;

namespace Skadi.FEM.Core.BasisFunctions._2D;

public interface IBasisFunctionsDerivativeProvider2D
{
    public IBasisFunction<Vector2D>[] GetDerivativeByX(IElement element);
    public IBasisFunction<Vector2D>[] GetDerivativeByY(IElement element);
}