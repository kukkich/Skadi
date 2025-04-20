using Skadi.Matrices.Sparse;
using Skadi.Numeric;
using Skadi.Vectors;

namespace Skadi.EquationsSystem.Preconditions;

public interface IPreconditioner : ILinearOperator
{
    public Vector MultiplyOn(Vector x, Vector? resultMemory = null) => MultiplyOn((IReadonlyVector<double>)x, resultMemory);
}

public interface IPreconditionerPart : ILinearOperator
{
    public Vector MultiplyOn(Vector x, Vector? resultMemory = null) => MultiplyOn((IReadonlyVector<double>)x, resultMemory);
}

public interface IPreconditioner<out TResult>
{
    public TResult Decompose(SparseMatrix matrix);
}