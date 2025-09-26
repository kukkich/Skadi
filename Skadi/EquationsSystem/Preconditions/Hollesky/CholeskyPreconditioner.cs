using Skadi.LinearAlgebra;
using Skadi.LinearAlgebra.Matrices.Sparse;
using Skadi.LinearAlgebra.Vectors;

namespace Skadi.EquationsSystem.Preconditions.Hollesky;

public class CholeskyPreconditioner(SymmetricRowSparseMatrix decomposedMatrix) : IPreconditioner
{
    // Evaluate M^-1 * v = x, where M = L*L^T
    public Vector MultiplyOn(ReadOnlySpan<double> vector, Vector? resultMemory = null)
    {
        LinAl.ValidateOrAllocateIfNull(vector, ref resultMemory);
        
        // M^-1 * v = x 
        // v = M * r = L*(L^T * x) = L * y
        var y = ResolveY(vector, resultMemory!);
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

            y[i] = (v[i] - sum) / decomposedMatrix.Diagonal[i];
        }
        return y;
    }

    private Vector ResolveX(Vector y)
    {
        for (var i = decomposedMatrix.Size - 1; i >= 0; i--)
        {
            y[i] /= decomposedMatrix.Diagonal[i];
            
            foreach (var entry in decomposedMatrix[i])
            {
                y[entry.ColumnIndex] -= entry.Value * y[i];
            }
        }

        return y;
    }
}