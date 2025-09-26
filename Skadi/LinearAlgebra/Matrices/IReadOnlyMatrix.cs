namespace Skadi.LinearAlgebra.Matrices;

public interface IReadOnlyMatrix
{
    public double this[int row, int column] { get; }
    public int Rows { get; }
    public int Columns { get; }
}