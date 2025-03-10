using Skadi.Matrices.Sparse;
using Skadi.Matrices.Sparse.Decompositions;

namespace Skadi.EquationsSystem.Preconditions.Hollesky;

public class IncompleteHolleskyPreconditionerFactory : IPreconditionerFactory<SymmetricRowSparseMatrix>
{
    public IPreconditioner CreatePreconditioner(SymmetricRowSparseMatrix matrix)
    {
        return new CholeskyPreconditioner(IncompleteHollesky.Decompose(matrix));
    }
}