namespace Skadi.Splines;

public interface ISpline<in TPoint>
{
    public double Calculate(TPoint point);
}