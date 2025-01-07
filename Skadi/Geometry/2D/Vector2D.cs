using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using Skadi.Matrices;
using Skadi.Vectors;
using Vector = Skadi.Vectors.Vector;

namespace Skadi.Geometry._2D;

public readonly record struct Vector2D(double X, double Y) : INumberBase<Vector2D>,
    IMultiplyOperators<Vector2D, double, Vector2D>
{
    public static Vector2D Zero => new(0, 0);
    public static Vector2D XAxis => new(1, 0);
    public static Vector2D YAxis => new(0, 1);

    public double Length => Math.Sqrt(X * X + Y * Y);
    public Vector2D Orthogonal => new(-Y, X);
    
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
    
    public static Vector2D operator +(Vector2D left, Vector2D right) => left.Add(right);
    public static Vector2D operator -(Vector2D left, Vector2D right) => left.Subtract(right);
    public static Vector2D operator -(Vector2D v) => v.Negate();
    public static Vector2D operator +(Vector2D value) => value;
    public static Vector2D operator *(double d, Vector2D v) => new(d * v.X, d * v.Y);
    public static Vector2D operator *(Vector2D v, double d) => d * v;
    public static Vector2D operator /(Vector2D v, double d) => new(v.X / d, v.Y / d);

    public bool IsParallelTo(Vector2D other, double tolerance = 1e-10)
    {
        var dp = Math.Abs(Normalize().DotProduct(other.Normalize()));
        return Math.Abs(1 - dp) <= tolerance;
    }
    public bool IsParallelTo(Vector2D other, Angle tolerance)
    {
        var angle = AngleBetween(other);
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
        var angle = AngleBetween(other);
        return (angle - Angle.HalfPi).Abs() < tolerance;
    }

    public Angle AngleBetween(Vector2D other)
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

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        throw new NotImplementedException();
    }
    
    public static Vector2D AdditiveIdentity => Zero;

    public static bool IsCanonical(Vector2D value) => true;
    public static bool IsComplexNumber(Vector2D value) => false;
    public static bool IsEvenInteger(Vector2D value) => false;
    public static bool IsFinite(Vector2D value) => double.IsFinite(value.X) && double.IsFinite(value.Y);
    public static bool IsImaginaryNumber(Vector2D value) => false;
    public static bool IsInfinity(Vector2D value) => double.IsInfinity(value.X) || double.IsInfinity(value.Y);
    public static bool IsInteger(Vector2D value) => false;
    public static bool IsNaN(Vector2D value) => double.IsNaN(value.X) || double.IsNaN(value.Y);
    public static bool IsNegative(Vector2D value) => false;
    public static bool IsNegativeInfinity(Vector2D value) => false;
    public static bool IsNormal(Vector2D value) => value.Length == 1;
    public static bool IsOddInteger(Vector2D value) => false;
    public static bool IsPositive(Vector2D value) => false;
    public static bool IsPositiveInfinity(Vector2D value) => false;
    public static bool IsRealNumber(Vector2D value) => false;
    public static bool IsSubnormal(Vector2D value) => false;
    public static bool IsZero(Vector2D value) => value == default;
    public static Vector2D MaxMagnitude(Vector2D x, Vector2D y) => MaxMagnitudeNumber(x, y);
    public static Vector2D MaxMagnitudeNumber(Vector2D x, Vector2D y) => x.Length > y.Length ? x : y;
    public static Vector2D MinMagnitude(Vector2D x, Vector2D y) => MinMagnitudeNumber(x, y);
    public static Vector2D MinMagnitudeNumber(Vector2D x, Vector2D y) => x.Length < y.Length ? x : y;

    #region Formatting
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static Vector2D Parse(string s, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Vector2D result)
    {
        throw new NotImplementedException();
    }

    public static Vector2D Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Vector2D result)
    {
        throw new NotImplementedException();
    }

    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out Vector2D result)
    {
        throw new NotImplementedException();
    }

    public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, out Vector2D result)
    {
        throw new NotImplementedException();
    }

    public static Vector2D Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }
    public static Vector2D Parse(string s, NumberStyles style, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Converters
    public static bool TryConvertFromChecked<TOther>(TOther value, out Vector2D result) where TOther : INumberBase<TOther>
    {
        if (value is Vector2D other)
        {
            result = other;
            return true;
        }
        result = default;
        return false;
    }
    public static bool TryConvertFromSaturating<TOther>(TOther value, out Vector2D result) where TOther : INumberBase<TOther>
    {
        return TryConvertFromChecked(value, out result);
    }
    public static bool TryConvertFromTruncating<TOther>(TOther value, out Vector2D result) where TOther : INumberBase<TOther>
    {
        return TryConvertFromChecked(value, out result);
    }
    public static bool TryConvertToChecked<TOther>(Vector2D value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        if (typeof(TOther) == typeof(Vector2D))
        {
            result = (TOther)(object)value;
            return true;
        }
        result = TOther.Zero;
        return false;
    }
    public static bool TryConvertToSaturating<TOther>(Vector2D value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        return TryConvertToChecked(value, out result);
    }
    public static bool TryConvertToTruncating<TOther>(Vector2D value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        return TryConvertToChecked(value, out result);
    }
    #endregion

    #region NotSupported
    public static Vector2D One => throw new NotSupportedException(); // INumberBase
    public static int Radix => throw new NotSupportedException(); // INumberBase
    public static Vector2D operator --(Vector2D value) => throw new NotSupportedException(); // IDecrementOperators
    public static Vector2D operator /(Vector2D left, Vector2D right) => throw new NotSupportedException(); // IDivisionOperators<TSelf, TSelf, TSelf>,
    public static Vector2D operator ++(Vector2D value) => throw new NotSupportedException(); // IIncrementOperators<TSelf>
    public static Vector2D MultiplicativeIdentity => throw new NotSupportedException(); // IMultiplicativeIdentity
    public static Vector2D operator *(Vector2D left, Vector2D right) => throw new NotSupportedException(); // IMultiplyOperators<TSelf, TOther, TResult>
    public static Vector2D Abs(Vector2D value) => throw new NotSupportedException(); // INumberBase
    #endregion
}