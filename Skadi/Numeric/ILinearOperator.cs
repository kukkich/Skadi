using Skadi.Vectors;

namespace Skadi.Numeric;

public interface ILinearOperator
{
    public Vector MultiplyOn(ReadOnlySpan<double> vector, Vector? resultMemory = null);
}