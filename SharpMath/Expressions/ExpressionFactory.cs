using System.Linq.Expressions;
using SharpMath.Geometry._2D;

namespace SharpMath.Expressions;

public static class ExpressionFactory
{
    public static Expression<Func<Point2D, double>> CreatePointBasedExpression(string expression)
    {
        var pointParam = Expression.Parameter(typeof(Point2D), "point");

        var xProperty = Expression.PropertyOrField(pointParam, nameof(Point2D.X));
        var yProperty = Expression.PropertyOrField(pointParam, nameof(Point2D.Y));

        var parameterReplacer = new ParameterExpressionReplacer(
            ("x", xProperty),
            ("y", yProperty)
        );

        var parsedBody = System.Linq.Dynamic.Core.DynamicExpressionParser
            .ParseLambda([pointParam], typeof(double), expression)
            .Body;

        var body = parameterReplacer.Visit(parsedBody);

        return Expression.Lambda<Func<Point2D, double>>(body, pointParam);
    }
}