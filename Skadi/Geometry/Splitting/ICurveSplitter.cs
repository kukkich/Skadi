using Skadi.Geometry.Shapes;

namespace Skadi.Geometry.Splitting;

public interface ICurveSplitter
{
    public int Steps { get; }
    public IEnumerable<TPoint> EnumerateValues<TPoint>(ICurve<TPoint> curve);
}