using Skadi.FEM.Core;
using Skadi.Geometry._2D;

namespace Skadi.FiniteElement.Core.BasisFunctions._2D;

public interface IBasicFunctionsDerivativeProvider2D
{
    public IBasisFunction<Point2D>[] GetDerivativeByX(IElement element);
    public IBasisFunction<Point2D>[] GetDerivativeByY(IElement element);
}