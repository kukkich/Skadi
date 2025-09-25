using System.Numerics;
using Microsoft.Extensions.Logging;
using Skadi.Geometry._1D;
using Skadi.Geometry._2D;

namespace Skadi.Algorithms.Integration;

public class Gauss2D : Method<GaussConfig>, IIntegrator2D
{
    public Gauss2D(GaussConfig config, ILogger logger)
        : base(config, logger)
    {
        if (config.Segments < 1)
        {
            throw new ArgumentException("The number of segments must be at least 1.");
        }
    }

    public T Calculate<T>(Func<Vector2D, T> f, Line1D xInterval, Line1D yInterval) 
        where T : IAdditiveIdentity<T, T>, IAdditionOperators<T, T, T>, IMultiplyOperators<T, double, T>
    {
        var integral = T.AdditiveIdentity;

        var xSegmentLength = (xInterval.End - xInterval.Start) / Config.Segments;
        var ySegmentLength = (yInterval.End - yInterval.Start) / Config.Segments;

        for (var i = 0; i < Config.Segments; i++)
        {
            for (var j = 0; j < Config.Segments; j++)
            {
                var xStart = xInterval.Start + i * xSegmentLength;
                var xEnd = xStart + xSegmentLength;

                var yStart = yInterval.Start + j * ySegmentLength;
                var yEnd = yStart + ySegmentLength;

                integral += CalculateOnSubInterval(f, xStart, xEnd, yStart, yEnd);
            }
        }

        return integral;
    }

    private T CalculateOnSubInterval<T>(Func<Vector2D, T> f, double xStart, double xEnd, double yStart, double yEnd)
        where T : IAdditiveIdentity<T, T>, IAdditionOperators<T, T, T>, IMultiplyOperators<T, double, T>
    {
        var ySum = T.AdditiveIdentity;

        var xHalfLength = (xEnd - xStart) / 2;
        var xMid = (xStart + xEnd) / 2;
        var yHalfLength = (yEnd - yStart) / 2;
        var yMid = (yStart + yEnd) / 2;

        for (var i = 0; i < Config.Nodes.Count; i++)
        {
            var y = (yHalfLength * Config.Nodes[i] + yMid);
            var xSum = T.AdditiveIdentity;
            for (var j = 0; j < Config.Nodes.Count; j++)
            {
                var x = xHalfLength * Config.Nodes[j] + xMid;
                xSum += f(new Vector2D(x, y)) * Config.Weights[j];
            }

            ySum += xSum * (Config.Weights[i] * xHalfLength);
        }

        return ySum * yHalfLength ;
    }
}

public class GaussConfig
{
    public required IReadOnlyList<double> Nodes { get; init; }
    public required IReadOnlyList<double> Weights { get; init; }
    public int Segments { get; private init; }

    private GaussConfig() { }
    
    public static GaussConfig Gauss2(int segments) => new()
    {
        Nodes = [-1d / Math.Sqrt(3), 1d / Math.Sqrt(3)],
        Weights = [1d, 1d],
        Segments = segments
    };

    public static GaussConfig Gauss3(int segments) => new()
    {
        Nodes =
        [
            -Math.Sqrt(3d / 5),
            0,
            Math.Sqrt(3d / 5),
        ],
        Weights =
        [
            5d / 9,
            8d / 9,
            5d / 9
        ],
        Segments = segments
    };

    public static GaussConfig Gauss4(int segments) => new()
    {
        Nodes =
        [
            -Math.Sqrt((3 - 2 * Math.Sqrt(6d / 5)) / 7),
            Math.Sqrt((3 - 2 * Math.Sqrt(6d / 5)) / 7),
            -Math.Sqrt((3 + 2 * Math.Sqrt(6d / 5)) / 7),
            Math.Sqrt((3 + 2 * Math.Sqrt(6d / 5)) / 7),
        ],
        Weights =
        [
            (18d + Math.Sqrt(30)) / 36,
            (18d + Math.Sqrt(30)) / 36,
            (18d - Math.Sqrt(30)) / 36,
            (18d - Math.Sqrt(30)) / 36,
        ],
        Segments = segments
    };
}