using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ShaderGen;

/// <summary>
/// A class for generating shader code from C# expressions.
/// </summary>
public class ShaderGenerator
{
    /// <summary>
    /// Generates a shader from a C# expression that returns a Vec4 color.
    /// </summary>
    public string Generate<TUniforms>(Expression<Func<TUniforms, Vec4>> expression)
    {
        var uniforms = typeof(TUniforms).GetProperties()
            .Select(p => $"uniform {Glsl.FromType(p.PropertyType)} {p.Name.ToLower()};")
            .ToList();

        var glslExpression = ParseExpression(expression.Body);

        var sb = new StringBuilder();
        sb.AppendLine("#version 330 core");
        sb.AppendLine(string.Join("\n", uniforms));
        sb.AppendLine("out vec4 FragColor;");
        sb.AppendLine("void main()");
        sb.AppendLine("{");
        sb.AppendLine($"    FragColor = {glslExpression};");
        sb.AppendLine("}");
        return sb.ToString();
    }

    private string ParseExpression(Expression expression)
    {
        switch (expression)
        {
            case ConstantExpression constant:
                if (constant.Value is float f)
                {
                    var floatString = f.ToString(CultureInfo.InvariantCulture);
                    if (!floatString.Contains('.') && !floatString.Contains('e', StringComparison.InvariantCultureIgnoreCase))
                    {
                        return floatString + ".0";
                    }
                    return floatString;
                }
                return constant.Value?.ToString() ?? "null";

            case BinaryExpression binary:
            {
                var left = ParseExpression(binary.Left);
                var right = ParseExpression(binary.Right);
                var op = binary.NodeType switch
                {
                    ExpressionType.Add => "+",
                    ExpressionType.Subtract => "-",
                    ExpressionType.Multiply => "*",
                    ExpressionType.Divide => "/",
                    _ => throw new NotSupportedException($"Binary operator '{binary.NodeType}' is not supported.")
                };
                return $"({left} {op} {right})";
            }

            case NewExpression newExp:
                if (newExp.Constructor?.DeclaringType == typeof(Vec4))
                {
                    var args = newExp.Arguments.Select(ParseExpression);
                    return $"vec4({string.Join(", ", args)})";
                }
                if (newExp.Constructor?.DeclaringType == typeof(Vec3))
                {
                    var args = newExp.Arguments.Select(ParseExpression);
                    return $"vec3({string.Join(", ", args)})";
                }
                if (newExp.Constructor?.DeclaringType == typeof(Vec2))
                {
                    var args = newExp.Arguments.Select(ParseExpression);
                    return $"vec2({string.Join(", ", args)})";
                }
                break;

            case ParameterExpression parameter:
                return parameter.Name;

            case MemberExpression member:
                // Uniform access: e.g., uniforms.Time
                if (member.Expression is ParameterExpression)
                {
                    return member.Member.Name.ToLower();
                }

                // Vector member access: e.g., vec.X
                if ((member.Member.DeclaringType == typeof(Vec2) ||
                     member.Member.DeclaringType == typeof(Vec3) ||
                     member.Member.DeclaringType == typeof(Vec4))
                    && member.Expression != null)
                {
                    var parent = ParseExpression(member.Expression);
                    return $"{parent}.{member.Member.Name.ToLower()}";
                }
                break;

            case UnaryExpression unary:
                if (unary.NodeType == ExpressionType.Convert)
                {
                    return ParseExpression(unary.Operand);
                }
                break;

            case MethodCallExpression call:
                if (call.Method.DeclaringType == typeof(Math) || call.Method.DeclaringType == typeof(ShaderMath))
                {
                    var args = call.Arguments.Select(ParseExpression);
                    var functionName = call.Method.Name.ToLower();
                    return $"{functionName}({string.Join(", ", args)})";
                }

                var declaringType = call.Method.DeclaringType;
                if (declaringType == typeof(Vec2) || declaringType == typeof(Vec3) || declaringType == typeof(Vec4))
                {
                    var left = ParseExpression(call.Arguments[0]);
                    var right = ParseExpression(call.Arguments[1]);
                    var op = call.Method.Name switch
                    {
                        "op_Addition" => "+",
                        "op_Subtraction" => "-",
                        "op_Multiply" => "*",
                        "op_Division" => "/",
                        _ => throw new NotSupportedException($"Unsupported vector operation: {call.Method.Name}")
                    };
                    return $"({left} {op} {right})";
                }
                break;
        }

        throw new NotSupportedException($"Expression type '{expression.NodeType}' is not supported.");
    }
}
