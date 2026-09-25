using System.Runtime.CompilerServices;

namespace Skadi.LinearAlgebra;

public static class Shape
{
    public static void SameLength(
        int a, int b,
        [CallerArgumentExpression(nameof(a))] string? aName = null,
        [CallerArgumentExpression(nameof(b))] string? bName = null)
    {
        if (a != b)
            throw new ArgumentException($"{aName} and {bName} must have the same length ({a} != {b}).");
    }

    public static void SameSize(
        int aRows, int aColumns, int bRows, int bColumns,
        [CallerArgumentExpression(nameof(aRows))] string? aName = null,
        [CallerArgumentExpression(nameof(bRows))] string? bName = null)
    {
        if (aRows != bRows || aColumns != bColumns)
            throw new ArgumentException(
                $"{aName} and {bName} must have the same size ({aRows}x{aColumns} != {bRows}x{bColumns})."
            );
    }

    public static void Square(
        int rows, int columns,
        [CallerArgumentExpression(nameof(rows))] string? rowsName = null,
        [CallerArgumentExpression(nameof(columns))] string? columnsName = null)
    {
        if (rows != columns)
            throw new ArgumentException($"{rowsName} and {columnsName} must be equal for a square matrix ({rows} != {columns}).");
    }

    public static void Conformant(
        int aColumns, int bRows,
        [CallerArgumentExpression(nameof(aColumns))] string? aName = null,
        [CallerArgumentExpression(nameof(bRows))] string? bName = null)
    {
        if (aColumns != bRows)
            throw new ArgumentException($"{aName} and {bName} can't be multiplied: {aColumns} != {bRows}.");
    }

    public static void NotSameInstance(
        object a, object? b,
        [CallerArgumentExpression(nameof(b))] string? bName = null)
    {
        if (ReferenceEquals(a, b))
            throw new ArgumentException($"{bName} can't be the same instance as one of the operands.");
    }
}
