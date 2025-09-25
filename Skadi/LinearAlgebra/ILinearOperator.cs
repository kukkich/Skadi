using Skadi.LinearAlgebra.Vectors;

namespace Skadi.LinearAlgebra;

public interface ILinearOperator
{
    public Vector MultiplyOn(ReadOnlySpan<double> vector, Vector? resultMemory = null);
}