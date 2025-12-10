using Skadi.LinearAlgebra;
using Skadi.LinearAlgebra.Matrices.Sparse;
using Skadi.LinearAlgebra.Vectors;

namespace Skadi.EquationsSystem.Preconditions.Diagonal;

public class DiagonalComplexPreconditioner : IPreconditioner
{
    private readonly int _threadsCount;
    private readonly ComplexMatrix _preconditionMatrix;
    
    public DiagonalComplexPreconditioner(ComplexMatrix matrix, int threadsCount = 1)
    {
        _threadsCount = threadsCount;
        var inverseDiagonal = new double[matrix.Diagonal.Length];

        var offset = 0;
        for (var i = 0; i < matrix.Size; ++i)
        {
            var block = matrix[i, i];

            var inverseDeterminant = 1d / block.Determinant;

            inverseDiagonal[offset] = block.Real * inverseDeterminant;
            offset++;
            if (!block.HasImaginary) 
                continue;
            
            inverseDiagonal[offset] = -block.Imaginary * inverseDeterminant;
            offset++;
        }

        _preconditionMatrix = ComplexMatrix.CreateDiagonal(inverseDiagonal, matrix.DiagonalIndexes);
    }

    public Vector MultiplyOn(ReadOnlySpan<double> vector, Vector? resultMemory)
        => ParallelLinAl.Multiply(_preconditionMatrix, vector, resultMemory, _threadsCount);
}