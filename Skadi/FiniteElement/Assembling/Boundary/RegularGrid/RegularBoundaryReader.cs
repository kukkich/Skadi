using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;
using Skadi.Expressions;
using Skadi.FiniteElement.Core.Assembling.Boundary;

namespace Skadi.FiniteElement.Assembling.Boundary.RegularGrid;

public class RegularBoundaryReader(IConfiguration configuration)
{
    public (
        IReadOnlyList<RegularBoundaryCondition> Conditions,
        IReadOnlyList<LambdaExpression> lambdaExpressions
        ) Get()
    {
        var path = configuration["Boundary"] ?? throw new Exception("Boundary path not found in configuration.");
        using var reader = new StreamReader(path);

        var firstLine = reader.ReadLine();
        if (firstLine == null)
        {
            throw new Exception("File is empty.");
        }

        var (conditions, expressionsCount) = ReadConditions(firstLine, reader);
        
        var expressions = new LambdaExpression[expressionsCount];
        var line = reader.ReadLine();
        for (var i = 0; !string.IsNullOrWhiteSpace(line) && i < expressionsCount; i++)
        {
            var expression = ExpressionFactory.CreatePointBasedExpression(line);
            expressions[i] = expression;
            line = reader.ReadLine();
        }
        
        return (conditions, expressions);
    }

    private static (RegularBoundaryCondition[] conditions, int expressionsCount) ReadConditions(string firstLine, StreamReader reader)
    {
        var boundaryCount = int.Parse(firstLine);
        var conditions = new RegularBoundaryCondition[boundaryCount];
        var maxExpressionId = 0;
        for (var i = 0; i < boundaryCount; i++)
        {
            var line = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
            {
                throw new Exception($"Missing data for line {i + 1}.");
            }
            var values = line.Split(' ');
            if (values.Length != 6)
            {
                throw new Exception($"Invalid data for line {i + 1}.");
            }
            
            conditions[i] = new RegularBoundaryCondition
            {
                ExpressionId = int.Parse(values[0]),
                Type = (BoundaryConditionType) int.Parse(values[1]),
                LeftBoundId = int.Parse(values[2]),
                RightBoundId = int.Parse(values[3]),
                BottomBoundId = int.Parse(values[4]),
                TopBoundId = int.Parse(values[5]),
            };
            conditions[i].EnsureValid();
            maxExpressionId = int.Max(maxExpressionId, conditions[i].ExpressionId);
        }

        return (conditions, maxExpressionId + 1);
    }
}