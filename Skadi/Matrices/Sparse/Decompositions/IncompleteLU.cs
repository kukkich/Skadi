namespace Skadi.Matrices.Sparse.Decompositions;

public static class IncompleteLU
{
    public static CSRMatrix Decompose(CSRMatrix a)
    {
        // Клонируем данные матрицы, чтобы не изменять исходную матрицу a
        var rowPointers = a.RowPointers.ToArray();
        var columnIndexes = a.ColumnIndexes.ToArray();
        var values = (double[])a.Values.Clone();
        var n = rowPointers.Length - 1;
        
        // Цикл по строкам
        for (var i = 0; i < n; i++)
        {
            // Обработка каждого элемента в строке i, находящегося ниже диагонали (элементы L)
            for (var idx = rowPointers[i]; idx < rowPointers[i + 1]; idx++)
            {
                var col = columnIndexes[idx];
                // Рассматриваем только элементы ниже диагонали (col < i)
                if (col >= i)
                    continue;
                
                // Находим диагональный элемент в строке col
                var diagIdx = FindDiagonalIndex(col, rowPointers, columnIndexes);
                if (diagIdx == -1)
                    throw new Exception("Отсутствует диагональный элемент в строке " + col);
                
                // Вычисляем множитель L(i,col)
                values[idx] /= values[diagIdx];
                var factor = values[idx];
                
                // Обновляем элементы U в строке i,
                // проходясь по строке col и вычитая вклад L(i,col)*U(col,k)
                for (var k = rowPointers[col]; k < rowPointers[col + 1]; k++)
                {
                    var col_k = columnIndexes[k];
                    // Обновляем только элементы правее диагонали col
                    if (col_k <= col)
                        continue;
                    
                    // Находим в строке i элемент с таким же столбцом col_k.
                    // Если внутри разреженной структуры нет соответствующего элемента, его значение не обновляем (ILU0).
                    var idx2 = FindPosition(i, col_k, rowPointers, columnIndexes);
                    if (idx2 != -1)
                    {
                        values[idx2] -= factor * values[k];
                    }
                }
            }
        }
        
        return new CSRMatrix(rowPointers, columnIndexes, values);
    }
    
    // Вспомогательный метод для поиска индекса диагонального элемента в строке row (т.е. где column == row)
    private static int FindDiagonalIndex(int row, int[] rowPointers, int[] columnIndexes)
    {
        for (var i = rowPointers[row]; i < rowPointers[row + 1]; i++)
        {
            if (columnIndexes[i] == row)
                return i;
        }
        return -1;
    }
    
    // Вспомогательный метод для поиска позиции (индекса в массиве values) элемента с индексом столбца col в строке row
    private static int FindPosition(int row, int col, int[] rowPointers, int[] columnIndexes)
    {
        for (var i = rowPointers[row]; i < rowPointers[row + 1]; i++)
        {
            if (columnIndexes[i] == col)
                return i;
        }
        return -1;
    }
}