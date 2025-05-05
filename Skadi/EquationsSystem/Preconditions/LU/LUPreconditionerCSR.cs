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
    public Vector MultiplyOn(ReadOnlySpan<double> vector, Vector? resultMemory = null)
    {
        var n = _decomposed.Size;
        LinAl.ValidateOrAllocateIfNull(vector, ref resultMemory!);
        
        var y = new double[n];
        // Решаем L·y = v (прямой ход, учитываем, что L имеет единицы на диагонали)
        // Для i=0..n-1:
        // y[i] = v[i] - sum_{j < i} L(i,j)*y[j]
        for (var i = 0; i < n; i++)
        {
            var sum = vector[i];
            foreach (var (col, value) in _decomposed[i])
            {
                if (col >= i)
                {
                    break;
                }

                sum -= value * y[col];
            }
            y[i] = sum;
        }
        
        // Решаем U·x = y (обратный ход)
        // Для i=n-1..0:
        // x[i] = (y[i] - sum_{j > i} U(i,j)*x[j]) / U(i,i)
        for (var i = n - 1; i >= 0; i--)
        {
            var sum = y[i];
            double diag = 0;
            foreach (var (col, value) in _decomposed[i])
            {
                if (col == i)
                {
                    diag = value;
                }
                else if (col > i)
                {
                    sum -= value * resultMemory[col];
                }
            }

            if (diag == 0)
            {
                throw new Exception($"В матрице нулевая диагональ на строке {i}");
            }
            
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
        public Vector MultiplyOn(ReadOnlySpan<double> vector, Vector? resultMemory = null)
        {
            var n = _decomposed.Size;
            LinAl.ValidateOrAllocateIfNull(vector, ref resultMemory!);
            
            var rowPointers = _decomposed.RowPointers;
            var cols = _decomposed.ColumnIndexes;
            var vals = _decomposed.Values;
            
            for (var i = 0; i < n; i++)
            {
                var sum = vector[i];
                // Для каждого элемента L(i,j) c j < i
                for (var idx = rowPointers[i]; idx < rowPointers[i + 1]; idx++)
                {
                    var col = cols[idx];
                    if (col < i)
                    {
                        sum -= vals[idx] * resultMemory[col];
                    }
                    // Пропускаем diag и верхнюю часть U
                }
                resultMemory[i] = sum;
            }
            
            return resultMemory;
        }
    }
}