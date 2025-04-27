using Skadi.FEM.Assembling;
using Skadi.FEM.Core.Geometry;
using Skadi.FEM.Core.Geometry._2D.Quad;
using Skadi.Geometry._2D;
using Skadi.Geometry.Splitting;

namespace Skadi.Tests.FEM.Core;

[TestOf(typeof(RegularEdgeGridBuilder))]
public class RegularEdgeGridBuilderTest
{
    private IGridBuilder<Vector2D, RegularGridDefinition, IEdgeElement> builder = null!;
    
    [SetUp]
    public void SetUp()
    {
        builder = new RegularEdgeGridBuilder(new EdgesPortraitBuilder(new QuadElementEdgeResolver()));
    }

    [Test]
    public void Grid1X1()
    {
        var definition = new RegularGridDefinition
        (
            new Vector2D[,]
            {
                {new(1, -1), new(3, -1)},
                {new(1, 2), new(1, 2)},
            },
            [new UniformSplitter(1)],
            [new UniformSplitter(1)],
            [new AreaDefinition(0, 1, 0, 1)],
            []
        );

        var grid = builder.Build(definition);
        
        Assert.That(grid.Elements.Single().EdgeIds, Is.EqualTo(new[] {0, 1, 2, 3}).AsCollection);
    }
    
    [TestCase(0, 0, 3, 4, 5)]
    [TestCase(2, 12, 17, 18, 19)]
    [TestCase(3, 7, 11, 13, 14)]
    [TestCase(6, 16, 20, 22, 23)]
    [TestCase(7, 1, 4, 6, 7)]
    [TestCase(8, 2, 6, 8, 9)]
    public void Grid3X3(int elementId, params int[] expectedEdges)
    {
        var definition = new RegularGridDefinition
        (
            new Vector2D[,]
            {
                {new(1, -1), new(2, -1), new(3, -1), new(4, -1)},
                {new(1, 2), new(2, 2), new(3, 2), new(4, 2)},
                {new(1, 5), new(2, 5), new(3, 5), new(4, 5)},
                {new(1, 7), new(2, 7), new(3, 7), new(4, 7)},
            },
            [new UniformSplitter(1), new UniformSplitter(1), new UniformSplitter(1)],
            [new UniformSplitter(1), new UniformSplitter(1), new UniformSplitter(1)],
            [new AreaDefinition(0, 1, 0, 3), new AreaDefinition(1, 3, 1, 3), new AreaDefinition(1, 3, 0, 1)],
            []
        );

        var grid = builder.Build(definition);
        
        Assert.That(grid.Elements[elementId].EdgeIds, Is.EqualTo(expectedEdges).AsCollection);
    }
}