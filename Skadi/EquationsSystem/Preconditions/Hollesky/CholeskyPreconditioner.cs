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
    
    // Evaluate M^-1 * v = x, where M = L*L^T
    public Vector MultiplyOn(Vector v, Vector? resultMemory = null)
    {
        LinAl.ValidateOrAllocateIfNull(v, ref resultMemory);
        
        // M^-1 * v = x 
        // v = M * r = L*(L^T * x) = L * y
        var y = ResolveY(v, resultMemory!);
        // L^T * x = y
        var x = ResolveX(y);

        return x;
    }

    private Vector ResolveY(Vector v, Vector y)
    {
        v.CopyTo(y);
        
        for (var i = 0; i < _decomposedMatrix.Size; i++)
        {
            var sum = 0d;
            foreach (var entry in _decomposedMatrix[i])
            {
                sum += entry.Value * y[entry.ColumnIndex];
            }

            y[i] = (v[i] - sum) / _decomposedMatrix.Diagonal[i];
        }
        return y;
    }

    private Vector ResolveX(Vector y)
    {
        var x = y;
        for (var i = _decomposedMatrix.Size - 1; i >= 0; i--)
        {
            x[i] /= _decomposedMatrix.Diagonal[i];
            
            foreach (var entry in _decomposedMatrix[i])
            {
                x[entry.ColumnIndex] -= entry.Value * x[i];
            }
        }

        return x;
    }
}