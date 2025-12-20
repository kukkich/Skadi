using Skadi.LinearAlgebra.Vectors;

namespace Skadi.LinearAlgebra.Matrices.Sparse;

public class ComplexMatrix
(
    double[] diagonal,
    double[] values,
    int[] diagonalIndexes,
    int[] offDiagonalIndexes,
    int[] rowIndexes,
    int[] columnIndexes
) : ILinearOperator
{
    public static ComplexMatrix CreateDiagonal(double[] diagonal, int[] diagonalIndexes)
        => new
        (
            diagonal,
            [],
            diagonalIndexes,
            [],
            new int[diagonalIndexes.Length],
            []
        );
    
    public int Size => DiagonalIndexes.Length - 1;
    public int[] DiagonalIndexes { get; } = diagonalIndexes;
    public int[] RowIndexes { get; } = rowIndexes;
    public int[] OffDiagonalIndexes { get; } = offDiagonalIndexes;
    public int[] ColumnIndexes { get; } = columnIndexes;
    public double[] Diagonal { get; } = diagonal;
    public double[] Values { get; } = values;

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
            length = DiagonalIndexes[i + 1] - DiagonalIndexes[i];

            return new Block(Diagonal.AsSpan(currentBlockIndex, length));
        }

        currentBlockIndex = OffDiagonalIndexes[j];
        length = OffDiagonalIndexes[j + 1] - OffDiagonalIndexes[j];

        return new Block(Values.AsSpan(currentBlockIndex, length));
    }
    
    public readonly ref struct Block(ReadOnlySpan<double> values)
    {
        private readonly ReadOnlySpan<double> _values = values;
        public double Real => _values[0];
        public double Imaginary => HasImaginary ? _values[1] : 0;
        public bool HasImaginary => _values.Length == 2;

        public double Determinant => Math.Pow(Real, 2) + Math.Pow(Imaginary, 2);

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