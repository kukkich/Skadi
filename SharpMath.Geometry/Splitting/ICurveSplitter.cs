using SharpMath.Geometry.Shapes;

namespace SharpMath.Geometry.Splitting;

public interface ICurveSplitter
{
    public IEnumerable<TPoint> EnumerateValues<TPoint>(ICurve<TPoint> curve);
}