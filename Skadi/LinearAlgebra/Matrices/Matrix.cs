namespace Skadi.LinearAlgebra.Matrices;

public class Matrix(double[,] values) : IReadOnlyMatrix
{
    public double this[int row, int column]
    {
        set => values[row, column] = value; 
        get => values[row, column];
    }
    public int Rows => values.GetLength(0);
    public int Columns => values.GetLength(1);
}