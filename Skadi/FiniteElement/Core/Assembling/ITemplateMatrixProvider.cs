using Skadi.Matrices;

namespace Skadi.FiniteElement.Core.Assembling;

public interface ITemplateMatrixProvider
{
    public ImmutableMatrix GetMatrix();
}