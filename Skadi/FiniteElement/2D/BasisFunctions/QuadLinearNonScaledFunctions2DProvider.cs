using Skadi.FEM.Core;
using Skadi.Geometry._2D;
using Skadi.FiniteElement.Core.BasisFunctions;
using Skadi.FiniteElement.Core.BasisFunctions._2D;

namespace Skadi.FiniteElement._2D.BasisFunctions;

public class QuadLinearNonScaledFunctions2DProvider : IBasisFunctionsProvider<IElement, Point2D>, IBasicFunctionsDerivativeProvider2D
{
    private readonly Func<double, double>[] _functions1D = [
        t => 1 - t,
        t => t
    ];
    private readonly Func<double, double>[] _derivatives1D = [
        _ => -1d,
        _ => 1d
    ];
    
    public IBasisFunction<Point2D>[] GetFunctions(IElement element)
    {
        return
        [
            new BasisFunction2D(_functions1D[0], _functions1D[0]),
            new BasisFunction2D(_functions1D[1], _functions1D[0]),
            new BasisFunction2D(_functions1D[0], _functions1D[1]),
            new BasisFunction2D(_functions1D[1], _functions1D[1]),
        ];
    }

    public IBasisFunction<Point2D>[] GetDerivativeByX(IElement element)
    {
        return
        [
            new BasisFunction2D(_derivatives1D[0], _functions1D[0]),
            new BasisFunction2D(_derivatives1D[1], _functions1D[0]),
            new BasisFunction2D(_derivatives1D[0], _functions1D[1]),
            new BasisFunction2D(_derivatives1D[1], _functions1D[1]),
        ];
    }

    public IBasisFunction<Point2D>[] GetDerivativeByY(IElement element)
    {
        return
        [
            new BasisFunction2D(_functions1D[0], _derivatives1D[0]),
            new BasisFunction2D(_functions1D[1], _derivatives1D[0]),
            new BasisFunction2D(_functions1D[0], _derivatives1D[1]),
            new BasisFunction2D(_functions1D[1], _derivatives1D[1]),
        ];
    }
}