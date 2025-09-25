using Skadi.EquationsSystem.Preconditions.Hollesky;
using Skadi.LinearAlgebra.Matrices.Sparse;
using Skadi.LinearAlgebra.Matrices.Sparse.Decompositions;

namespace Skadi.EquationsSystem.Preconditions.LDLT;

public class IncompleteLDLTPreconditionerFactory : IPreconditionerFactory<SymmetricRowSparseMatrix>
{
    public IPreconditioner CreatePreconditioner(SymmetricRowSparseMatrix matrix)
    {
        return new CholeskyPreconditioner(IncompleteLDLT.Decompose(matrix));
    }
}