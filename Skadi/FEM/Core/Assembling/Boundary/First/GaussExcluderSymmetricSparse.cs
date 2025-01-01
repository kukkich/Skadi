using Skadi.Matrices.Sparse;

namespace Skadi.FEM.Core.Assembling.Boundary.First;

public class GaussExcluderSymmetricSparse : IFirstBoundaryApplier<SymmetricSparseMatrix>
{
    public void Apply(Equation<SymmetricSparseMatrix> equation, FirstCondition condition)
    {
        var value = condition.Value;
        var index = condition.NodeIndex;
        
        var row = equation.Matrix[index];
        foreach (var indexValue in row)
        {
            equation.RightSide[indexValue.ColumnIndex] -= indexValue.Value * value;
            indexValue.SetValue(0);
        }
        
        for (var i = index + 1; i < equation.Matrix.Size; i++)
        {
            var sparseRow = equation.Matrix[i];
            if (!sparseRow.HasColumn(index))
            {
                continue;
            }

            equation.RightSide[i] -= sparseRow[index] * value;
            sparseRow[index] = 0;
        }

        equation.RightSide[index] = value;
        equation.Matrix.Diagonal[index] = 1d;
    }
}