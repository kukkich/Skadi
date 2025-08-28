using Skadi.Geometry._2D.Shapes;

namespace Skadi.Geometry._2D;

public static class Extensions
{
    public static Rectangle GetBoundingBox(this IEnumerable<Vector2D> points)
    {
        ArgumentNullException.ThrowIfNull(points);

        using var enumerator = points.GetEnumerator();
        if (!enumerator.MoveNext())
            throw new ArgumentException("Sequence contains no elements", nameof(points));

        var minX = enumerator.Current.X;
        var maxX = enumerator.Current.X;
        var minY = enumerator.Current.Y;
        var maxY = enumerator.Current.Y;

        while (enumerator.MoveNext())
        {
            var p = enumerator.Current;
            if (p.X < minX) minX = p.X;
            if (p.X > maxX) maxX = p.X;
            if (p.Y < minY) minY = p.Y;
            if (p.Y > maxY) maxY = p.Y;
        }

        return new Rectangle(minX, maxX, minY, maxY);
    }
}