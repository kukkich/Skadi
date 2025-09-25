using System.Linq.Expressions;
using Skadi.FEM.Core.Assembling.Params;

namespace Skadi.FEM.Assembling;

public class ArrayExpressionProvider(IReadOnlyList<LambdaExpression> expressions) : IExpressionProvider
{
    public LambdaExpression GetExpression(int id) => expressions[id];
}