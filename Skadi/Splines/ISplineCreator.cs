using Skadi.FEM.Core;
using Skadi.Geometry._2D;

namespace Skadi.Splines;

public interface ISplineCreator<TPoint, TElement> : IAllocationRequired<Grid<TPoint, TElement>> 
    where TElement : IElement
{
    public ISpline<TPoint> CreateSpline(FuncValue<TPoint>[] functionValues, double alpha);
}
