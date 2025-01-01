using Skadi.Geometry.Shapes.Primitives;

namespace Skadi.Geometry._2D.Shapes;

public class Line2D : ICurve2D
{
    public double Length => GetLength(Start, End);

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

    public static double GetLength(Point2D start, Point2D end)
    {
        return Math.Sqrt(Math.Pow(end.X - start.X, 2) + Math.Pow(end.Y - start.Y, 2));
    }
}
