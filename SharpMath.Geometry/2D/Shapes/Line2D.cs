using SharpMath.Geometry.Shapes.Primitives;

namespace SharpMath.Geometry._2D.Shapes;

public class Line2D : ICurve2D
{
    public Point2D Start { get; }
    public Point2D End { get; }

    public Line2D(Point2D start, Point2D end)
    {
        Start = start;
        End = end;
    }

    public Point2D GetByParameter(CurveParameter t)
    {
        return Start + t * (End - Start);
    }
}
