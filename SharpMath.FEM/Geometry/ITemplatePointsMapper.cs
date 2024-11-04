namespace SharpMath.FEM.Geometry;

public interface ITemplatePointsMapper<TPoint>
{
    public TPoint Map(TPoint point);
}