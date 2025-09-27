using Skadi.LinearAlgebra.Vectors;

namespace Skadi.LinearAlgebra.Matrices.Sparse;

public class ProfileMatrix(int[] rowsIndexes, double[] diagonal, List<double> lowerValues, List<double> upperValues)
{
    public double[] Diagonal { get; } = diagonal;
    public List<double> LowerValues { get; } = lowerValues;
    public List<double> UpperValues { get; } = upperValues;
    public int[] RowsIndexes { get; } = rowsIndexes;

    public int CountRows => Diagonal.Length;
    public int CountColumns => Diagonal.Length;

    // todo вынести в LinAl
    public static Vector operator *(ProfileMatrix matrix, Vector vector)
    {
        var result = new Vector(matrix.CountRows);

        for (var i = 0; i < matrix.CountRows; i++)
        {
            result[i] += matrix.Diagonal[i] * vector[i];

            var k = i - (matrix.RowsIndexes[i + 1] - matrix.RowsIndexes[i]);

            for (var j = matrix.RowsIndexes[i]; j < matrix.RowsIndexes[i + 1]; j++, k++)
            {
                result[i] += matrix.LowerValues[j] * vector[k];
                result[k] += matrix.UpperValues[j] * vector[i];
            }
        }

        return result;
    }

    public ProfileMatrix LU()
    {
        for (var i = 0; i < CountRows; i++)
        {
            var j = i - (RowsIndexes[i + 1] - RowsIndexes[i]);

            var sumD = 0d;

            for (var ij = RowsIndexes[i]; ij < RowsIndexes[i + 1]; ij++, j++)
            {
                var sumL = 0d;
                var sumU = 0d;

                var k = j - (RowsIndexes[j + 1] - RowsIndexes[j]);

                var ik = RowsIndexes[i];
                var kj = RowsIndexes[j];

                if (k - (i - (RowsIndexes[i + 1] - RowsIndexes[i])) < 0) kj -= k - (i - (RowsIndexes[i + 1] - RowsIndexes[i]));
                else ik += k - (i - (RowsIndexes[i + 1] - RowsIndexes[i]));

                for (; ik < ij; ik++, kj++)
                {
                    sumL += LowerValues[ik] * UpperValues[kj];
                    sumU += LowerValues[kj] * UpperValues[ik];
                }

                LowerValues[ij] -= sumL;
                UpperValues[ij] = (UpperValues[ij] - sumU) / Diagonal[j];

                sumD += LowerValues[ij] * UpperValues[ij];
            }

            Diagonal[i] -= sumD;

        }

        return this;
    }
}