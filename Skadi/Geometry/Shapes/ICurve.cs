using Skadi.Geometry.Shapes.Primitives;

namespace Skadi.Geometry.Shapes;

public interface ICurve<out TPoint>
{
    public TPoint Start { get; }
    public TPoint End { get; }

    public TPoint GetByParameter(CurveParameter t);
}
