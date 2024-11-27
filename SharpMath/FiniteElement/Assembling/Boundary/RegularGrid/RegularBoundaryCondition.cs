using SharpMath.FEM.Geometry._2D.Quad;
using SharpMath.FiniteElement.Core.Assembling.Boundary;

namespace SharpMath.FiniteElement.Assembling.Boundary.RegularGrid;

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