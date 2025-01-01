using Skadi.FEM.Geometry;
using Skadi.Geometry._2D;
using Skadi.FiniteElement.Core.Assembling.Params;

namespace Skadi.FiniteElement._2D.Parameters;

public class ConvectionVelocity : IUniversalParameterProvider<Point2D, Point2D>
{
    private readonly double _maxVelocity;
    private readonly double _height;
    private readonly double _radius;
    
    public ConvectionVelocity(IPointsCollection<Point2D> points, double maxVelocity)
    {
        var lb = points[0];
        var rt = points[points.TotalPoints - 1];
        _height = rt.Y - lb.Y;
        _radius = rt.X - lb.X;
        
        _maxVelocity = maxVelocity;
    }
    
    public Point2D Get(Point2D point)
    {
        var lowerThanFirst = point.Y <= _height * point.X / _radius;
        var lowerThanSecond = point.Y <= _height * (1 - point.X / _radius);
        var heightQuarter = _height / 4;
        var radiusQuarter = _radius / 4;
        var factor = 1 - (lowerThanFirst, lowerThanSecond) switch
        {
            (true, true) => Math.Abs(point.Y - heightQuarter) / heightQuarter,
            (true, false) => Math.Abs(point.X - 3 * radiusQuarter) / radiusQuarter,
            (false, false) => Math.Abs(point.Y - 3 * heightQuarter) / heightQuarter,
            (false, true) => Math.Abs(point.X - radiusQuarter) / radiusQuarter,
        };
        Point2D direction = (lowerThanFirst, lowerThanSecond) switch
        {
            (true, true) => new (1, 0),
            (true, false) => new (0, 1),
            (false, false) => new (-1, 0),
            (false, true) => new (0, -1),
        };
        
        var velocity = direction * factor * _maxVelocity;

        return velocity;
    }
}