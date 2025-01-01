using Skadi.FEM.Core.Assembling.Boundary;
using Skadi.FEM.Core.Geometry._2D;

namespace Skadi.FEM.Assembling.Boundary.RegularGrid;

public record RegularBoundaryCondition
{
    public Orientation Orientation
    {
        get
        {
            if (LeftBoundId == RightBoundId)
            {
                return Orientation.Vertical;
            }
            if (BottomBoundId == TopBoundId)
            {
                return Orientation.Horizontal;
            }
            throw new InvalidOperationException();
        }
    }
    public int ExpressionId { get; init; }
    public BoundaryConditionType Type { get; init; }
    public int LeftBoundId { get; init; } 
    public int RightBoundId { get; init; }
    public int BottomBoundId { get; init; }
    public int TopBoundId { get; init; }

    public void EnsureValid()
    {
        if (LeftBoundId != RightBoundId && TopBoundId != BottomBoundId)
        {
            throw new InvalidOperationException();
        }
    }
}