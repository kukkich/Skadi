using System.Globalization;
using Microsoft.Extensions.Configuration;
using SharpMath.Geometry._2D;
using SharpMath.Geometry.Splitting;

namespace SharpMath.FEM.Geometry._2D.Quad.IO;

public class GridDefinitionReader(IConfiguration configuration) : IGridDefinitionProvider<RegularGridDefinition>
{
    public RegularGridDefinition Get()
    {
        var gridPath = configuration["GridPath"] ?? throw new Exception("GridPath not found in configuration.");
        var splitPath = configuration["SplitPath"] ?? throw new Exception("SplitPath not found in configuration.");
        
        using var gridReader = new StreamReader(gridPath);
        
        var controlPoints = ReadPoints(gridReader);
        var areas = ReadAreas(gridReader);

        var (xSplitters, ySplitters) = ReadSplitters(splitPath);
        CurveBoundDefinition[] curveBounds = [];

        return new RegularGridDefinition(controlPoints, xSplitters, ySplitters, areas, curveBounds);
    }

    private static (ICurveSplitter[] xSplitter, ICurveSplitter[] ySplitters) ReadSplitters(string splitPath)
    {
        using var splitReader = new StreamReader(splitPath);
        var nestingLine = splitReader.ReadLine();
        if (nestingLine == null)
        {
            throw new Exception("Missing splitter data for nestingLine.");
        }

        var nestingArgs = nestingLine.Split(' ')
            .Select(int.Parse)
            .ToArray();
        if (nestingArgs.Length != 2)
        {
            throw new Exception($"NestingArgs has invalid format: {nestingLine}");
        }
        var (nestingX, nestingY) = (nestingArgs[0], nestingArgs[1]);
        
        
        var xLine = splitReader.ReadLine();
        if (xLine == null)
        {
            throw new Exception("Missing splitter data for X.");
        }

        var xData = xLine.Split(',');
        if (xData.Length == 0)
        {
            throw new Exception("Invalid splitter data for X.");
        }

        var xSplitters = new ICurveSplitter[xData.Length];
        for (var i = 0; i < xData.Length; i++)
        {
            var values = xData[i].Trim().Split();
            if (values.Length != 2)
            {
                throw new Exception($"Invalid format for X-splitter at index {i}.");
            }

            var nx = int.Parse(values[0]);
            var cx = double.Parse(values[1], CultureInfo.InvariantCulture);

            cx = cx > 0 
                ? cx 
                : 1d / cx;
            
            xSplitters[i] = GetSplitter(nx * nestingX, cx);
        }

        var yLine = splitReader.ReadLine();
        if (yLine == null)
        {
            throw new Exception("Missing splitter data for Y.");
        }

        var yData = yLine.Split(',');
        if (yData.Length == 0)
        {
            throw new Exception("Invalid splitter data for Y.");
        }

        var ySplitters = new ICurveSplitter[yData.Length];
        for (var i = 0; i < yData.Length; i++)
        {
            var values = yData[i].Trim().Split();
            if (values.Length != 2)
            {
                throw new Exception($"Invalid format for Y-splitter at index {i}.");
            }

            var ny = int.Parse(values[0]);
            var cy = double.Parse(values[1], CultureInfo.InvariantCulture);
            
            cy = cy > 0 
                ? cy 
                : 1d / cy;
            
            ySplitters[i] = GetSplitter(ny * nestingY, cy);
        }

        return (xSplitters, ySplitters);
    }

    private static ICurveSplitter GetSplitter(int steps, double dischargeRatio)
    {
        return dischargeRatio switch
        {
            1d => new UniformSplitter(steps),
            < 0 => new ProportionalSplitter(steps, 1 / Math.Abs(dischargeRatio)), 
            > 0 => new ProportionalSplitter(steps, dischargeRatio),
            _ => throw new ArgumentOutOfRangeException(nameof(dischargeRatio), dischargeRatio, null)
        };
    }

    private static AreaDefinition[] ReadAreas(StreamReader reader)
    {
        var areasCountLine = reader.ReadLine();
        if (areasCountLine == null)
        {
            throw new Exception("Missing areas count.");
        }

        var areasLength = int.Parse(areasCountLine);

        var areas = new AreaDefinition[areasLength];

        for (var i = 0; i < areasLength; i++)
        {
            var areaLine = reader.ReadLine();
            if (areaLine == null)
            {
                throw new Exception($"Missing data for area {i}.");
            }

            var areaParams = areaLine.Split(',');
            if (areaParams.Length != 5)
            {
                areaParams = areaLine.Split(' ');
                if (areaParams.Length != 5)
                {
                    throw new Exception($"Invalid number of parameters for area {i}.");
                }
            }

            var materialId = int.Parse(areaParams[0].Trim());
            var leftBoundId = int.Parse(areaParams[1].Trim());
            var rightBoundId = int.Parse(areaParams[2].Trim());
            var bottomBoundId = int.Parse(areaParams[3].Trim());
            var topBoundId = int.Parse(areaParams[4].Trim());

            areas[i] = new AreaDefinition(leftBoundId, rightBoundId, bottomBoundId, topBoundId, materialId);
        }

        return areas;
    }

    private static Point2D[,] ReadPoints(StreamReader reader)
    {
        var firstLine = reader.ReadLine();
        if (firstLine == null)
        {
            throw new Exception("File is empty.");
        }
            
        var gridSize = firstLine.Split();
        if (gridSize.Length != 2)
        {
            throw new Exception("Invalid grid size format.");
        }

        var kx = int.Parse(gridSize[0]);
        var ky = int.Parse(gridSize[1]);

        var controlPoints = new Point2D[ky, kx];
        for (var i = 0; i < ky; i++)
        {
            var line = reader.ReadLine();
            if (line == null)
            {
                throw new Exception($"Missing data for line {i}.");
            }

            var points = line.Split(',');
            if (points.Length != kx)
            {
                throw new Exception($"Invalid number of points for line {i}.");
            }

            for (var j = 0; j < kx; j++)
            {
                var point = points[j].Trim().Split();
                if (point.Length != 2)
                {
                    throw new Exception($"Invalid point format at ({i}, {j}).");
                }

                controlPoints[i, j] = new Point2D(double.Parse(point[0]), double.Parse(point[1]));
            }
        }

        return controlPoints;
    }
}