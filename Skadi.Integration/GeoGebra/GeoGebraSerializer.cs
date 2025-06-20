using System.Globalization;
using System.Xml.Linq;
using Skadi.Geometry._2D;

namespace Skadi.Integration.GeoGebra;

public static class GeoGebraSerializer
{
    public static Grid ParseGeoGebraFile(string filePath)
    {
        var points = new List<Point2D>();
        var segments = new List<Segment>();
        var doc = XDocument.Load(filePath);

        // Парсинг точек
        foreach (var element in doc.Descendants("element")
                     .Where(x => x.Attribute("type") is {Value: "point"})
                 )
        {
            var label = element.Attribute("label")?.Value;
            var coords = element.Element("coords");
            if (coords is null || label is null)
            {
                continue;
            }
            
            points.Add(new Point2D
            (
                label,
                new Vector2D
                (
                    ParseValue<double>(coords, "x"),
                    ParseValue<double>(coords, "y")
                )
            ));
        }

        var commands = doc.Descendants("command");
        foreach (var command in commands
                     .Where(x => x.Attribute("name") is { Value: "Segment" })
                 )
        {
            var output = command.Element("output");
            var input = command.Element("input");

            if (output is null || input is null)
            {
                continue;
            }
            
            var segmentName = output.Attribute("a0")?.Value;
            var startPoint = input.Attribute("a0")?.Value;
            var endPoint = input.Attribute("a1")?.Value;

            if (segmentName is null || startPoint is null || endPoint is null)
            {
                continue;
            }
            
            segments.Add(new Segment(segmentName, startPoint, endPoint));
        }

        return new Grid(points.ToArray(), segments.ToArray());
    }

    private static T ParseValue<T>(XElement element, string attributeName)
        where T : IParsable<T>
    {
        return T.Parse
        (
            element.Attribute(attributeName)?.Value 
                ?? throw new InvalidOperationException(attributeName), 
            CultureInfo.InvariantCulture
        );
    }
}