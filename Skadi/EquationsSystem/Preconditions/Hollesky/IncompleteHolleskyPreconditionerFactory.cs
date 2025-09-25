using Skadi.LinearAlgebra.Matrices.Sparse;
using Skadi.LinearAlgebra.Matrices.Sparse.Decompositions;

namespace Skadi.EquationsSystem.Preconditions.Hollesky;

public class IncompleteHolleskyPreconditionerFactory : IPreconditionerFactory<SymmetricRowSparseMatrix>
{
    public IPreconditioner CreatePreconditioner(SymmetricRowSparseMatrix matrix)
    {
        return new CholeskyPreconditioner(IncompleteHollesky.Decompose(matrix));
    }
}