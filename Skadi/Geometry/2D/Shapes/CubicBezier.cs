using Skadi.Geometry.Shapes.Primitives;
using Skadi.LinearAlgebra;

namespace Skadi.Geometry._2D.Shapes;

public readonly record struct CubicBezier : IParametricCurve2D
{
    private readonly Vector2D[] _points;
    public Vector2D Start => _points[0];
    public Vector2D End => _points[3];

    public CubicBezier(Vector2D[] points)
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
            Math.Pow(1 - t, 3),
            3 * t * Math.Pow(1 - t, 2),
            3 * Math.Pow(t, 2),
            Math.Pow(t, 3),
        ];
        return LinAl.MultiplyAsTransparent<Vector2D>(tc, _points);
    }
}