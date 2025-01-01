namespace Skadi.FEM.Core.Geometry.Edges;

public interface IEdgesPortraitBuilder
{
    public EdgesPortrait Build(IEnumerable<IElement> elements, int nodesCount);
}

public class EdgesPortrait
{
    public int[] ColumnIndexes { get; init; } = null!;
    public int[] RowIndexes { get; init; } = null!;
}