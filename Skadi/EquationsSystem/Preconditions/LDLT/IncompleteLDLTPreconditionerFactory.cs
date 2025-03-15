using Skadi.EquationsSystem.Preconditions.Hollesky;
using Skadi.Matrices.Sparse;
using Skadi.Matrices.Sparse.Decompositions;

namespace Skadi.EquationsSystem.Preconditions.LDLT;

public class IncompleteLDLTPreconditionerFactory : IPreconditionerFactory<SymmetricRowSparseMatrix>
{
    public IPreconditioner CreatePreconditioner(SymmetricRowSparseMatrix matrix)
    {
        return new CholeskyPreconditioner(IncompleteHollesky.Decompose(matrix));
    }
}