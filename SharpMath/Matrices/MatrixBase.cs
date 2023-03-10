using SharpMath.Matrices.Transformation;

namespace SharpMath.Matrices;

// TODO LU разложение
// TODO обратную, чтобы умножать A^-1 * v = w

public abstract class MatrixBase
{
    public virtual double this[int x, int y] => Values[x, y];
    public int Rows => Values.GetLength(0);
    public int Columns => Values.GetLength(1);

    protected readonly double[,] Values;

    protected MatrixBase(double[,] values)
    {
        Values = values;
    }

    public abstract ImmutableMatrix AsImmutable();
    public abstract Matrix AsMutable();

    // ReSharper disable once InconsistentNaming
    public virtual LUMatrix LU()
    {
        throw new NotImplementedException();
        return new LUMatrix(Values);
    }

    public virtual Matrix Clone()
    {
        var values = new double[Rows, Columns];
        for (int i = 0; i < Rows; i++)
            for (int j = 0; j < Columns; j++)
                values[i, j] = this[i, j];

        return new Matrix(values);
    }
}