namespace SharpMath.Geometry.Shapes.Primitives;

public readonly struct CurveParameter
{
    public double Value { get; }

    public CurveParameter(double value)
    {
        if (value is < 0 or > 1d)
        {
            throw new ArgumentException("Curve value must be from 0 to 1", nameof(value));
        }
        Value = value;
    }

    public static implicit operator double(CurveParameter parameter)
    {
        return parameter.Value;
    }

    public static implicit operator CurveParameter(double value)
    {
        return new CurveParameter(value);
    }
}