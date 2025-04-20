using Skadi.Matrices.Sparse;
using Skadi.Vectors;

namespace Skadi.EquationsSystem.Preconditions.LDLT;

public class LDLTPreconditioner : IPreconditioner
{
    private readonly SymmetricRowSparseMatrix _decomposedMatrix;

    public LDLTPreconditioner(SymmetricRowSparseMatrix decomposedMatrix)
    {
        _decomposedMatrix = decomposedMatrix;
    }
    
    // Evaluate M^-1 * v = x, where M = L*D*L^T
    public Vector MultiplyOn(IReadonlyVector<double> v, Vector? resultMemory = null)
    {
        LinAl.ValidateOrAllocateIfNull(v, ref resultMemory);
        
        // M^-1 * v = x 
        // v = M * r = L*(D * [L^T * x]) = L * (D * z) = L * y
        var y = ResolveY(v, resultMemory!);

        for (var i = 0; i < y.Length; i++)
        {
            y[i] /= _decomposedMatrix.Diagonal[i];
        }
        
        // L^T * x = y
        var x = ResolveX(y);

        return x;
    }

    private Vector ResolveY(IReadonlyVector<double> v, Vector y)
    {
        for (var i = 0; i < v.Length; i++)
        {
            y[i] = v[i];
        }
        
        for (var i = 0; i < _decomposedMatrix.Size; i++)
        {
            var sum = 0d;
            foreach (var entry in _decomposedMatrix[i])
            {
                sum += entry.Value * y[entry.ColumnIndex];
            }

            y[i] = v[i] - sum;
        }
        return y;
    }

    private Vector ResolveX(Vector y)
    {
        var x = y;
        for (var i = _decomposedMatrix.Size - 1; i >= 0; i--)
        {
            foreach (var entry in _decomposedMatrix[i])
            {
                x[entry.ColumnIndex] -= entry.Value * x[i];
            }
        }

        return x;
    }
}