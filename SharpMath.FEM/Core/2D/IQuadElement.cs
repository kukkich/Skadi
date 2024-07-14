namespace SharpMath.FEM.Core._2D;

public interface IQuadElement : IElement2D
{
    public int LeftBoundId { get; }
    public int RightBoundId { get; }
    public int TopBoundId { get; }
    public int BottomBoundId { get; }
}