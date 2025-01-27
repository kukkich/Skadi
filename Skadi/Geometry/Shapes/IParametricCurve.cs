using Skadi.Geometry.Shapes.Primitives;

namespace Skadi.Geometry.Shapes;

public interface IParametricCurve<out TPoint>
{
    public TPoint Start { get; }
    public TPoint End { get; }

    public TPoint GetByParameter(CurveParameter t);
}
