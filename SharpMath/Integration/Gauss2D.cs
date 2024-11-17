using Microsoft.Extensions.Logging;
using SharpMath.Geometry._1D;
using SharpMath.Geometry._2D;

namespace SharpMath.Integration;

public class Gauss2D : Method<GaussConfig>, IIntegrator2D
{
    public Gauss2D(GaussConfig config, ILogger logger)
        : base(config, logger)
    {
        if (config.Segments != 1)
        {
            throw new NotSupportedException();
        }
    }

    public double Calculate(Func<Point2D, double> f, Line1D xInterval, Line1D yInterval)
    {
        var ySum = 0d;

        var xHalfLength = (xInterval.End - xInterval.Start) / 2;
        var xMid = (xInterval.Start + xInterval.End) / 2;
        var yHalfLength = (yInterval.End - yInterval.Start) / 2;
        var yMid = (yInterval.Start + yInterval.End) / 2;
        
        for (var i = 0; i < Config.Nodes.Count; i++)
        {
            var y = (yHalfLength * Config.Nodes[i] + yMid);
            var xSum = 0d;
            for (var j = 0; j < Config.Nodes.Count; j++)
            {
                var x = xHalfLength * Config.Nodes[j] + xMid;
                xSum += Config.Weights[j] * f(new Point2D(x, y));
            }

            ySum += Config.Weights[i] * xHalfLength * xSum;
        }

        return yHalfLength * ySum;
    }
}

public class GaussConfig
{
    public IReadOnlyList<double> Nodes { get; init; }
    public IReadOnlyList<double> Weights { get; init; }
    public int Segments { get; init; }

    public static GaussConfig Gauss2(int segments) => new()
    {
        Nodes = [-1d / Math.Sqrt(3), 1d / Math.Sqrt(3)],
        Weights = [1d, 1d],
        Segments = segments
    };
    
    public static GaussConfig Gauss3(int segments) => new()
    {
        Nodes = [
            -Math.Sqrt(3d / 5),
            0,
            Math.Sqrt(3d / 5),
        ],
        Weights = [
            5d / 9,
            8d / 9,
            5d / 9
        ],
        Segments = segments
    };
    
    public static GaussConfig Gauss4(int segments) => new()
    {
        Nodes = [
            -Math.Sqrt((3 - 2 * Math.Sqrt(6d/5)) / 7),
            Math.Sqrt((3 - 2 * Math.Sqrt(6d/5)) / 7),
            -Math.Sqrt((3 + 2 * Math.Sqrt(6d/5)) / 7),
            Math.Sqrt((3 + 2 * Math.Sqrt(6d/5)) / 7),
        ],
        Weights = [
            (18d + Math.Sqrt(30)) / 36,
            (18d + Math.Sqrt(30)) / 36,
            (18d - Math.Sqrt(30)) / 36,
            (18d - Math.Sqrt(30)) / 36,
        ],
        Segments = segments
    };
}