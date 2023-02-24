namespace SharpMath.Matrices;

public class Matrix : MatrixBase
{
    public new double this[int x, int y]
    {
        set => Values[x, y] = value; 
        get => base[x, y];
    }

    public override ImmutableMatrix AsImmutable()
    {
        return new ImmutableMatrix(Values);
    }

    public override Matrix AsMutable()
    {
        return this;
    }

    public Matrix(double[,] values) 
        : base(values) 
    { }
}