using Skadi.Matrices.Sparse;
using Skadi.Vectors;

namespace Skadi.EquationsSystem.Preconditions;

public interface IPreconditioner
{
    public Vector MultiplyOn(Vector x, Vector? resultMemory = null);
}

public interface IPreconditioner<out TResult>
{
    public TResult Decompose(SparseMatrix matrix);
}