using SharpMath.FiniteElement.Core.Assembling;
using SharpMath.Matrices.Sparse;
using SharpMath.Vectors;

namespace SharpMath.FiniteElement.Assembling;

public class SymmetricInserter : IStackInserter<SymmetricSparseMatrix>
{
    public void InsertVector(Vector vector, StackLocalVector localVector)
    {
        for (var i = 0; i < localVector.IndexPermutation.Length; i++)
        {
            vector[localVector.IndexPermutation.Apply(i)] += localVector[i];
        }
    }

    public void InsertMatrix(SymmetricSparseMatrix matrix, StackLocalMatrix localMatrix)
    {
        var matrixSize = localMatrix.IndexPermutation.Length;
        for (var i = 0; i < matrixSize; i++)
        {
            var row = localMatrix.IndexPermutation
                .Apply(i);

            for (var j = 0; j < matrixSize; j++)
            {
                var column = localMatrix.IndexPermutation
                    .Apply(j);
                if (column > row) continue;
        
                matrix[row, column] += localMatrix[i, j];
            }
        }
    }
}