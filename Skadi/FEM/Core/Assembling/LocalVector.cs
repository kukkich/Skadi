namespace Skadi.FEM.Core.Assembling;

public readonly ref struct StackLocalVector(Span<double> vector, StackIndexPermutation permutation)
{
    public double this[int x] => _vector[x];
    public StackIndexPermutation IndexPermutation { get; } = permutation;
    public int Length => _vector.Length;

    private readonly Span<double> _vector = vector;
}