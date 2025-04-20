using Skadi.Vectors;

namespace Skadi.Numeric;

public interface ILinearOperator
{
    public Vector MultiplyOn(IReadonlyVector<double> vector, Vector? resultMemory=null);
}