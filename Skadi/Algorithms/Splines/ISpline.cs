namespace Skadi.Algorithms.Splines;

public interface ISpline<in TPoint>
{
    public double Calculate(TPoint point);
}