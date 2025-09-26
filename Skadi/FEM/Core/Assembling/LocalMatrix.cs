using Skadi.LinearAlgebra.Matrices;

namespace Skadi.FEM.Core.Assembling;

public readonly ref struct StackLocalMatrix(MatrixSpan matrixSpan, StackIndexPermutation indexPermutation)
{
    public double this[int x, int y] => MatrixSpan[x, y];
    public StackIndexPermutation IndexPermutation { get; } = indexPermutation;
    public MatrixSpan MatrixSpan { get; } = matrixSpan;
}