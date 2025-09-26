namespace Skadi.LinearAlgebra.Matrices.Sparse.Storages;

public readonly ref struct SparseMatrixRow(ReadOnlySpan<int> columnIndexes, Span<double> values, int index)
{
    public int Index { get; } = index;
    public int Length => _values.Length;
    public ReadOnlySpan<int> ColumnIndexes { get; } = columnIndexes;

    public ref double this[int column]
    {
        get
        {
            if (!HasColumn(column, out var valueIndex))
                throw new IndexOutOfRangeException();

            return ref _values[valueIndex];
        }
    }
    
    public bool TryGetValue(int column, out double value)
    {
        value = 0;
        if (!HasColumn(column, out var valueIndex))
            return false;
        
        value = _values[valueIndex];
        return true;
    }
    
    public bool TrySetValue(int column, double value)
    {
        if (!HasColumn(column, out var valueIndex))
            return false;
        
        _values[valueIndex] = value;
        return true;
    }
    
    public bool HasColumn(int column)
    {
        var valueIndex = ColumnIndexes.BinarySearch(column);
        return valueIndex >= 0;
    }
    
    private bool HasColumn(int column, out int columnIndex)
    {
        columnIndex = ColumnIndexes.BinarySearch(column);
        return columnIndex >= 0;
    }

    private readonly Span<double> _values = values;

    public Enumerator GetEnumerator() => new(ColumnIndexes, _values);

    public ref struct Enumerator
    {
        private readonly ReadOnlySpan<int> _columnIndexes;
        private readonly Span<double> _values;

        private int _index;

        internal Enumerator(ReadOnlySpan<int> columnIndexes, Span<double> values)
        {
            _columnIndexes = columnIndexes;
            _values = values;
            _index = -1;
        }

        public bool MoveNext()
        {
            var index = _index + 1;
            if (index >= _values.Length) return false;
            _index = index;
            return true;

        }

        public RefIndexValue Current => new(_columnIndexes[_index], _values, _index);
    }
}