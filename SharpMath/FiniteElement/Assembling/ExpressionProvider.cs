using System.Linq.Expressions;
using SharpMath.FiniteElement.Core.Assembling.Params;

namespace SharpMath.FiniteElement.Assembling;

public class ArrayExpressionProvider: IExpressionProvider
{
    private readonly IReadOnlyList<LambdaExpression> _expressions;

    public ArrayExpressionProvider(IReadOnlyList<LambdaExpression> expressions)
    {
        _expressions = expressions;
    }

    public LambdaExpression GetExpression(int id) => _expressions[id];
}