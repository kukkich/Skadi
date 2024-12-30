using SharpMath.FEM.Core;
using SharpMath.Geometry._2D;

namespace SharpMath.FiniteElement.Core.BasisFunctions._2D;

public interface IBasicFunctionsDerivativeProvider2D
{
    public IBasisFunction<Point2D>[] GetDerivativeByX(IElement element);
    public IBasisFunction<Point2D>[] GetDerivativeByY(IElement element);
}