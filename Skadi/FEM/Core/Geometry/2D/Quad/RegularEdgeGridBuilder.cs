using Skadi.FEM.Assembling.Edges;
using Skadi.FEM.Core.Geometry.Edges;
using Skadi.Geometry._2D;

namespace Skadi.FEM.Core.Geometry._2D.Quad;

public class RegularEdgeGridBuilder(IEdgesPortraitBuilder edgesPortraitBuilder)
    : IGridBuilder<Vector2D, RegularGridDefinition, IEdgeElement>
{
    public Grid<Vector2D, IEdgeElement> Build(RegularGridDefinition definition)
    {
        var builder = new RegularGridBuilder();
        var grid = builder.Build(definition);
        var edgesPortrait = edgesPortraitBuilder.Build(grid.Elements, grid.Nodes.TotalPoints);
        var resolver = new EdgeResolver(edgesPortrait, grid.Elements, new QuadElementEdgeResolver());
        var elements = new IEdgeElement[grid.Elements.Length];

        for (var i = 0; i < grid.Elements.Length; i++)
        {
            var element = grid.Elements[i]; 
            var edgeIds = resolver.GetEdgeIdsByElement(i);
            elements[i] = new EdgeElement(element.AreaId, element.NodeIds, edgeIds);
        }
        
        return new Grid<Vector2D, IEdgeElement>(grid.Nodes, elements);
    }
}