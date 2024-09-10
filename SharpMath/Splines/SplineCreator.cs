using SharpMath.Geometry;
using SharpMath.Geometry._2D;

namespace SharpMath.Splines;

public interface ISplineCreator<TPoint, TElement>
{
    // public ISpline<TPoint> CreateSpline(Grid<TPoint, TElement> grid, FuncValue[] funcValues);
    public ISpline<TPoint> CreateSpline(Grid<TPoint, TElement> grid, FuncSmoothValue[] funcValues);
}

public readonly record struct FuncValue(Point Point, double Value);
public readonly record struct FuncSmoothValue(Point Point, double Value, double Derivative);
