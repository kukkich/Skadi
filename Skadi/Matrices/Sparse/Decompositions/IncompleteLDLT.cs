namespace Skadi.Matrices.Sparse.Decompositions;

public class IncompleteLDLT
{
    public static SymmetricRowSparseMatrix Decompose(SymmetricRowSparseMatrix matrix)
    {
        var L = matrix.Clone();
        var n = L.Size;

        for (var i = 0; i < n; i++)
        {
            var diagUpdate = L.Diagonal[i];

            var rowI = L[i];

            // По всем элементам в строке i (все индексы j в строке i удовлетворяют j < i)
            foreach (var entry in rowI)
            {
                var lij = entry.Value;
                diagUpdate -= lij * lij * L.Diagonal[entry.ColumnIndex];
            }

            L.Diagonal[i] = diagUpdate;

            for (var j = i + 1; j < n; j++)
            {
                var rowJ = L[j];

                if (!rowJ.TryGetValue(i, out var s))
                    continue;

                foreach (var entry in rowI)
                {
                    var k = entry.ColumnIndex;
                    if (k >= i)
                        break;
                    var lik = entry.Value;
                    if (rowJ.TryGetValue(k, out var ljk))
                    {
                        s -= lik * ljk * L.Diagonal[k];
                    }
                }

                rowJ[i] = s / L.Diagonal[i];
            }
        }

        return L;
    }
}