using Skadi.FEM.Core;

namespace Skadi.FiniteElement.Core.Harmonic;

public class HarmonicContext<TPoint, TElement, TMatrix> : Context<TPoint, TElement, TMatrix>
    where TElement : IElement
{
    public required double Frequency { get; set; }
}