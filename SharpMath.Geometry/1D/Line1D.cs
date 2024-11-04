using SharpMath.Geometry.Shapes.Primitives;

namespace SharpMath.Geometry._1D;

public class Line1D : ICurve1D
{
    public double Start { get; }
    public double End { get; }

    public Line1D(double start, double end)
    {
        Start = start;
        End = end;
    }

    public double GetByParameter(CurveParameter t)
    {
        return Start + t * (End - Start);
    }
}