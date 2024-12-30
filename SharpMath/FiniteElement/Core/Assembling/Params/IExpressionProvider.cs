using System.Linq.Expressions;

namespace SharpMath.FiniteElement.Core.Assembling.Params;

public interface IExpressionProvider
{
    public LambdaExpression GetExpression(int id);
}