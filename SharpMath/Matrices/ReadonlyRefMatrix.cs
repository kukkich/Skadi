namespace SharpMath.Matrices;

public readonly ref struct StackAllocMatrix
{
    private readonly ReadOnlySpan<double> _values;
    private readonly int _size;

    public StackAllocMatrix(ReadOnlySpan<double> values, int size)
    {
        _values = values;
        _size = size;
    }


}