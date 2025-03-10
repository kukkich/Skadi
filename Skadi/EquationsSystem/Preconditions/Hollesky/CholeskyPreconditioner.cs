using Skadi.Matrices.Sparse;
using Skadi.Vectors;

namespace Skadi.EquationsSystem.Preconditions.Hollesky;

public class CholeskyPreconditioner : IPreconditioner
{
    private readonly SymmetricRowSparseMatrix _decomposedMatrix;

    public CholeskyPreconditioner(SymmetricRowSparseMatrix decomposedMatrix)
    {
        _decomposedMatrix = decomposedMatrix;
    }
    
    public Vector MultiplyOn(Vector v, Vector? resultMemory = null)
    {
        throw new NotImplementedException();
    }
}