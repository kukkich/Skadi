using SharpMath.Matrices;

namespace SharpMath.EquationsSystem.Preconditions;

public interface IPreconditionerFactory
{
    public IPreconditioner CreatePreconditioner(SymmetricSparseMatrix matrix);
}