using System.Runtime.CompilerServices;

namespace Skadi.LinearAlgebra.Matrices;

public readonly ref struct ReadOnlyMatrixSpan
{
    private readonly ReadOnlySpan<double> _values;
    public int Size { get; }

    public double this[int row, int column] => _values[GetSpanIndex(row, column)];

    public ReadOnlyMatrixSpan(ReadOnlySpan<double> values, int size)
    {
        if (values.Length != size * size)
            throw new ArgumentException($"Size of {nameof(values)} and {nameof(size)} do not match.");

        _values = values;
        Size = size;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetSpanIndex(int row, int column)
    {
        return row * Size + column;
    }
}