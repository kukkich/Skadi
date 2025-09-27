namespace Skadi.LinearAlgebra.Matrices.Sparse.Storages;

public readonly ref struct RefIndexValue(int columnIndex, Span<double> value, int valueIndex)
{
    public int ColumnIndex { get; } = columnIndex;
    public double Value => _value[valueIndex];

    public void SetValue(double value)
    {
        _value[valueIndex] = value;
    }

    private readonly Span<double> _value = value;

    public void Deconstruct(out int columnIndex, out double value)
    {
        columnIndex = ColumnIndex;
        value = _value[valueIndex];
    }
}