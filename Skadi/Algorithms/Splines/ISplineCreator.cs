using Skadi.FEM.Core.Geometry;

namespace Skadi.Algorithms.Splines;

public interface ISplineCreator<TPoint, TElement> : IAllocationRequired<Grid<TPoint, TElement>> 
    where TElement : IElement
{
    public ISpline<TPoint> CreateSpline(FuncValue<TPoint>[] functionValues, double alpha);
}
