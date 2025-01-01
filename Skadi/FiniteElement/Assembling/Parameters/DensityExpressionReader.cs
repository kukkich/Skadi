using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;
using Skadi.Expressions;

namespace Skadi.FiniteElement.Assembling.Parameters;

public class DensityExpressionReader(IConfiguration configuration)
{
    private const string EndMark = "---";
    
    public LambdaExpression GetDensity()
    {
        var path = configuration["Density"] 
                   ?? throw new Exception("Density path not found in configuration.");
        var fileContent = File.ReadAllText(path);
        
        if (fileContent.Contains(EndMark))
        {
            fileContent = fileContent[..fileContent.IndexOf(EndMark, StringComparison.Ordinal)];
        }

        var expression = ExpressionFactory.CreatePointBasedExpression(fileContent);
        return expression;
    }
}