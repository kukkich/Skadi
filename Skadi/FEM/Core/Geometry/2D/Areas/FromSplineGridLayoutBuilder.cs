using Skadi.FEM.Core.Geometry._2D.Quad;
using Skadi.Geometry._2D;
using Skadi.Geometry.Shapes;
using Skadi.Geometry.Splitting;

namespace Skadi.FEM.Core.Geometry._2D.Areas;

public class FromSplineGridLayoutBuilder
{
    public (Vector2D[,] controlPoints, AreaDefinition[] areas) Build(IEnumerable<IParametricCurve<Vector2D>> horizontalBounds, ICurveSplitter splitter)
    {
        var curveBounds = horizontalBounds.ToArray();
        var areasCount = (curveBounds.Length - 1) * splitter.Steps;
        
        var points = new Vector2D[curveBounds.Length, splitter.Steps + 1];
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