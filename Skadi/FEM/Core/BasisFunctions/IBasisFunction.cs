namespace Skadi.FEM.Core.BasisFunctions;

public interface IBasisFunction<in TPoint>
{
    public double Evaluate(TPoint point);
}