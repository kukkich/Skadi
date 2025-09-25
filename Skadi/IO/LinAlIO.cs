﻿// ReSharper disable InconsistentNaming

using Skadi.LinearAlgebra.Matrices;
using Skadi.LinearAlgebra.Vectors;

namespace Skadi.IO;

public static class LinAlIO
{
    public static void Write(MatrixBase matrix, string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentException("Путь к файлу не может быть пустым.", nameof(path));
        }

        try
        {
            using var writer = new StreamWriter(path);
            writer.WriteLine(matrix.Rows);
            writer.WriteLine(matrix.Columns);

            for (var row = 0; row < matrix.Rows; row++)
            {
                for (var column = 0; column < matrix.Columns; column++)
                {
                    writer.WriteLine(matrix[row, column]);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка при записи матрицы в файл: {ex.Message}");
            throw;
        }
    }

    public static void Write(IReadonlyVector<double> v, string path, bool specifyLength = true)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentException("Путь к файлу не может быть пустым.", nameof(path));
        }

        try
        {
            using var writer = new StreamWriter(path);
            if (specifyLength)
            {
                writer.WriteLine(v.Length);
            }

            for (var i = 0; i < v.Length; i++)
            {
                writer.WriteLine(v[i]);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка при записи вектора в файл: {ex.Message}");
            throw;
        }
    }

    public static Vector ReadVector(string path, bool lengthSpecified = true)
    {
        using var stream = new StreamReader(File.OpenRead(path));
        Vector result;
        if (lengthSpecified)
        {
            var size = int.Parse(stream.ReadLine()!);
            result = Vector.Create(size, _ => double.Parse(stream.ReadLine()));
        }
        else
        {
            result = new Vector(EnumerateFileValues(stream));
        }

        return result;

        IEnumerable<double> EnumerateFileValues(StreamReader stream)
        {
            while (stream.ReadLine() is { } line)
            {
                line = line.Trim();

                if (string.IsNullOrEmpty(line))
                {
                    yield break;
                }

                yield return double.Parse(line);
            }
        }
    }

    public static Matrix Read(string path, Matrix? resultMemory = null)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentException("Путь к файлу не может быть пустым.", nameof(path));
        }

        try
        {
            using var reader = new StreamReader(path);
            var rows = int.Parse(reader.ReadLine()!);
            var columns = int.Parse(reader.ReadLine()!);

            if (resultMemory is null)
            {
                resultMemory = new Matrix(new double[rows, columns]);
            }
            else if (resultMemory.Rows != rows || resultMemory.Columns != columns)
            {
                throw new ArgumentException(
                    $"Invalid result memory size. Expected rows={rows} & columns={columns}" +
                    $", but was rows={resultMemory.Rows}, columns={resultMemory.Columns}",
                    nameof(resultMemory)
                );
            }

            for (var row = 0; row < resultMemory.Rows; row++)
            {
                for (var column = 0; column < resultMemory.Columns; column++)
                {
                    var value = double.Parse(reader.ReadLine()!);
                    resultMemory[row, column] = value;
                }
            }

            return resultMemory;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка при записи матрицы в файл: {ex.Message}");
            throw;
        }
    }
}