using System.Diagnostics;
using SharpMath.Geometry._2D;

namespace SharpMath.FiniteElement._2D;

// Кубический лагранжевый базис
[DebuggerDisplay("Indexes = [{NodeIndexes[0]}, {NodeIndexes[1]}, {NodeIndexes[2]}, {NodeIndexes[3]}], MaterialId = {MaterialId}")]
public record struct Element(int[] NodeIndexes, double Length, double Width, int MaterialId = 0)
{
    public const int StepsInsideElement = 1;
    public const int NodesOnBound = StepsInsideElement + 1;
    public const int NodesInElement = NodesOnBound * NodesOnBound;

    public int[] GetBoundNodeIndexes(Bound bound) =>
        bound switch
        {
            Bound.Left => new[]
            {
                NodeIndexes[0],
                NodeIndexes[2]
            },
            Bound.Right => new[]
            {
                NodeIndexes[1],
                NodeIndexes[3],
            },
            Bound.Bottom => new[]
            {
                NodeIndexes[0],
                NodeIndexes[1],
            },
            Bound.Top => new[]
            {
                NodeIndexes[2],
                NodeIndexes[3],
            },
            _ => throw new ArgumentOutOfRangeException()
        };
}