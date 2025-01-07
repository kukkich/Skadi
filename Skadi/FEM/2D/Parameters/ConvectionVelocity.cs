using Skadi.FEM.Core.Assembling.Params;
using Skadi.FEM.Core.Geometry;
using Skadi.Geometry._2D;

namespace Skadi.FEM._2D.Parameters;

public class ConvectionVelocity : IUniversalParameterProvider<Vector2D, Vector2D>
{
    private readonly double _maxVelocity;
    private readonly double _height;
    private readonly double _radius;
    
    public ConvectionVelocity(IPointsCollection<Vector2D> points, double maxVelocity)
    {
        var lb = points[0];
        var rt = points[points.TotalPoints - 1];
        _height = rt.Y - lb.Y;
        _radius = rt.X - lb.X;
        
        _maxVelocity = maxVelocity;
    }
    
    public Vector2D Get(Vector2D vector)
    {
        var lowerThanFirst = vector.Y <= _height * vector.X / _radius;
        var lowerThanSecond = vector.Y <= _height * (1 - vector.X / _radius);
        var heightQuarter = _height / 4;
        var radiusQuarter = _radius / 4;
        var factor = 1 - (lowerThanFirst, lowerThanSecond) switch
        {
            (true, true) => Math.Abs(vector.Y - heightQuarter) / heightQuarter,
            (true, false) => Math.Abs(vector.X - 3 * radiusQuarter) / radiusQuarter,
            (false, false) => Math.Abs(vector.Y - 3 * heightQuarter) / heightQuarter,
            (false, true) => Math.Abs(vector.X - radiusQuarter) / radiusQuarter,
        };
        Vector2D direction = (lowerThanFirst, lowerThanSecond) switch
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