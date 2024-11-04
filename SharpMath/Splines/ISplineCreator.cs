using SharpMath.FEM.Core;
using SharpMath.Geometry._2D;

namespace SharpMath.Splines;

public interface ISplineCreator<TPoint, TElement> : IAllocationRequired<Grid<TPoint, TElement>> 
    where TElement : IElement
{
    public ISpline<TPoint> CreateSpline(FuncValue[] funcValues, double alpha);
}

public readonly record struct FuncValue(Point2D Point, double Value);