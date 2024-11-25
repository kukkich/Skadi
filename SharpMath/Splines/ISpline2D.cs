namespace SharpMath.Splines;

public interface ISpline2D<in TPoint> : ISpline<TPoint>
{
    public double CalculateDerivativeByX(TPoint point);
    public double CalculateDerivativeByY(TPoint point);
}