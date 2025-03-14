using Skadi.Matrices.Sparse;

namespace Skadi.EquationsSystem.Preconditions.Diagonal;

public class DiagonalPreconditionerFactory : IPreconditionerFactory<SymmetricRowSparseMatrix>
{
    public IPreconditioner CreatePreconditioner(SymmetricRowSparseMatrix matrix)
    {
        return new DiagonalPreconditioner(matrix.Diagonal);
    }
}