using SharpMath.FEM.Geometry._2D.Quad;
using SharpMath.Geometry._2D;
using SharpMath.Geometry.Shapes;
using SharpMath.Geometry.Splitting;

namespace SharpMath.FEM.Geometry._2D.Areas;

public class FromSplineGridLayoutBuilder
{
    public (Point2D[,] controlPoints, AreaDefinition[] areas) Build(IEnumerable<ICurve<Point2D>> horizontalBounds, ICurveSplitter splitter)
    {
        var curveBounds = horizontalBounds.ToArray();
        var areasCount = (curveBounds.Length - 1) * splitter.Steps;
        
        var points = new Point2D[curveBounds.Length, splitter.Steps + 1];
        var areas = new AreaDefinition[areasCount];

        var areaId = 0;
        for (var i = 0; i < curveBounds.Length - 1; i++)
        {
            for (var j = 0; j < splitter.Steps; j++)
            {
                areas[areaId] = new AreaDefinition(j, j + 1, i, i + 1, areaId);
                areaId++;
            }
        }

        var layerIndex = 0;
        foreach (var bound in curveBounds)
        {
            var pointOnLayerIndex = 0;
            foreach (var point in splitter.EnumerateValues(bound))
            {
                points[layerIndex, pointOnLayerIndex] = point;
                pointOnLayerIndex++;
            }
            
            layerIndex++;
        }
        
        return (points, areas);
    }
}