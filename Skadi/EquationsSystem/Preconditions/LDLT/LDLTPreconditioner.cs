using Skadi.LinearAlgebra;
using Skadi.LinearAlgebra.Matrices.Sparse;
using Skadi.LinearAlgebra.Vectors;

namespace Skadi.EquationsSystem.Preconditions.LDLT;

public class LDLTPreconditioner(SymmetricRowSparseMatrix decomposedMatrix) : IPreconditioner
{
    // Evaluate M^-1 * v = x, where M = L*D*L^T
    public Vector MultiplyOn(ReadOnlySpan<double> v, Vector? resultMemory = null)
    {
        LinAl.ValidateOrAllocateIfNull(v, ref resultMemory);
        
        // M^-1 * v = x 
        // v = M * r = L*(D * [L^T * x]) = L * (D * z) = L * y
        var y = ResolveY(v, resultMemory!);

        for (var i = 0; i < y.Length; i++)
        {
            y[i] /= decomposedMatrix.Diagonal[i];
        }
        
        // L^T * x = y
        var x = ResolveX(y);

        return x;
    }

    private Vector ResolveY(ReadOnlySpan<double> v, Vector y)
    {
        for (var i = 0; i < v.Length; i++)
        {
            y[i] = v[i];
        }
        
        for (var i = 0; i < decomposedMatrix.Size; i++)
        {
            var sum = 0d;
            foreach (var entry in decomposedMatrix[i])
            {
                sum += entry.Value * y[entry.ColumnIndex];
            }

            y[i] = v[i] - sum;
        }
        return y;
    }

    private Vector ResolveX(Vector y)
    {
        for (var i = decomposedMatrix.Size - 1; i >= 0; i--)
        {
            foreach (var entry in decomposedMatrix[i])
            {
                y[entry.ColumnIndex] -= entry.Value * y[i];
            }
        }

        return y;
    }
}