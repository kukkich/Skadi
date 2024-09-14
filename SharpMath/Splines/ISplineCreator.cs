using SharpMath.Geometry;
using SharpMath.Geometry._2D;

namespace SharpMath.Splines;

public interface ISplineCreator<TPoint, TElement>
{
    public ISpline<TPoint> CreateSpline(Grid<TPoint, TElement> grid, FuncValue[] funcValues);
}

public readonly record struct FuncValue(Point Point, double Value);