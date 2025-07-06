using Skadi.Geometry.Shapes.Primitives;

namespace Skadi.Geometry._2D.Shapes;

public readonly record struct Line2D(Vector2D Start, Vector2D End) : IParametricCurve2D
{
    public double Length => GetLength(Start, End);

    public Vector2D GetByParameter(CurveParameter t)
    {
        return Start + t * (End - Start);
    }

    public static double GetLength(Vector2D start, Vector2D end)
    {
        return (end - start).Length;
    }
}
