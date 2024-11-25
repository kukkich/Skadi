using SharpMath.Geometry;
using SharpMath.Geometry._2D;

namespace SharpMath.Splines;

public interface ISplineCreator<out TSpline, TPoint, TElement> : IAllocationRequired<Grid<TPoint, TElement>>
{
    public TSpline CreateSpline(FuncValue[] funcValues, double[] weights, double alpha);
}

public record struct FuncValue(Point Point, double Value);