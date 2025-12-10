using System;
using System.Globalization;
using System.Linq.Expressions;

namespace ShaderGen;

/// <summary>
/// A class for generating shader code from C# expressions.
/// </summary>
public class ShaderGenerator
{
    /// <summary>
    /// Generates shader code from a C# expression.
    /// </summary>
    /// <param name="expression">The C# expression representing the shader logic.</param>
    /// <returns>The generated shader code.</returns>
    public string Generate(Expression<Func<float>> expression)
    {
        var glslExpression = ParseExpression(expression.Body);

        return $@"
#version 330 core
out vec4 FragColor;

void main()
{{
    float result = {glslExpression};
    FragColor = vec4(result, result, result, 1.0);
}}
";
    }

    private string ParseExpression(Expression expression)
    {
        if (expression is ConstantExpression constant)
        {
            if (constant.Value is float f)
            {
                return f.ToString(CultureInfo.InvariantCulture);
            }
            return constant.Value?.ToString() ?? "null";
        }

        if (expression is BinaryExpression binary)
        {
            var left = ParseExpression(binary.Left);
            var right = ParseExpression(binary.Right);
            string op;
            switch (binary.NodeType)
            {
                case ExpressionType.Add:
                    op = "+";
                    break;
                case ExpressionType.Subtract:
                    op = "-";
                    break;
                case ExpressionType.Multiply:
                    op = "*";
                    break;
                case ExpressionType.Divide:
                    op = "/";
                    break;
                default:
                     throw new NotSupportedException($"Binary operator '{binary.NodeType}' is not supported.");
            }
            return $"({left} {op} {right})";
        }

        throw new NotSupportedException($"Expression type '{expression.NodeType}' is not supported.");
    }
}
