using Skadi.Geometry._2D;
using Skadi.Geometry.Shapes;
using Skadi.Geometry.Shapes.Primitives;

namespace Skadi.Algorithms.Splines._1D;

public class SplineParametricCurve(ISpline<double> spline, double left, double right) : IParametricCurve<Vector2D>
{
    public Vector2D Start { get; } = new(left, spline.Calculate(left));
    public Vector2D End { get; } = new(right, spline.Calculate(right));

    public Vector2D GetByParameter(CurveParameter t)
    {
        var x = left + (right - left) * t;
        var y = spline.Calculate(x);
        return new Vector2D(x, y);
    }
}