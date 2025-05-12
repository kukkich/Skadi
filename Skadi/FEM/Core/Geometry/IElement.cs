namespace Skadi.FEM.Core.Geometry;

public interface IElement
{
    public int AreaId { get; }
    public IReadOnlyList<int> NodeIds { get; }
    
    // Как вариант решения проблемы, чтобы каждый конкретный наследник мог создавать экземпляр именно себя
    // чтобы не приходилось потом маппить Element -> EdgeElement
    // IElement<TSelf> where TSelf : IElement<TSelf>
    // public static abstract TSelf Create(IReadOnlyList<int> nodeIds, int areaId);
}