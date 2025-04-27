using Skadi.FEM.Assembling;
using Skadi.FEM.Core.Geometry.Edges;
using Skadi.Geometry._2D;

namespace Skadi.FEM.Core.Geometry._2D.Quad;

public class RegularEdgeGridBuilder : IGridBuilder<Vector2D, RegularGridDefinition, IEdgeElement>
{
    private readonly IEdgesPortraitBuilder _edgesPortraitBuilder;

    public RegularEdgeGridBuilder(IEdgesPortraitBuilder edgesPortraitBuilder)
    {
        _edgesPortraitBuilder = edgesPortraitBuilder;
    }
    
    public Grid<Vector2D, IEdgeElement> Build(RegularGridDefinition definition)
    {
        var builder = new RegularGridBuilder();
        var grid = builder.Build(definition);
        var edgesPortrait = _edgesPortraitBuilder.Build(grid.Elements, grid.Nodes.TotalPoints);
        var resolver = new EdgeResolver(edgesPortrait, grid.Elements, new QuadElementEdgeResolver());
        var elements = new List<EdgeElement>(grid.Elements.Length);

        for (var i = 0; i < grid.Elements.Length; i++)
        {
            var element = grid.Elements[i]; 
            var edgeIds = resolver.GetEdgeIdsByElement(i);
            elements.Add(new EdgeElement(element.AreaId, element.NodeIds, edgeIds));
        }
        
        return new Grid<Vector2D, IEdgeElement>(grid.Nodes, elements);
    }
}