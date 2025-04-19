namespace Skadi.Matrices;

public class CSRMatrix
{
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
        
        _rowPointers = rowPointers;
        _columnIndexes = columnIndexes;
        Values = values;
    }
}