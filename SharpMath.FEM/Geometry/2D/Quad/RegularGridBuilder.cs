using SharpMath.FEM.Core;
using SharpMath.Geometry._1D;
using SharpMath.Geometry._2D;
using SharpMath.Geometry._2D.Shapes;
using SharpMath.Geometry.Splitting;

namespace SharpMath.FEM.Geometry._2D.Quad;

public class RegularGridBuilder : IGridBuilder<Point2D, RegularGridDefinition>
{
    public Grid<Point2D, IElement> Build(RegularGridDefinition definition)
    {
        var (_, xSplitters, ySplitters, areas, _) = definition;
        
        var (xNodesCount, yNodesCount) = GetSizes(xSplitters, ySplitters);
        var elementsCount = (xNodesCount - 1) * (yNodesCount - 1);
        var nodes = new Point2D[xNodesCount * yNodesCount];
        var elements = new Element[elementsCount];
        var elementIndex = 0;
        
        for (var areaIndex = 0; areaIndex < areas.Length; areaIndex++)
        {
            var area = areas[areaIndex];
            for (var yIndex = area.BottomBoundId; yIndex < area.TopBoundId; yIndex++)
            {
                for (var xIndex = area.LeftBoundId; xIndex < area.RightBoundId; xIndex++)
                {
                    var horizontalLinesSkipped = 0;
                    for (var verticalIntervalId = 0; verticalIntervalId < yIndex; verticalIntervalId++)
                    {
                        horizontalLinesSkipped += ySplitters[verticalIntervalId].Steps;
                    }
                    var verticalLinesSkipped = 0;
                    for (var horizontalIntervalId = 0; horizontalIntervalId < xIndex; horizontalIntervalId++)
                    {
                        verticalLinesSkipped += xSplitters[horizontalIntervalId].Steps;
                    }
                    var subAreaIndexPadding = horizontalLinesSkipped * xNodesCount + verticalLinesSkipped;
                    
                    // todo дженериковый код, нужно обобщить получение маппера для криволинейных
                    var (bottomCurve, horizontalSplitter) = definition.GetCurveWithSplitter(Orientation.Horizontal, yIndex, xIndex);
                    var (topCurve, _) =  definition.GetCurveWithSplitter(Orientation.Horizontal, yIndex + 1, xIndex);
                    var (leftCurve, verticalSplitter) = definition.GetCurveWithSplitter(Orientation.Vertical, xIndex, yIndex);
                    var (rightCurve, _) = definition.GetCurveWithSplitter(Orientation.Vertical, xIndex + 1, yIndex);
                    
                    var masterPoints = SplitMasterArea(horizontalSplitter, verticalSplitter);
                    var pointsMapper = new LinearTemplatePointsMapper([
                        bottomCurve.Start, bottomCurve.End,
                        topCurve.Start, topCurve.End
                    ]);
                    
                    
                    for (var i = 0; i < verticalSplitter.Steps; i++)
                    {
                        var indexPadding = subAreaIndexPadding + i * xNodesCount;
                        for (var j = 0; j < horizontalSplitter.Steps; j++)
                        {
                            IEnumerable<int> indexes = [0, 1, xNodesCount, xNodesCount + 1];
                            var element = new Element(
                                areaIndex, 
                                indexes.Select(x => x + indexPadding).ToArray()
                            );
                            indexPadding++;

                            elements[elementIndex] = element;
                            elementIndex++;
                        }
                    }
                    
                    for (var i = 0; i <= verticalSplitter.Steps; i++)
                    {
                        var indexPadding = subAreaIndexPadding + i * xNodesCount;
                        for (var j = 0; j <= horizontalSplitter.Steps; j++)
                        {
                            var masterPoint = masterPoints[i * (horizontalSplitter.Steps + 1) + j];
                            nodes[indexPadding] = pointsMapper.Map(masterPoint);

                            indexPadding++;
                        }
                    }
                }   
            }
        }
        
        return new Grid<Point2D, IElement>(new IrregularPointsCollection(nodes), elements);
    }

    private static (int XNodesCount, int YNodesCount) GetSizes(
        IEnumerable<ICurveSplitter> xSplitters, 
        IEnumerable<ICurveSplitter> ySplitters
        ) => (xSplitters.Sum(x => x.Steps) + 1, ySplitters.Sum(x => x.Steps) + 1);

    private Point2D[] SplitMasterArea(ICurveSplitter xSplitter, ICurveSplitter ySplitter)
    {
        var line = new Line1D(0, 1);
        var (xNodesCount, yNodesCount) = GetSizes([xSplitter], [ySplitter]);
        var points = new Point2D[xNodesCount * yNodesCount];
        var xValues = xSplitter.EnumerateValues(line).ToArray();
        var yValues = ySplitter.EnumerateValues(line).ToArray();

        for (var i = 0; i < yValues.Length; i++)
        {
            var y = yValues[i];

            for (var j = 0; j < xValues.Length; j++)
            {
                var x = xValues[j];
                points[i * xValues.Length + j] = new Point2D(x, y);
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
        var (points, xSplitters, ySplitters, _, _) = definition;

        var startIndex = intervalId;
        var endIndex = intervalId + 1;
        var layerIndex = lineId;

        var (start, end) = orientation switch
        {
            Orientation.Horizontal => (points[layerIndex, startIndex], points[layerIndex, endIndex]),
            Orientation.Vertical => (points[startIndex, layerIndex], points[endIndex, layerIndex]),
            _ => throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null)
        };
        
        var curve = new Line2D(start, end);
        
        var splitter = orientation switch
        {
            Orientation.Horizontal => xSplitters[intervalId],
            Orientation.Vertical => ySplitters[intervalId],
            _ => throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null)
        };
        
        return (curve, splitter);
    }
}