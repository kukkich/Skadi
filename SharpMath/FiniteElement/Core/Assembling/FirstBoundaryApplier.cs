using SharpMath.FiniteElement.Core.Assembling.Boundary;

namespace SharpMath.FiniteElement.Core.Assembling;

public interface IFirstBoundaryApplier<in TMatrix>
{
    public void Apply(TMatrix matrix, FirstCondition condition);
}