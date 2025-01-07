using Skadi.FEM.Core.BasisFunctions;
using Skadi.FEM.Core.BasisFunctions._2D;
using Skadi.FEM.Core.Geometry;
using Skadi.Geometry._2D;

namespace Skadi.FEM._2D.BasisFunctions;

public class QuadLinearNonScaledFunctions2DProvider : IBasisFunctionsProvider<IElement, Vector2D>, IBasisFunctionsDerivativeProvider2D
{
    private readonly Func<double, double>[] _functions1D = [
        t => 1 - t,
        t => t
    ];
    private readonly Func<double, double>[] _derivatives1D = [
        _ => -1d,
        _ => 1d
    ];
    
    public IBasisFunction<Vector2D>[] GetFunctions(IElement element)
    {
        return
        [
            new BasisFunction2D(_functions1D[0], _functions1D[0]),
            new BasisFunction2D(_functions1D[1], _functions1D[0]),
            new BasisFunction2D(_functions1D[0], _functions1D[1]),
            new BasisFunction2D(_functions1D[1], _functions1D[1]),
        ];
    }

    public IBasisFunction<Vector2D>[] GetDerivativeByX(IElement element)
    {
        return
        [
            new BasisFunction2D(_derivatives1D[0], _functions1D[0]),
            new BasisFunction2D(_derivatives1D[1], _functions1D[0]),
            new BasisFunction2D(_derivatives1D[0], _functions1D[1]),
            new BasisFunction2D(_derivatives1D[1], _functions1D[1]),
        ];
    }

    public IBasisFunction<Vector2D>[] GetDerivativeByY(IElement element)
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