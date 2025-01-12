﻿using Skadi.Matrices.Sparse.Storages;

namespace Skadi.Matrices.Sparse;

public class SymmetricSparseMatrix
{
    public SparseMatrixRow this[int rowIndex]
    {
        get
        {
            ArgumentOutOfRangeException.ThrowIfNegative(rowIndex);

            var end = _rowIndexes[rowIndex + 1];

            var begin = _rowIndexes[rowIndex];
            var length = end - begin;

            return new SparseMatrixRow(
                new ReadOnlySpan<int>(_columnIndexes, begin, length),
                new Span<double>(Values, begin, length),
                rowIndex
            );
        }
    }

    public ref double this[int rowIndex, int columnIndex]
    {
        get
        {
            if (rowIndex < 0 || columnIndex < 0) throw new ArgumentOutOfRangeException(nameof(rowIndex));
            if (rowIndex == columnIndex)
            {
                return ref Diagonal[rowIndex];
            }
            if (columnIndex > rowIndex)
                (rowIndex, columnIndex) = (columnIndex, rowIndex);

            var end = _rowIndexes[rowIndex + 1];

            var begin = _rowIndexes[rowIndex];

            for (var i = begin; i < end; i++)
            {
                if (_columnIndexes[i] != columnIndex) continue;
                
                return ref Values[i];
            }

            throw new IndexOutOfRangeException();
        }
    }

    public ReadOnlySpan<int> RowIndexes => new(_rowIndexes);
    public ReadOnlySpan<int> ColumnIndexes => new(_columnIndexes);
    public int Size => Diagonal.Length;
    public double[] Diagonal { get; }
    public double[] Values { get; }

    private readonly int[] _rowIndexes;
    private readonly int[] _columnIndexes;

    public SymmetricSparseMatrix(
        IEnumerable<int> rowIndexes, IEnumerable<int> columnIndexes,
        IEnumerable<double> values,
        IEnumerable<double> diagonal
    )
    {
        _rowIndexes = rowIndexes.ToArray();
        _columnIndexes = columnIndexes.ToArray();
        Values = values.ToArray();
        Diagonal = diagonal.ToArray();

        if (Values.Length != _columnIndexes.Length) throw new ArgumentException(
            nameof(columnIndexes) + " and " + nameof(values) + "must have the same length"
        );
        if (Diagonal.Length != _rowIndexes.Length - 1) throw new ArgumentException(
            nameof(rowIndexes) + " and " + nameof(diagonal) + "must have the same length"
        );
    }

    public SymmetricSparseMatrix(
        IEnumerable<int> rowIndexes, IEnumerable<int> columnIndexes
    )
    {
        _rowIndexes = rowIndexes.ToArray();
        _columnIndexes = columnIndexes.ToArray();
        Diagonal = new double[_rowIndexes.Length - 1];
        Values = new double[_columnIndexes.Length];
    }
}