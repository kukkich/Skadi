using System.Linq.Expressions;

namespace Skadi.FiniteElement.Core.Assembling.Params;

public interface IExpressionProvider
{
    public LambdaExpression GetExpression(int id);
}