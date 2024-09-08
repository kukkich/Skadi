using SharpMath.Geometry._2D;
using SharpMath.Geometry.Splitting;

namespace SharpMath.FEM.Geometry._2D.Quad;

public class GridDefinition
{
    public Point2D[,] ControlPoints { get; set; }
    public ICurveSplitter[] XSplitters { get; set; }
    public ICurveSplitter[] YSplitters { get; set; }
    public AreaDefinition[] Areas { get; set; }
    // todo перечисление кривых границ с параметрами с. 467
}