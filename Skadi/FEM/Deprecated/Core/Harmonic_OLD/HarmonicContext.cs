using Skadi.FEM.Core.Geometry;

namespace Skadi.FEM.Deprecated.Core.Harmonic_OLD;

public class HarmonicContext<TPoint, TElement, TMatrix> : Context<TPoint, TElement, TMatrix>
    where TElement : IElement
{
    public required double Frequency { get; set; }
}