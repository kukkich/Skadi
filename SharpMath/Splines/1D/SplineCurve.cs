using SharpMath.Geometry._2D;
using SharpMath.Geometry.Shapes;
using SharpMath.Geometry.Shapes.Primitives;

namespace SharpMath.Splines._1D;

public class SplineCurve : ICurve<Point2D>
{
    public Point2D Start { get; }
    public Point2D End { get; }

    private readonly ISpline<double> _spline;
    private readonly double _left;
    private readonly double _right;
    
    public SplineCurve(ISpline<double> spline, double left, double right)
    {
        _spline = spline;
        _left = left;
        _right = right;
        Start = new Point2D(left, spline.Calculate(left));
        End = new Point2D(right, spline.Calculate(right));
    }
    
    public Point2D GetByParameter(CurveParameter t)
    {
        var x = _left + (_right - _left) * t;
        var y = _spline.Calculate(x);
        return new Point2D(x, y);
    }
}