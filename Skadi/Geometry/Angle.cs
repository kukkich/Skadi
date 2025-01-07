namespace Skadi.Geometry;

public readonly record struct Angle(double Radians) : IComparable<Angle>
{
    public static Angle FromDegrees(double value) => new(value * DegToRad);

    public static readonly Angle Zero = new(0);
    public static readonly Angle HalfPi = new(Math.PI / 2);
    public static readonly Angle Pi = new(Math.PI);
    public static readonly Angle TwoPi = new(2 * Math.PI);
    
    private const double RadToDeg = 180.0 / Math.PI;
    private const double DegToRad = Math.PI / 180.0;

    public double Degrees => Radians * RadToDeg;
    public double Cos => Math.Cos(Radians);
    public double Sin => Math.Sin(Radians);
    public double Tan => Math.Tan(Radians);

    public Angle Abs() => new(Math.Abs(Radians));
    public static Angle Acos(double d) => new(Math.Acos(d));
    public static Angle Asin(double d) => new(Math.Asin(d));
    public static Angle Atan(double d) => new(Math.Atan(d));
    public static Angle Atan2(double y, double x) => new(Math.Atan2(y, x));

    public static bool operator <(Angle left, Angle right) => left.Radians < right.Radians;
    public static bool operator >(Angle left, Angle right) => left.Radians > right.Radians;
    public static bool operator <=(Angle left, Angle right) => left.Radians <= right.Radians;
    public static bool operator >=(Angle left, Angle right) => left.Radians >= right.Radians;

    public static Angle operator *(double left, Angle right) => new(left * right.Radians);
    public static Angle operator *(Angle left, double right) => new(left.Radians * right);
    public static Angle operator /(Angle left, double right) => new(left.Radians / right);
    public static Angle operator +(Angle left, Angle right) => new(left.Radians + right.Radians);
    public static Angle operator -(Angle left, Angle right) => new(left.Radians - right.Radians);
    public static Angle operator -(Angle angle) => new(-1 * angle.Radians);

    public int CompareTo(Angle value) => Radians.CompareTo(value.Radians);

    public bool Equals(Angle other, double tolerance) => Math.Abs(Radians - other.Radians) < tolerance;
    public bool Equals(Angle other, Angle tolerance) => Math.Abs(Radians - other.Radians) < tolerance.Radians;
}