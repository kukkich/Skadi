using Skadi.LinearAlgebra.Vectors;

namespace Skadi.LinearAlgebra.Matrices.Sparse;

public class ComplexMatrix
(
    double[] di,
    double[] gg,
    int[] idi,
    int[] ijg,
    int[] ig,
    int[] jg
) : ILinearOperator
{
    public int Size => DiagonalIndexes.Length - 1;
    
    public int[] DiagonalIndexes { get; } = idi;
    public int[] RowIndex { get; } = ig;
    public int[] OffDiagonalIndexes { get; } = ijg;
    public int[] ColumnIndex { get; } = jg;
    public double[] Diagonal { get; } = di;
    public double[] Values { get; } = gg;

    public Block this[int i, int j] => GetBlock(i, j);
    
    public Vector MultiplyOn(ReadOnlySpan<double> vector, Vector? resultMemory = null)
        => ParallelLinAl.Multiply(this, vector, resultMemory);

    private Block GetBlock(int i, int j)
    {
        int currentBlockIndex;
        int length;

        if (i == j)
        {
            currentBlockIndex = DiagonalIndexes[i];
            length = GetDiagonalBlockSize(i);

            return new Block(Diagonal.AsSpan(currentBlockIndex, length));
        }

        currentBlockIndex = OffDiagonalIndexes[j];
        length = GetOffDiagonalBlockSize(j);

        return new Block(Values.AsSpan(currentBlockIndex, length));
    }
    
    private int GetDiagonalBlockSize(in int offset)
    {
        return DiagonalIndexes[offset + 1] - DiagonalIndexes[offset];
    }

    private int GetOffDiagonalBlockSize(in int offset)
    {
        return OffDiagonalIndexes[offset + 1] - OffDiagonalIndexes[offset];
    }

    public readonly ref struct Block(ReadOnlySpan<double> values)
    {
        private readonly ReadOnlySpan<double> _values = values;
        private double Real => _values[0];
        private double Imaginary => HasImaginary ? _values[1] : 0;
        private bool HasImaginary => _values.Length == 2;

        public static void Multiply(Block a, Block b, Span<double> result)
        {
            result[0] += a.Real * b.Real;
            result[1] += a.Real * b.Imaginary;

            if (a.HasImaginary && b.HasImaginary)
            {
                result[0] -= a.Imaginary * b.Imaginary;
                result[1] += a.Imaginary * b.Real;
            }
        }
    }
}