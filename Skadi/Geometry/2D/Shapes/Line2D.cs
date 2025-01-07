using Skadi.Geometry.Shapes.Primitives;

namespace Skadi.Geometry._2D.Shapes;

public class Line2D : IParametricCurve2D
{
    public double Length => GetLength(Start, End);

    public Vector2D Start { get; }
    public Vector2D End { get; }
    
    public Line2D(Vector2D start, Vector2D end)
    {
        Start = start;
        End = end;
    }

    public Vector2D GetByParameter(CurveParameter t)
    {
        return Start + t * (End - Start);
    }

    public static double GetLength(Vector2D start, Vector2D end)
    {
        return (end - start).Length;
    }
}
