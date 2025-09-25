using Skadi.Geometry._2D;
using Skadi.Geometry.Shapes;
using Skadi.Geometry.Shapes.Primitives;

namespace Skadi.Algorithms.Splines._1D;

public class SplineParametricCurve : IParametricCurve<Vector2D>
{
    public Vector2D Start { get; }
    public Vector2D End { get; }

    private readonly ISpline<double> _spline;
    private readonly double _left;
    private readonly double _right;
    
    public SplineParametricCurve(ISpline<double> spline, double left, double right)
    {
        _spline = spline;
        _left = left;
        _right = right;
        Start = new Vector2D(left, spline.Calculate(left));
        End = new Vector2D(right, spline.Calculate(right));
    }
    
    public Vector2D GetByParameter(CurveParameter t)
    {
        var x = _left + (_right - _left) * t;
        var y = _spline.Calculate(x);
        return new Vector2D(x, y);
    }
}