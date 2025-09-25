using Skadi.Matrices.Sparse.Storages;

namespace Skadi.Matrices.Sparse;

public class SymmetricRowSparseMatrix
{
    public static SymmetricRowSparseMatrix FromUpperTriangle
    (
        int[] upperRowPointers,
        int[] upperColumnIndexes,
        double[] upperValues,
        double[] diagonal
    )
    {
        var n = diagonal.Length;

        // Шаг 1. Подсчитаем для каждой строки нижнего треугольника число элементов,
        var lowerCounts = new int[n];
        for (var row = 0; row < n; row++)
        {
            var start = upperRowPointers[row];
            var end = upperRowPointers[row + 1];
            for (var pos = start; pos < end; pos++)
            {
                var col = upperColumnIndexes[pos];
                lowerCounts[col]++;
            }
        }

        // Шаг 2. Вычислим массив rowPointers для нижнего треугольника по накоплению lowerCounts.
        var lowerRowPointers = new int[n + 1];
        lowerRowPointers[0] = 0;
        for (var i = 0; i < n; i++)
        {
            lowerRowPointers[i + 1] = lowerRowPointers[i] + lowerCounts[i];
        }

        var totalLower = lowerRowPointers[n];

        var lowerValues = new double[totalLower];
        var lowerColumnIndexes = new int[totalLower];

        // Чтобы располагать элементы в массиве без повторного распределения памяти,
        // подготовим временный массив-счетчик текущей позиции для каждой строки.
        var currentPos = new int[n];
        Array.Copy(lowerRowPointers, currentPos, n);

        // Шаг 3. Заполним нижний треугольник.
        // Проходим по всем строкам верхнего треугольника: для элемента (i,j) (i < j) добавляем
        // в строку j элемент со столбцом i.
        for (var i = 0; i < n; i++)
        {
            var start = upperRowPointers[i];
            var end = upperRowPointers[i + 1];
            for (var pos = start; pos < end; pos++)
            {
                var j = upperColumnIndexes[pos]; // j > i
                var dest = currentPos[j];
                lowerColumnIndexes[dest] = i; // симметричный элемент: (j,i)
                lowerValues[dest] = upperValues[pos];
                currentPos[j]++;
            }
        }

        return new SymmetricRowSparseMatrix(lowerRowPointers, lowerColumnIndexes, lowerValues, diagonal);
    }

    public static SymmetricRowSparseMatrix FromUpperTriangle(int[] upperRowPointers, int[] upperColumnIndexes)
    {
        var n = upperRowPointers.Length - 1;

        // Шаг 1. Подсчитаем для каждой строки нижнего треугольника число элементов,
        var lowerCounts = new int[n];
        for (var row = 0; row < n; row++)
        {
            var start = upperRowPointers[row];
            var end = upperRowPointers[row + 1];
            for (var pos = start; pos < end; pos++)
            {
                var col = upperColumnIndexes[pos];
                lowerCounts[col]++;
            }
        }

        // Шаг 2. Построим массив rowPointers для нижнего треугольника через накопление lowerCounts.
        var lowerRowPointers = new int[n + 1];
        lowerRowPointers[0] = 0;
        for (var i = 0; i < n; i++)
        {
            lowerRowPointers[i + 1] = lowerRowPointers[i] + lowerCounts[i];
        }

        var totalLower = lowerRowPointers[n];
        var lowerColumnIndexes = new int[totalLower];

        // Массив для отслеживания текущей вставки в каждую строку нижнего портрета.
        var currentPos = new int[n];
        Array.Copy(lowerRowPointers, currentPos, n);

        // Шаг 3. Заполним нижний портрет: для каждого элемента (i, j) из верхнего портрета
        // добавляем симметричный элемент (j, i) в портрет нижнего треугольника.
        for (var i = 0; i < n; i++)
        {
            var start = upperRowPointers[i];
            var end = upperRowPointers[i + 1];
            for (var pos = start; pos < end; pos++)
            {
                var j = upperColumnIndexes[pos]; // j > i
                var dest = currentPos[j];
                lowerColumnIndexes[dest] = i; // симметричный элемент: (j, i)
                currentPos[j]++;
            }
        }

        // Создаем и возвращаем матрицу, используя конструктор, принимающий только портрет (rowPointers и columnIndexes).
        return new SymmetricRowSparseMatrix(lowerRowPointers, lowerColumnIndexes);
    }

    public SparseMatrixRow this[int rowIndex]
    {
        get
        {
            ArgumentOutOfRangeException.ThrowIfNegative(rowIndex);

            var begin = _rowPointers[rowIndex];
            var end = _rowPointers[rowIndex + 1];
            var length = end - begin;

            return new SparseMatrixRow(
                new ReadOnlySpan<int>(_columnIndexes, begin, length),
                new Span<double>(Values, begin, length),
                rowIndex
            );
        }
    }

    public ref double this[int rowIndex, int columnIndex]
    {
        get
        {
            if (rowIndex < 0 || columnIndex < 0) throw new ArgumentOutOfRangeException(nameof(rowIndex));
            if (rowIndex == columnIndex)
            {
                return ref Diagonal[rowIndex];
            }

            if (columnIndex > rowIndex)
                (rowIndex, columnIndex) = (columnIndex, rowIndex);

            var begin = _rowPointers[rowIndex];
            var end = _rowPointers[rowIndex + 1];

            for (var i = begin; i < end; i++)
            {
                if (_columnIndexes[i] != columnIndex) continue;

                return ref Values[i];
            }

            throw new IndexOutOfRangeException($"Matrix portrait doesn't contain element [{rowIndex},{columnIndex}]");
        }
    }
    
    public ReadOnlySpan<int> RowPointers => new(_rowPointers);
    public ReadOnlySpan<int> ColumnIndexes => new(_columnIndexes);
    public int Size => Diagonal.Length;
    public double[] Diagonal { get; }
    public double[] Values { get; }

    private readonly int[] _rowPointers;
    private readonly int[] _columnIndexes;

    public SymmetricRowSparseMatrix
    (
        IEnumerable<int> rowPointers,
        IEnumerable<int> columnIndexes,
        IEnumerable<double> values,
        IEnumerable<double> diagonal
    )
    {
        _rowPointers = rowPointers.ToArray();
        _columnIndexes = columnIndexes.ToArray();
        Values = values.ToArray();
        Diagonal = diagonal.ToArray();

        if (Values.Length != _columnIndexes.Length)
            throw new ArgumentException(
                nameof(columnIndexes) + " and " + nameof(values) + "must have the same length"
            );
        if (Diagonal.Length != _rowPointers.Length - 1)
            throw new ArgumentException(
                nameof(rowPointers) + " length must be equal to " + nameof(diagonal) + " length - 1"
            );
    }

    public SymmetricRowSparseMatrix(IEnumerable<int> rowPointers, IEnumerable<int> columnIndexes)
    {
        _rowPointers = rowPointers.ToArray();
        _columnIndexes = columnIndexes.ToArray();
        Diagonal = new double[_rowPointers.Length - 1];
        Values = new double[_columnIndexes.Length];
    }

    public SymmetricRowSparseMatrix Clone()
    {
        return new SymmetricRowSparseMatrix
        (
            _rowPointers.ToArray(),
            _columnIndexes.ToArray(),
            Values.ToArray(),
            Diagonal.ToArray()
        );
    }
    
    public double GetValue(int rowIndex, int columnIndex)
    {
        try
        {
            return this[rowIndex, columnIndex];
        }
        catch (IndexOutOfRangeException)
        {
            return 0;
        }
    }
}