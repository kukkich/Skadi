using Skadi.Matrices.Sparse;

namespace Skadi.EquationsSystem.Preconditions;

public interface IPreconditionerFactory<in TMatrix>
{
    public IPreconditioner CreatePreconditioner(TMatrix matrix);
}