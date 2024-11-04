using SharpMath.Geometry.Shapes.Primitives;

namespace SharpMath.Geometry.Shapes;

public interface ICurve<out TPoint>
{
    public TPoint Start { get; }
    public TPoint End { get; }

    public TPoint GetByParameter(CurveParameter t);
}
