using Skadi.Primitives;
using Skadi.Geometry.Shapes;

namespace Skadi.Geometry.Splitting;

public interface ICurveSplitter
{
    public Positive<int> Steps { get; }
    public IEnumerable<TPoint> EnumerateValues<TPoint>(ICurve<TPoint> curve);
}