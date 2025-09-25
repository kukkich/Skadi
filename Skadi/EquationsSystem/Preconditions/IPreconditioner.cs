using Skadi.LinearAlgebra;
using Skadi.LinearAlgebra.Matrices.Sparse;
using Skadi.LinearAlgebra.Vectors;

namespace Skadi.EquationsSystem.Preconditions;

public interface IPreconditioner : ILinearOperator
{
    public Vector MultiplyOn(Vector x, Vector? resultMemory = null) => ((ILinearOperator) this).MultiplyOn(x, resultMemory);
}

public interface IPreconditionerPart : ILinearOperator
{
    public Vector MultiplyOn(Vector x, Vector? resultMemory = null) => ((ILinearOperator) this).MultiplyOn(x, resultMemory);
}

public interface IPreconditioner<out TResult>
{
    public TResult Decompose(SparseMatrix matrix);
}