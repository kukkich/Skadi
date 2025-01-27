using Skadi.Geometry.Shapes.Primitives;

namespace Skadi.Geometry._2D.Shapes;

public class CubicSpline : IParametricCurve2D
{
    private readonly Vector2D[] _points;
    public Vector2D Start => _points[0];
    public Vector2D End => _points[3];

    public CubicSpline(Vector2D[] points)
    {
        if (points.Length != 4)
        {
            throw new ArgumentException("Invalid number of points");
        }
        _points = points;
    }
    
    public Vector2D GetByParameter(CurveParameter t)
    {
        Span<double> tc =
        [
            -9d / 2 * (t - 1d / 3) * (t - 2d / 3) * (t - 1),
            27d / 2 * t * (t - 2d / 3) * (t - 1),
            -27d / 2 * t * (t - 1d / 3) * (t - 1),
            9d / 2 * t * (t - 1d / 3) * (t - 2d / 3),
        ];
        return LinAl.MultiplyAsTransparent<Vector2D>(tc, _points);
    }
}