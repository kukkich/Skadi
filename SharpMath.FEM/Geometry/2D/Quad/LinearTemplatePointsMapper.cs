using SharpMath.Geometry._2D;

namespace SharpMath.FEM.Geometry._2D.Quad;

public class LinearTemplatePointsMapper(Point2D[] areaPoints) : ITemplatePointsMapper<Point2D>
{
    public Point2D Map(Point2D point)
    {
        Span<double> wx = [1 - point.X, point.X];
        Span<double> wy = [1 - point.Y, point.Y];

        var (x, y) = (0d, 0d);
        for (var i = 0; i < 4; i++)
        {
            x += wx[i % 2] * wy[i / 2] * areaPoints[i].X;
            y += wx[i % 2] * wy[i / 2] * areaPoints[i].Y;
        }
        
        return new Point2D(x, y);
    }
}