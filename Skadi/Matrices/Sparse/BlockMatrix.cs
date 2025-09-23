using System.Numerics;

namespace Skadi.Matrices;

public class BlockMatrix
{
    private readonly int[] _ig;
    private readonly int[] _jg;
}

public class ComplexVector
{
}

// Скалярное произведение
// BlockMatrix * ComplexVector
// предобуславливатель, нужно только: M^-1 * v = ...
