﻿using Skadi.Matrices.Transformation;

namespace Skadi.Matrices;

// TODO LU разложение
// TODO обратную, чтобы умножать A^-1 * v = w
public abstract class MatrixBase
{
    public abstract double this[int row, int column] { get; }
    public abstract int Rows { get; }
    public abstract int Columns { get; }

    public abstract ImmutableMatrix AsImmutable();
    public abstract Matrix AsMutable();

    public virtual LUMatrix LU()
    {
        throw new NotImplementedException();
    }

    public virtual Matrix Clone()
    {
        var values = new double[Rows, Columns];
        for (var i = 0; i < Rows; i++)
            for (var j = 0; j < Columns; j++)
                values[i, j] = this[i, j];

        return new Matrix(values);
    }
    
    public virtual void CopyTo(MatrixSpan destination)
    {
        if (destination.Size != Rows || destination.Size != Columns)
        {
            throw new ArgumentException("Matrix size mismatch");
        }
        
        for (var i = 0; i < Rows; i++)
        for (var j = 0; j < Columns; j++)
            destination[i, j] = this[i, j];
    }
    public virtual void CopyTo(Matrix destination)
    {
        if (destination.Rows != Rows || destination.Columns != Columns)
        {
            throw new ArgumentException("Matrix size mismatch");
        }
        
        for (var i = 0; i < Rows; i++)
        for (var j = 0; j < Columns; j++)
            destination[i, j] = this[i, j];
    }
}