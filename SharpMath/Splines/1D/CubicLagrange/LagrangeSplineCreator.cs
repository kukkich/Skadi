using SharpMath.FEM.Core;

namespace SharpMath.Splines._1D.CubicLagrange;

public class LagrangeSplineCreator : ISplineCreator<double, IElement>
{
    
    public void Allocate(Grid<double, IElement> param)
    {
        throw new NotImplementedException();
    }

    public ISpline<double> CreateSpline(FuncValue<double>[] funcValues, double alpha)
    {
        throw new NotImplementedException();
    }
}