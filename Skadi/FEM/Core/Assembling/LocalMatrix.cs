using Skadi.LinearAlgebra.Matrices;

namespace Skadi.FEM.Core.Assembling;

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