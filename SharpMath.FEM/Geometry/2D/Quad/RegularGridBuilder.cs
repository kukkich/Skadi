using SharpMath.FEM.Core;
using SharpMath.Geometry;
using SharpMath.Geometry._1D;
using SharpMath.Geometry._2D;
using SharpMath.Geometry._2D.Shapes;
using SharpMath.Geometry.Splitting;

namespace SharpMath.FEM.Geometry._2D.Quad;

public class RegularGridBuilder : IGridBuilder<Point2D, RegularGridDefinition>
{
    public Grid<Point2D, IElement> Build(RegularGridDefinition definition)
    {
        var (controlPoints, xSplitters, ySplitters, areas, curveBounds) = definition;
        
        var (xNodesCount, yNodesCount) = GetSizes(xSplitters, ySplitters);
        var elementsCount = (xNodesCount - 1) * (yNodesCount - 1);
        var nodes = new Point2D[xNodesCount,yNodesCount];
        var elements = new Element[elementsCount];

        foreach (var area in areas)
        {
            for (var yIndex = area.BottomBoundId; yIndex < area.TopBoundId; yIndex++)
            {
                
                for (var xIndex = area.LeftBoundId; xIndex < area.RightBoundId; xIndex++)
                {
                    var (horizontalCurve, horizontalSplitter) = definition.GetCurveWithSplitter(Orientation.Horizontal, yIndex, xIndex);
                    var (verticalCurve, verticalSplitter) = definition.GetCurveWithSplitter(Orientation.Vertical, xIndex, yIndex);

                    var masterPoints = SplitMasterArea(horizontalSplitter, verticalSplitter);
                    

                }   
            }
        }
        
        for (var xIndex = 0; xIndex < controlPoints.GetLength(0) - 1; xIndex++)
        {
            var xSplitter = xSplitters[xIndex];
            for (var yIndex = 0; yIndex < controlPoints.GetLength(1) - 1; yIndex++)
            {
                var ySplitter = ySplitters[yIndex];
                var masterPoints = SplitMasterArea(xSplitter, ySplitter);
                
                
                var lb = controlPoints[xIndex, yIndex];
                var lt = controlPoints[xIndex, yIndex + 1];
                var rb = controlPoints[xIndex + 1, yIndex];
                var rt = controlPoints[xIndex + 1, yIndex + 1];
                var 
            }
        }
        
        throw new NotImplementedException();
    }

    private static (int XNodesCount, int YNodesCount) GetSizes(
        IEnumerable<ICurveSplitter> xSplitters, 
        IEnumerable<ICurveSplitter> ySplitters
        ) => (xSplitters.Sum(x => x.Steps) + 1, ySplitters.Sum(x => x.Steps) + 1);

    public Point2D[,] SplitMasterArea(ICurveSplitter xSplitter, ICurveSplitter ySplitter)
    {
        var line = new Line1D(0, 1);
        var (xNodesCount, yNodesCount) = GetSizes([xSplitter], [ySplitter]);
        var points = new Point2D[xNodesCount, yNodesCount];
        var xValues = xSplitter.EnumerateValues(line).ToArray();
        var yValues = ySplitter.EnumerateValues(line).ToArray();

        for (var i = 0; i < xValues.Length; i++)
        {
            var x = xValues[i];
            for (var j = 0; j < yValues.Length; j++)
            {
                var y = yValues[j];
                points[i, j] = new Point2D(x, y);
            }
        }

        return points;
    }
}

file static class GridDefinitionExtensions
{
    public static (ICurve2D Curve, ICurveSplitter Spliter) GetCurveWithSplitter(
        this RegularGridDefinition definition, 
        Orientation orientation, int lineId, int intervalId
        )
    {
        var (points, xSplitters, ySplitters, _, curveBounds) = definition;

        var startIndex = intervalId;
        var endIndex = intervalId + 1;
        var layerIndex = lineId;

        var (start, end) = orientation switch
        {
            Orientation.Horizontal => (points[startIndex, layerIndex], points[endIndex, layerIndex]),
            Orientation.Vertical => (points[layerIndex, startIndex], points[layerIndex, endIndex]),
            _ => throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null)
        };

        var curve = curveBounds
            .FirstOrDefault(x => x.Orientation == orientation 
                                 && x.LineId == lineId
                                 && x.IntervalId == intervalId
            )?.Curve 
            ?? new Line2D(start, end);
        var splitter = orientation switch
        {
            Orientation.Horizontal => xSplitters[intervalId],
            Orientation.Vertical => ySplitters[intervalId],
            _ => throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null)
        };
        
        return (curve, splitter);
    }
}