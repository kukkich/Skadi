namespace Skadi.FEM.Core.Geometry;

public interface ITemplatePointsMapper<TPoint>
{
    public TPoint Map(TPoint point);
}