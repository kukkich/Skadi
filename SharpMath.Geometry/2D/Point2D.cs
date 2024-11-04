namespace SharpMath.Geometry._2D;

public record struct Point2D(double X, double Y)
{
    public static Point2D operator +(Point2D p1, Point2D p2)
    {
        return new Point2D(p1.X + p2.X, p1.Y + p2.Y);
    }

    public static Point2D operator -(Point2D p1, Point2D p2)
    {
        return new Point2D(p1.X - p2.X, p1.Y - p2.Y);
    }

    public static Point2D operator *(double coefficient, Point2D p)
    {
        return new Point2D(coefficient * p.X, coefficient * p.Y);
    }
    
    public static Point2D operator *(Point2D p, double coefficient)
    {
        return coefficient * p;
    }
}