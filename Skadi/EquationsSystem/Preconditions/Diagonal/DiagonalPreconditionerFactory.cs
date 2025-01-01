using Skadi.Matrices;
using Skadi.Matrices.Sparse;

namespace Skadi.EquationsSystem.Preconditions.Diagonal;

public class DiagonalPreconditionerFactory : IPreconditionerFactory
{
    public IPreconditioner CreatePreconditioner(SymmetricSparseMatrix matrix)
    {
        return new DiagonalPreconditioner(matrix.Diagonal);
    }
}