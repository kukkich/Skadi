using Skadi.Matrices.Sparse;
using Skadi.Vectors;

namespace Skadi.EquationsSystem.Preconditions.LU;

public class LUPreconditionerCSR : IPreconditioner
{
    private readonly CSRMatrix _decomposed;

    public LUPreconditionerCSR(CSRMatrix decomposed)
    {
        _decomposed = decomposed;
    }

    // Evaluate M⁻¹·v = U⁻¹·(L⁻¹·v)
    public Vector MultiplyOn(IReadonlyVector<double> vector, Vector? resultMemory = null)
    {
        var n = _decomposed.Size;
        LinAl.ValidateOrAllocateIfNull(vector, ref resultMemory!);
        var y = Vector.Create(n);

        // Прямая подстановка: L·y = v
        for (var i = 0; i < n; i++)
        {
            var sum = vector[i];
            for (var j = _decomposed.RowPointers[i]; j < _decomposed.RowPointers[i + 1]; j++)
            {
                var col = _decomposed.ColumnIndexes[j];
                if (col >= i) // Только нижняя часть (L), без диагонали
                {
                    break;
                }
                sum -= _decomposed.Values[j] * y[col];
            }

            y[i] = sum; // Диагональ L предполагается равной 1
        }

        // Обратная подстановка: U·x = y
        for (var i = n - 1; i >= 0; i--)
        {
            var sum = y[i];
            var diag = 0.0;

            for (var j = _decomposed.RowPointers[i]; j < _decomposed.RowPointers[i + 1]; j++)
            {
                var col = _decomposed.ColumnIndexes[j];
                if (col == i)
                {
                    diag = _decomposed.Values[j];
                }
                else if (col > i)
                {
                    sum -= _decomposed.Values[j] * resultMemory[col];
                }
            }

            if (Math.Abs(diag) < 1e-14)
                throw new InvalidOperationException($"Zero diagonal element at row {i} in U");

            resultMemory[i] = sum / diag;
        }

        return resultMemory;
    }

    public IPreconditionerPart GetPart() => new LUPreconditionerCSRPart(_decomposed);

    public class LUPreconditionerCSRPart : IPreconditionerPart
    {
        private readonly CSRMatrix _decomposed;

        public LUPreconditionerCSRPart(CSRMatrix decomposed)
        {
            _decomposed = decomposed;
        }

        // Evaluate L⁻¹·v, where L from M = L*U
        public Vector MultiplyOn(IReadonlyVector<double> vector, Vector? resultMemory = null)
        {
            var n = _decomposed.Size;
            LinAl.ValidateOrAllocateIfNull(vector, ref resultMemory!);

            for (var i = 0; i < n; i++)
            {
                var sum = vector[i];
                for (var j = _decomposed.RowPointers[i]; j < _decomposed.RowPointers[i + 1]; j++)
                {
                    var col = _decomposed.ColumnIndexes[j];
                    if (col >= i) // Только нижняя часть (L), без диагонали
                    {
                        break;
                    }
                    sum -= _decomposed.Values[j] * resultMemory[col];
                }

                resultMemory[i] = sum; // Диагональ L предполагается равной 1
            }

            return resultMemory;
        }
    }
}