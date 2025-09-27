namespace Skadi.LinearAlgebra.Matrices.Sparse;

public class SparseMatrix
(
    int[] rowsIndexes,
    int[] columnsIndexes,
    double[] diagonal,
    double[] lowerValues,
    double[] upperValues
)
{
    public double[] Diagonal { get; set; } = diagonal;
    public double[] LowerValues { get; set; } = lowerValues;
    public double[] UpperValues { get; set; } = upperValues;
    public int[] RowsIndexes { get; } = rowsIndexes;
    public int[] ColumnsIndexes { get; } = columnsIndexes;

    public int RowsCount => Diagonal.Length;
    public int ColumnsCount => Diagonal.Length;
    public int this[int rowIndex, int columnIndex] =>
        Array.IndexOf(ColumnsIndexes, columnIndex, RowsIndexes[rowIndex],
            RowsIndexes[rowIndex + 1] - RowsIndexes[rowIndex]);

    public SparseMatrix(int[] rowsIndexes, int[] columnsIndexes) 
        : this
        (
            rowsIndexes, 
            columnsIndexes, 
            new double[rowsIndexes.Length - 1], 
            new double[rowsIndexes[^1]], 
            new double[rowsIndexes[^1]]
        )
    { }

    public SparseMatrix Clone()
    {
        var rowIndexes = new int[RowsIndexes.Length];
        var columnIndexes = new int[ColumnsIndexes.Length];
        var diagonal = new double[Diagonal.Length];
        var lowerValues = new double[LowerValues.Length];
        var upperValues = new double[UpperValues.Length];

        Array.Copy(RowsIndexes, rowIndexes, RowsIndexes.Length);
        Array.Copy(ColumnsIndexes, columnIndexes, ColumnsIndexes.Length);
        Array.Copy(Diagonal, diagonal, Diagonal.Length);
        Array.Copy(LowerValues, lowerValues, LowerValues.Length);
        Array.Copy(UpperValues, upperValues, UpperValues.Length);

        return new SparseMatrix(rowIndexes, columnIndexes, diagonal, lowerValues, upperValues);
    }

    public int[] CloneRows()
    {
        var rowIndexes = new int[RowsIndexes.Length];

        Array.Copy(RowsIndexes, rowIndexes, RowsIndexes.Length);

        return rowIndexes;
    }

    public double[] CloneDiagonal()
    {
        var diagonal = new double[Diagonal.Length];

        Array.Copy(Diagonal, diagonal, Diagonal.Length);

        return diagonal;
    }
}