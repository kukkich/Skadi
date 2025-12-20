namespace ComplexSolvers;

public static class Binary
{
    public static IEnumerable<T> ReadFile<T>(Func<BinaryReader, T> readOne, string filePath)
    {
        using BinaryReader reader = new(File.Open(filePath, FileMode.Open));

        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            yield return readOne(reader);
        }
    }
}