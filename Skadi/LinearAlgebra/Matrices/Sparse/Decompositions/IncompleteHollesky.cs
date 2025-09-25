namespace Skadi.LinearAlgebra.Matrices.Sparse.Decompositions;

public static class IncompleteHollesky
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
                diagUpdate -= lij * lij;
            }

            // Если полученная величина отрицательна (из-за округления или особенностей неполного разложения),
            // можно задать защиту – например, взять max(diagUpdate, epsilon)
            if (diagUpdate <= 0)
                throw new Exception($"Невозможное разложение: отрицательный диагональный элемент в строке {i}.");

            L.Diagonal[i] = Math.Sqrt(diagUpdate);

            // Обновляем элементы ниже диагонали: для каждой строки j > i,
            // если в строке j присутствует элемент (j,i) (т.е. с колонкой i), то обновляем его.
            for (var j = i + 1; j < n; j++)
            {
                var rowJ = L[j];
                // Если в строке j нет элемента с колонкой i, значит
                // по схеме неполного разложения мы его не добавляем (не создаём fill–in)
                // Начинаем с A(j,i) (хранящегося в L[j][i])
                if (!rowJ.TryGetValue(i, out var s)) continue;


                // Вычитаем перекрёстные произведения L(i,k)*L(j,k) для k, присутствующих в строке i (k < i)
                // Перебираем все k из строки i; поскольку в портрете хранится только нижний треугольник,
                // гарантируется, что каждый k удовлетворяет k < i.
                foreach (var entry in rowI)
                {
                    var k = entry.ColumnIndex;
                    if (k >= i)
                        break; // дальнейшие k не удовлетворяют условию k < i

                    var lik = entry.Value;
                    // Если в строке j присутствует столбец k, то участвует в сумме
                    if (rowJ.TryGetValue(k, out var ljk))
                    {
                        s -= lik * ljk;
                    }
                }

                // Вычисляем новый элемент L(j,i)
                rowJ[i] = s / L.Diagonal[i];
            }
        }

        return L;
    }
}