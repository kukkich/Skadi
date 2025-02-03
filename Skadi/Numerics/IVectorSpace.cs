using System.Numerics;

namespace Skadi.Numerics;

public interface IVectorSpace<out TField, TSelf> :
    IReadOnlyList<TField>
    where TSelf : IVectorSpace<TField, TSelf>
    where TField :
    IAdditionOperators<TField, TField, TField>,
    IAdditiveIdentity<TField, TField>,
    IUnaryPlusOperators<TField, TField>,
    ISubtractionOperators<TField, TField, TField>,
    IUnaryNegationOperators<TField, TField>,
    IMultiplyOperators<TField, TField, TField>,
    IMultiplicativeIdentity<TField, TField>,
    IEqualityOperators<TField, TField, bool>
{
}