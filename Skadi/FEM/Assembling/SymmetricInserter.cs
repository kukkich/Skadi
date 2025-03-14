using Skadi.FEM.Core.Assembling;
using Skadi.Matrices.Sparse;
using Skadi.Vectors;

namespace Skadi.FEM.Assembling;

public class SymmetricInserter : IStackInserter<SymmetricRowSparseMatrix>
{
    public void InsertVector(Vector vector, StackLocalVector localVector)
    {
        for (var i = 0; i < localVector.IndexPermutation.Length; i++)
        {
            vector[localVector.IndexPermutation.Apply(i)] += localVector[i];
        }
    }

    public void InsertMatrix(SymmetricRowSparseMatrix matrix, StackLocalMatrix localMatrix)
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