using SharpMath.FiniteElement._2D.Elements;
using SharpMath.Geometry;
using SharpMath.Geometry._2D;

namespace SharpMath.Splines;

public class SmoothingSplineCreator : ISplineCreator<Point, BicubicFiniteElement>
{
    public ISpline<Point> CreateSpline(Grid<Point, BicubicFiniteElement> grid, FuncValue[] funcValues)
    {
        throw new NotImplementedException();
    }
}