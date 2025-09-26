using System.Linq.Expressions;

namespace Skadi.Expressions;

public class ParameterExpressionReplacer(params (string Name, Expression Replacement)[] replacements)
    : ExpressionVisitor
{
    protected override Expression VisitParameter(ParameterExpression node)
    {
        foreach (var (name, replacement) in replacements)
        {
            if (node.Name == name)
            {
                return replacement;
            }
        }
        return base.VisitParameter(node);
    }
}