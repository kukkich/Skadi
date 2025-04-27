namespace Skadi.FEM.Core.Geometry.Edges;

public interface IEdgesPortraitBuilder
{
    public EdgesPortrait Build<T>(IEnumerable<T> elements, int nodesCount)
        where T : IElement;
}

public class EdgesPortrait
{
    public int[] ColumnIndexes { get; init; } = null!;
    public int[] RowIndexes { get; init; } = null!;
}