using Skadi.Matrices;

namespace Skadi.FEM.Core.Assembling;

public class LocalMatrix
{
    public double this[int x, int y] => Matrix[x, y];
    public IndexPermutation IndexPermutation { get; }
    public ImmutableMatrix Matrix { get; private set; }

    public LocalMatrix(ImmutableMatrix matrix, IndexPermutation indexPermutation)
    {
        Matrix = matrix;
        IndexPermutation = indexPermutation;
    }
}

public readonly ref struct StackLocalMatrix
{
    public double this[int x, int y] => MatrixSpan[x, y];
    public StackIndexPermutation IndexPermutation { get; }
    public MatrixSpan MatrixSpan { get; }

    public StackLocalMatrix(MatrixSpan matrixSpan, StackIndexPermutation indexPermutation)
    {
        MatrixSpan = matrixSpan;
        IndexPermutation = indexPermutation;
    }
}