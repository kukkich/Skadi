using SharpMath.FiniteElement._2D;
using SharpMath.Geometry.Splitting;

namespace SharpMath.Geometry._2D;

public class GridBuilder
{
    private AxisSplitParameter? _xAxisSplitParameter;
    private AxisSplitParameter? _yAxisSplitParameter;
    private int[]? _materialsId;
    private int _totalXElements;
    private int _totalYElements;

    private int ComputedTotalXElements => _xAxisSplitParameter!.Splitters.Sum(x => x.Steps);
    private int ComputedTotalYElements => _yAxisSplitParameter!.Splitters.Sum(y => y.Steps);


    public GridBuilder SetXAxis(AxisSplitParameter splitParameter)
    {
        _xAxisSplitParameter = splitParameter;
        return this;
    }

    public GridBuilder SetYAxis(AxisSplitParameter splitParameter)
    {
        _yAxisSplitParameter = splitParameter;
        return this;
    }

    public GridBuilder SetMaterials(int[] materialsId)
    {
        _materialsId = materialsId;
        return this;
    }

    public Grid<Point, Element> Build()
    {
        if (_xAxisSplitParameter == null || _yAxisSplitParameter == null)
            throw new ArgumentNullException();

        _totalXElements = ComputedTotalXElements;
        _totalYElements = ComputedTotalYElements;
        var totalNodes = GetTotalNodes();
        var totalElements = GetTotalElements();

        var nodes = new Point[totalNodes];
        var elements = new Element[totalElements];
        _materialsId ??= new int[totalElements];

        Allocate(nodes, elements);

        var j = 0;
        foreach (var (ySection, ySplitter) in _yAxisSplitParameter.SectionWithParameter)
        {
            var yValues = ySplitter.EnumerateValues(ySection);
            if (j > 0) yValues = yValues.Skip(1);

            foreach (var y in yValues)
            {
                var k = 0;

                foreach (var (xSection, xSplitter) in _xAxisSplitParameter.SectionWithParameter)
                {
                    var xValues = xSplitter.EnumerateValues(xSection);
                    if (k > 0) xValues = xValues.Skip(1);

                    foreach (var x in xValues)
                    {
                        var nodeIndex = k + j * (_totalXElements + 1);

                        nodes[nodeIndex].X = x;
                        nodes[nodeIndex].Y = y;

                        if (j > 0 && k > 0)
                        {
                            var elementIndex = k - 1 + (j - 1) * _totalXElements;

                            FillNodeIndexes(j - 1, k - 1, elements[elementIndex].NodeIndexes);

                            var indexes = elements[elementIndex].NodeIndexes;

                            var leftBottom = nodes[indexes[0]];
                            var leftTop = nodes[indexes[2]];
                            var rightBottom = nodes[indexes[1]];
                            
                            elements[elementIndex].Length = rightBottom.X - leftBottom.X;
                            elements[elementIndex].Width = leftTop.Y - leftBottom.Y;

                            elements[elementIndex].MaterialId = _materialsId[elementIndex];
                        }

                        k++;
                    }
                }

                j++;
            }
        }

        return new Grid<Point, Element>(nodes, elements);
    }

    private int GetTotalNodes()
    {
        return (_totalXElements + 1) * (_totalYElements + 1);
    }

    private int GetTotalElements()
    {
        return _totalXElements * _totalYElements;
    }

    private void FillNodeIndexes(int j, int k, int[] indexes)
    {
        indexes[0] = k + j * (_totalXElements + 1);
        indexes[1] = k + 1 + j * (_totalXElements + 1);
        indexes[2] = k + (j + 1) * (_totalXElements + 1);
        indexes[3] = k + 1 + (j + 1) * (_totalXElements + 1);
    }

    private void Allocate(Point[] points, Element[] elements)
    {
        for (int i = 0; i < points.Length; i++)
            points[i] = new Point();

        for (int i = 0; i < elements.Length; i++)
            elements[i] = new Element(new int[4], 0, 0);
    }
}