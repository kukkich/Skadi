using System.Linq.Expressions;

namespace Skadi.FEM.Core.Assembling.Params;

public interface IExpressionProvider
{
    public LambdaExpression GetExpression(int id);
}