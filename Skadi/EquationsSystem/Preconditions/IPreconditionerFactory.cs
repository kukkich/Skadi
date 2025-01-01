using Skadi.Matrices;
using Skadi.Matrices.Sparse;

namespace Skadi.EquationsSystem.Preconditions;

public interface IPreconditionerFactory
{
    public IPreconditioner CreatePreconditioner(SymmetricSparseMatrix matrix);
}