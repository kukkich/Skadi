using System.Linq.Expressions;

namespace Skadi.Expressions;

public class ParameterExpressionReplacer: ExpressionVisitor
{
    private readonly (string Name, Expression Replacement)[] _replacements;

    public ParameterExpressionReplacer(params (string Name, Expression Replacement)[] replacements)
    {
        _replacements = replacements;
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        foreach (var (name, replacement) in _replacements)
        {
            if (node.Name == name)
            {
                return replacement;
            }
        }
        return base.VisitParameter(node);
    }
}