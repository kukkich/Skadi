using System.Linq.Expressions;
using Skadi.Geometry._2D;

namespace Skadi.Expressions;

public static class ExpressionFactory
{
    public static Expression<Func<Vector2D, double>> CreatePointBasedExpression(string expression)
    {
        var pointParam = Expression.Parameter(typeof(Vector2D), "point");

        var xProperty = Expression.PropertyOrField(pointParam, nameof(Vector2D.X));
        var yProperty = Expression.PropertyOrField(pointParam, nameof(Vector2D.Y));

        var parameterReplacer = new ParameterExpressionReplacer(
            ("x", xProperty),
            ("y", yProperty)
        );

        var parsedBody = System.Linq.Dynamic.Core.DynamicExpressionParser
            .ParseLambda([pointParam], typeof(double), expression)
            .Body;

        var body = parameterReplacer.Visit(parsedBody);

        return Expression.Lambda<Func<Vector2D, double>>(body, pointParam);
    }
}