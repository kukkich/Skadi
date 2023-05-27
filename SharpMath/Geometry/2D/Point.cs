namespace SharpMath.Geometry._2D;

public record struct Point(double X, double Y)
{
    public static Point operator /(Point point, double coefficient) =>
        new Point(point.X / coefficient, point.Y / coefficient);
}