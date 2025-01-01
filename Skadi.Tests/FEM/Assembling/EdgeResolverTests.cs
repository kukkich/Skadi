using Skadi.FEM.Geometry;
using Skadi.FEM.Geometry._2D.Quad;
using Skadi.FiniteElement.Assembling;
using Skadi.Geometry._2D;
using Skadi.Geometry.Splitting;

namespace Skadi.Tests.FEM.Assembling;

public class EdgeResolverTests
{
    private IEdgeResolver _edgeResolver = null!;
    
    [SetUp]
    public void Setup()
    {
        var gridBuilder = new RegularGridBuilder();
        var gridDefinition = new RegularGridDefinition(
            new Point2D[,]
            {
                { new(0, 0), new(1, 0) },
                { new(0, 1), new(1, 1) },
            }, 
            [new UniformSplitter(2)], 
            [new UniformSplitter(3)], 
            [new AreaDefinition(0, 1, 0, 1)],
            []);
        var grid = gridBuilder.Build(gridDefinition);
        
        var elementEdgeResolver = new QuadElementEdgeResolver();
        var portraitBuilder = new EdgesPortraitBuilder(elementEdgeResolver);
        var portrait = portraitBuilder.Build(grid.Elements, grid.Nodes.TotalPoints);
        
        _edgeResolver = new EdgeResolver(portrait, grid.Elements, elementEdgeResolver);
    }

    [TestCase(0, 1, 0)]
    [TestCase(4, 5, 6)]
    [TestCase(4, 7, 8)]
    [TestCase(6, 7, 9)]
    [TestCase(8, 11, 15)]
    [TestCase(9, 10, 14)]
    [TestCase(10, 11, 16)]
    public void EdgeExistAndShouldBeCorrect(int node1, int node2, int expectedEdge)
    {
        var edge = _edgeResolver.GetEdgeId(node1, node2);
        
        Assert.That(edge, Is.EqualTo(expectedEdge));
    }
    
    [TestCase(0, 4)]
    [TestCase(1, 3)]
    [TestCase(3, 1)]
    [TestCase(6, 10)]
    [TestCase(7, 11)]
    [TestCase(4, 9)]
    [TestCase(0, 8)]
    public void EdgeNotExistShouldThrowException(int node1, int node2)
    {
        Assert.Throws<InvalidOperationException>(() => 
            _edgeResolver.GetEdgeId(node1, node2)
        );
    }
    
    [TestCase(0, 12)]
    [TestCase(5, 16)]
    [TestCase(12, 13)]
    [TestCase(11, 12)]
    public void NodesNotExistShouldThrowException(int node1, int node2)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => 
            _edgeResolver.GetEdgeId(node1, node2)
        );
    }

    [TestCase(0, 0, 1)]
    [TestCase(1, 1, 2)]
    [TestCase(2, 0, 3)]
    [TestCase(3, 1, 4)]
    [TestCase(4, 3, 4)]
    [TestCase(5, 2, 5)]
    [TestCase(8, 4, 7)]
    [TestCase(12, 6, 9)]
    [TestCase(15, 8, 11)]
    [TestCase(16, 10, 11)]
    public void GetNodesByEdgeWhenEdgeExistShouldBeCorrect(int edge, int minNodeExpected, int maxNodeExpected)
    {
        var (minNode, maxNode) = _edgeResolver.GetNodesByEdge(edge);
        Assert.Multiple(() =>
        {
            Assert.That(minNode, Is.EqualTo(minNodeExpected));
            Assert.That(maxNode, Is.EqualTo(maxNodeExpected));
        });
    }
    
    [TestCase(-1)]
    [TestCase(17)]
    [TestCase(21)]
    public void GivenEdgeNotExistAndShouldTrowException(int edge)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => 
            _edgeResolver.GetNodesByEdge(edge)
        );
    }

    [TestCase(0, 0, 2, 3, 4)]
    [TestCase(2, 4, 7, 8, 9)]
    [TestCase(3, 6, 8, 10, 11)]
    [TestCase(5, 11, 13, 15, 16)]
    public void ElementEdgesShouldBeCorrect(int elementId, params int[] expectedEdges)
    {
        var edges = _edgeResolver.GetElementEdges(elementId);
        Assert.That(edges.SequenceEqual(expectedEdges), Is.True);
    }
}