using SharpMath.Geometry.Shapes;
using SharpMath.Geometry.Shapes.Primitives;

namespace SharpMath.Geometry._2D.Shapes;

public class Line : ICurve<Point2D>
{
    public Point2D Start { get; set; }

    public Point2D End { get; set; }

    public Line(Point2D start, Point2D end)
    {
        Start = start;
        End = end;
    }

    public Point2D GetByParameter(CurveParameter t)
    {
        return Start + t * (End - Start);
    }
}
