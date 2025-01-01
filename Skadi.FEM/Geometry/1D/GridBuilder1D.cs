using Skadi.Geometry._1D;
using Skadi.Geometry.Splitting;
using Skadi.FEM.Core;
using Skadi.FEM.Geometry._2D;

namespace Skadi.FEM.Geometry._1D;

public class GridBuilder1D : IGridBuilder<double, GridDefinition1D> 
{
    public Grid<double, IElement> Build(GridDefinition1D definition)
    {
        if (definition.ControlPoints.Length < 2 || definition.ControlPoints.Length != definition.Splitters.Length + 1)
        {
            throw new ArgumentException("Invalid grid definition");
        }

        var nodesCount = definition.Splitters.Sum(x => x.Steps) + 1;
        var elementsCount = nodesCount - 1;
        var elements = Enumerable.Range(0, elementsCount)
            .Select(x => new Element(0, [x, x + 1]))
            .ToArray();
        var nodes = new double[nodesCount];

        var currentNodeIndex = 0;
        
        for (var i = 0; i < definition.Splitters.Length; i++)
        {
            var splitter = definition.Splitters[i];
            var begin = definition.ControlPoints[i];
            var end = definition.ControlPoints[i + 1];
            var interval = new Line1D(begin, end);
            foreach (var point in splitter.EnumerateValues(interval))
            {
                nodes[currentNodeIndex] = point;
                currentNodeIndex++;
            }

            currentNodeIndex--;
        }
        
        return new Grid<double, IElement>(new IrregularPointsCollection<double>(nodes), elements);
    }
}

public record GridDefinition1D(double[] ControlPoints, ICurveSplitter[] Splitters);