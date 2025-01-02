using System.Runtime.CompilerServices;

namespace Skadi.Matrices;

public readonly ref struct MatrixSpan
{
    private readonly Span<double> _values;
    public int Size { get; }

    public double this[int row, int column]
    {
        get => _values[GetSpanIndex(row, column)];

        set => _values[GetSpanIndex(row, column)] = value;
    }

    public MatrixSpan(Span<double> values, int size)
    {
        if (values.Length != size * size)
            throw new ArgumentException();

        _values = values;
        Size = size;
    }

    public ReadOnlyMatrixSpan AsReadOnly() => this;
    public static implicit operator ReadOnlyMatrixSpan(MatrixSpan a) => new (a._values, a.Size);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetSpanIndex(int row, int column)
    {
        return row * Size + column;
    }
}