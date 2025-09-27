namespace Skadi.FEM.Core.Assembling;

public readonly ref struct StackIndexPermutation(Span<int> permutation)
{
    public int Apply(int index) => Permutation[index];
    public int Length => Permutation.Length;
    public readonly Span<int> Permutation = permutation;
}