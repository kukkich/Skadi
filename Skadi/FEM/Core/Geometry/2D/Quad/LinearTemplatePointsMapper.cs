using Skadi.Geometry._2D;

namespace Skadi.FEM.Core.Geometry._2D.Quad;

public class LinearTemplatePointsMapper(Vector2D[] areaPoints) : ITemplatePointsMapper<Vector2D>
{
    public Vector2D Map(Vector2D vector)
    {
        Span<double> wx = [1 - vector.X, vector.X];
        Span<double> wy = [1 - vector.Y, vector.Y];

        var (x, y) = (0d, 0d);
        for (var i = 0; i < 4; i++)
        {
            x += wx[i % 2] * wy[i / 2] * areaPoints[i].X;
            y += wx[i % 2] * wy[i / 2] * areaPoints[i].Y;
        }
        
        return new Vector2D(x, y);
    }
}