using Skadi.LinearAlgebra.Matrices.Sparse.Storages;
using Skadi.LinearAlgebra.Vectors;

namespace Skadi.LinearAlgebra.Matrices.Sparse;

public class CSRMatrix : ILinearOperator
{
    public SparseMatrixRow this[int rowIndex]
    {
        get
        {
            ArgumentOutOfRangeException.ThrowIfNegative(rowIndex);

            var begin = _rowPointers[rowIndex];
            var end = _rowPointers[rowIndex + 1];
            var length = end - begin;

            return new SparseMatrixRow(
                new ReadOnlySpan<int>(_columnIndexes, begin, length),
                new Span<double>(Values, begin, length),
                rowIndex
            );
        }
    }
    
    public ReadOnlySpan<int> RowPointers => new(_rowPointers);
    public ReadOnlySpan<int> ColumnIndexes => new(_columnIndexes);
    public int Size => _rowPointers.Length - 1;
    public double[] Values { get; }

    private readonly int[] _rowPointers;
    private readonly int[] _columnIndexes;
    
    public CSRMatrix(int[] rowPointers, int[] columnIndexes, double[] values)
    {
        if (values.Length != columnIndexes.Length)
            throw new ArgumentException(
                nameof(columnIndexes) + " and " + nameof(values) + "must have the same length"
            );
        if (rowPointers[^1] != columnIndexes.Length)
            throw new ArgumentException(
                "Last element of " + nameof(rowPointers) + " should be equal to the length of " + nameof(columnIndexes)
            );
        _rowPointers = rowPointers;
        _columnIndexes = columnIndexes;
        Values = values;
    }

    public Vector MultiplyOn(ReadOnlySpan<double> vector, Vector? resultMemory = null)
    {
        VectorOps.EnsureDestination(vector, ref resultMemory);
        var result = resultMemory!;

        for (var row = 0; row < Size; row++)
        {
            var rowStart = _rowPointers[row];
            var rowEnd = _rowPointers[row + 1];

            var sum = 0d;
            for (var i = rowStart; i < rowEnd; i++)
                sum += Values[i] * vector[_columnIndexes[i]];

            result[row] = sum;
        }

        return result;
    }
}