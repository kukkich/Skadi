using Skadi.Matrices;
using Skadi.Vectors;

namespace Skadi.Geometry._2D;

public readonly record struct Vector2D(double X, double Y)
{
    public static Vector2D Zero => new(0, 0);
    public static Vector2D XAxis => new(1, 0);
    public static Vector2D YAxis => new(0, 1);

    public double Length => Math.Sqrt(X * X + Y * Y);

    public Vector2D Orthogonal => new(-Y, X);

    public static Vector2D operator +(Vector2D left, Vector2D right) => left.Add(right);

    public static Vector2D operator -(Vector2D left, Vector2D right) => left.Subtract(right);

    public static Vector2D operator -(Vector2D v) => v.Negate();

    public static Vector2D operator *(double d, Vector2D v) => new(d * v.X, d * v.Y);

    public static Vector2D operator *(Vector2D v, double d) => d * v;

    public static Vector2D operator /(Vector2D v, double d) => new(v.X / d, v.Y / d);
    

    public static Vector2D FromPolar(double radius, Angle angle)
    {
        if (radius < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(radius), radius, "Expected a radius greater than or equal to zero.");
        }

        return new Vector2D(
            radius * angle.Cos,
            radius * angle.Sin
            );
    }

    public static Vector2D OfVector(IReadonlyVector<double> vector)
    {
        if (vector.Length != 2)
        {
            throw new ArgumentException("The vector length must be 2 in order to convert it to a Vector2D");
        }

        return new Vector2D(vector[0], vector[1]);
    }
    
    public bool IsParallelTo(Vector2D other, double tolerance = 1e-10)
    {
        var dp = Math.Abs(Normalize().DotProduct(other.Normalize()));
        return Math.Abs(1 - dp) <= tolerance;
    }

    public bool IsParallelTo(Vector2D other, Angle tolerance)
    {
        var angle = AngleTo(other);
        if (angle < tolerance)
        {
            return true;
        }

        return new Angle(Math.PI) - angle < tolerance;
    }

    public bool IsPerpendicularTo(Vector2D other, double tolerance = 1e-10)
    {
        return Math.Abs(Normalize().DotProduct(other.Normalize())) < tolerance;
    }
    
    public bool IsPerpendicularTo(Vector2D other, Angle tolerance)
    {
        var angle = AngleTo(other);
        return (angle - Angle.HalfPi).Abs() < tolerance;
    }

    public Angle AngleTo(Vector2D other)
    {
        return new Angle(
            Math.Atan2(
                CrossProduct(other),
                DotProduct(other)
            )).Abs();
    }

    public Vector2D Rotate(Angle angle)
    {
        var cs = angle.Cos;
        var sn = angle.Sin;
        var x = X * cs - Y * sn;
        var y = X * sn + Y * cs;
        return new Vector2D(x, y);
    }

    public double DotProduct(Vector2D other) => X * other.X + Y * other.Y;

    /// <summary>
    /// Performs the 2D 'cross product' as if the 2D vectors were really 3D vectors in the z=0 plane, returning
    /// the scalar magnitude and direction of the resulting z value.
    /// Formula: (X * other.Y) - (Y * other.X)
    /// </summary>
    /// <returns>(X * other.Y) - (Y * other.X)</returns>
    public double CrossProduct(Vector2D other) => X * other.Y - Y * other.X;

    public Vector2D ProjectOn(Vector2D other) => other * (DotProduct(other) / other.DotProduct(other));

    public Vector2D Normalize()
    {
        var l = Length;
        return new Vector2D(X / l, Y / l);
    }
    
    public Vector2D ScaleBy(double d) => new(d * X, d * Y);
    public Vector2D Negate() => new(-1 * X, -1 * Y);
    public Vector2D Subtract(Vector2D v) => new(X - v.X, Y - v.Y);
    public Vector2D Add(Vector2D v) => new(X + v.X, Y + v.Y);
    
    public Vector2D TransformBy(Matrix m)
    {
        Span<double> transformed = stackalloc double[2];
        transformed = LinAl.Multiply(m, ToVector(), transformed);
        return new Vector2D(transformed[0], transformed[1]);
    }
    public Vector2D TransformBy(ReadOnlyMatrixSpan m)
    {
        Span<double> transformed = stackalloc double[2];
        transformed = LinAl.Multiply(m, ToVector().AsReadOnlySpan(), transformed);
        return new Vector2D(transformed[0], transformed[1]);
    }
    
    public Vector ToVector() => new(X, Y);

    public bool Equals(Vector2D other, double tolerance)
    {
        if (tolerance < 0)
        {
            throw new ArgumentException("epsilon < 0");
        }

        return Math.Abs(other.X - X) < tolerance &&
               Math.Abs(other.Y - Y) < tolerance;
    }
}