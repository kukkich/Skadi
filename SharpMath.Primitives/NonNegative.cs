using System.Numerics;

namespace SharpMath.Primitives;

public readonly struct NonNegative<T> where T : INumber<T>
{
    public T Value { get; }

    public NonNegative(T value)
    {
        if (T.IsPositive(value))
        {
            throw new ArgumentException("Value must be non negative", nameof(value));
        }

        Value = value;
    }

    public static implicit operator T(NonNegative<T> positive)
    {
        return positive.Value;
    }

    public static implicit operator NonNegative<T>(T value)
    {
        return new NonNegative<T>(value);
    }
}