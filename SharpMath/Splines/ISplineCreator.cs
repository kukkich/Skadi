using SharpMath.FEM.Core;
using SharpMath.Geometry._2D;

namespace SharpMath.Splines;

public interface ISplineCreator<TPoint, TElement> : IAllocationRequired<Grid<TPoint, TElement>> 
    where TElement : IElement
{
    public ISpline<TPoint> CreateSpline(FuncValue<TPoint>[] funcValues, double alpha);
}
