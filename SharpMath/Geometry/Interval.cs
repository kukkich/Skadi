namespace SharpMath.Geometry;

public readonly struct Interval
{
    public const double MaxAccuracy = 1e-15;
    public double Begin { get; }
    public double End { get; }

    public double Length => End - Begin;

    public Interval(double begin, double end)
    {
        Begin = begin;
        End = end;
    }

    public bool Has(double value)
    {
        return value >= Begin && value <= End + MaxAccuracy;
    }
}