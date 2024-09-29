using SharpMath.Geometry.Shapes;
using SharpMath.Primitives;

namespace SharpMath.Geometry.Splitting;

public interface ICurveSplitter
{
    public Positive<int> Steps { get; }
    public IEnumerable<TPoint> EnumerateValues<TPoint>(ICurve<TPoint> curve);
}