using Skadi.LinearAlgebra.Matrices;
using Skadi.LinearAlgebra.Vectors;

namespace Skadi.LinearAlgebra;

public static class MatrixOps
{
    // ---- kernels: explicit destination, never allocate (dense storage only) ----

    public static void Scale(double coefficient, ReadOnlyMatrixSpan a, MatrixSpan destination)
    {
        Shape.SameSize(a.Rows, a.Columns, destination.Size, destination.Size);

        for (var row = 0; row < a.Rows; row++)
        for (var column = 0; column < a.Columns; column++)
            destination[row, column] = a[row, column] * coefficient;
    }

    public static void Sum(ReadOnlyMatrixSpan a, ReadOnlyMatrixSpan b, MatrixSpan destination)
    {
        Shape.SameSize(a.Rows, a.Columns, b.Rows, b.Columns);
        Shape.SameSize(a.Rows, a.Columns, destination.Size, destination.Size);

        for (var row = 0; row < a.Rows; row++)
        for (var column = 0; column < a.Columns; column++)
            destination[row, column] = a[row, column] + b[row, column];
    }

    // Restricted to square matrices to match ReadOnlyMatrixSpan's other kernel-level contracts.
    public static void Multiply(ReadOnlyMatrixSpan a, ReadOnlySpan<double> vector, Span<double> destination)
    {
        Shape.Square(a.Rows, a.Columns);
        Shape.SameLength(a.Rows, vector.Length);
        Shape.SameLength(a.Rows, destination.Length);

        for (var row = 0; row < a.Rows; row++)
        {
            var sum = 0d;
            for (var column = 0; column < a.Columns; column++)
                sum += a[row, column] * vector[column];

            destination[row] = sum;
        }
    }

    public static void Multiply(IReadOnlyMatrix a, ReadOnlySpan<double> vector, Span<double> destination)
    {
        Shape.Conformant(a.Columns, vector.Length);
        Shape.SameLength(a.Rows, destination.Length);

        for (var row = 0; row < a.Rows; row++)
        {
            var sum = 0d;
            for (var column = 0; column < a.Columns; column++)
                sum += a[row, column] * vector[column];

            destination[row] = sum;
        }
    }

    // ---- convenience: allocate destination if omitted (dense storage only) ----

    public static Matrix Scale(double coefficient, IReadOnlyMatrix a, Matrix? destination = null)
    {
        destination = EnsureDestination(a.Rows, a.Columns, destination);

        for (var row = 0; row < a.Rows; row++)
        for (var column = 0; column < a.Columns; column++)
            destination[row, column] = a[row, column] * coefficient;

        return destination;
    }

    public static Matrix Sum(IReadOnlyMatrix a, IReadOnlyMatrix b, Matrix? destination = null)
    {
        Shape.SameSize(a.Rows, a.Columns, b.Rows, b.Columns);
        destination = EnsureDestination(a.Rows, a.Columns, destination);

        for (var row = 0; row < a.Rows; row++)
        for (var column = 0; column < a.Columns; column++)
            destination[row, column] = a[row, column] + b[row, column];

        return destination;
    }

    public static Matrix Multiply(IReadOnlyMatrix a, IReadOnlyMatrix b, Matrix? destination = null)
    {
        Shape.Conformant(a.Columns, b.Rows);
        if (destination is not null)
        {
            Shape.NotSameInstance(a, destination);
            Shape.NotSameInstance(b, destination);
        }

        destination = EnsureDestination(a.Rows, b.Columns, destination);

        for (var row = 0; row < a.Rows; row++)
        {
            for (var column = 0; column < b.Columns; column++)
            {
                var sum = 0d;
                for (var k = 0; k < a.Columns; k++)
                    sum += a[row, k] * b[k, column];

                destination[row, column] = sum;
            }
        }

        return destination;
    }

    public static Vector MultiplyOn(IReadOnlyMatrix a, IReadonlyVector<double> vector, Vector? destination = null)
    {
        destination = VectorOps.EnsureDestination(a.Rows, destination);
        Multiply(a, VectorOps.AsSpan(vector), destination);
        return destination;
    }

    private static Matrix EnsureDestination(int rows, int columns, Matrix? destination)
    {
        if (destination is null)
            return new Matrix(new double[rows, columns]);

        Shape.SameSize(rows, columns, destination.Rows, destination.Columns);
        return destination;
    }
}
