using SharpMath.EquationsSystem.Preconditions;
using SharpMath.Matrices.Sparse;
using SharpMath.Vectors;

namespace SharpMath.EquationsSystem.Solver;

public class LUSparse : ISLAESolver<SparseMatrix>
{
    private readonly LUPreconditioner _luPreconditioner;

    public LUSparse(LUPreconditioner luPreconditioner)
    {
        _luPreconditioner = luPreconditioner;
    }

    public Vector Solve(Equation<SparseMatrix> equation)
    {
        var matrix = _luPreconditioner.Decompose(equation.Matrix);
        var y = CalcY(matrix, equation.RightSide);
        var x = CalcX(matrix, y);
        return x;
    }

    public Vector CalcY(SparseMatrix sparseMatrix, Vector b)
    {
        var y = b;

        for (var i = 0; i < sparseMatrix.RowsCount; i++)
        {
            var sum = 0.0;
            for (var j = sparseMatrix.RowsIndexes[i]; j < sparseMatrix.RowsIndexes[i + 1]; j++)
            {
                sum += sparseMatrix.LowerValues[j] * y[sparseMatrix.ColumnsIndexes[j]];
            }
            y[i] = (b[i] - sum) / sparseMatrix.Diagonal[i];
        }
        return y;
    }

    public Vector CalcX(SparseMatrix sparseMatrix, Vector y)
    {
        var x = y.Copy(new double[sparseMatrix.RowsCount]);

        for (var i = sparseMatrix.RowsCount - 1; i >= 0; i--)
        {
            for (var j = sparseMatrix.RowsIndexes[i + 1] - 1; j >= sparseMatrix.RowsIndexes[i]; j--)
            {
                x[sparseMatrix.ColumnsIndexes[j]] -= sparseMatrix.UpperValues[j] * x[i];
            }
        }

        return x;
    }

    public void CalcXWithoutMemory(SparseMatrix sparseMatrix, Vector y)
    {
        var x = y;

        for (var i = sparseMatrix.RowsCount - 1; i >= 0; i--)
        {
            for (var j = sparseMatrix.RowsIndexes[i + 1] - 1; j >= sparseMatrix.RowsIndexes[i]; j--)
            {
                x[sparseMatrix.ColumnsIndexes[j]] -= sparseMatrix.UpperValues[j] * x[i];
            }
        }
    }
}