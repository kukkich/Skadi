using Skadi.Geometry.Shapes.Primitives;

namespace Skadi.Geometry._1D;

public readonly record struct Line1D(double Start, double End) : IParametricCurve1D
{
    public static Line1D Unit { get; } = new(0, 1);

    public double Length => End - Start;
    
    public double GetByParameter(CurveParameter t)
    {
        return Start + t * (End - Start);
    }
}