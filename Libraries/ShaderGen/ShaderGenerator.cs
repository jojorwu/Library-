using System;
using System.Collections.Generic;
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
    private class ParsingContext
    {
        public HashSet<string> DeclaredVariables { get; } = new();
    }

    /// <summary>
    /// Generates a shader from a C# expression that returns a Vec4 color.
    /// </summary>
    public string Generate<TUniforms>(Expression<Func<TUniforms, Vec4>> expression)
    {
        var uniforms = typeof(TUniforms).GetProperties()
            .Select(p => $"uniform {Glsl.FromType(p.PropertyType)} {p.Name.ToLower()};")
            .ToList();

        var sb = new StringBuilder();
        sb.AppendLine("#version 330 core");
        sb.AppendLine(string.Join("\n", uniforms));
        sb.AppendLine("out vec4 FragColor;");
        sb.AppendLine("void main()");
        sb.AppendLine("{");

        var context = new ParsingContext();
        string resultExpression;

        if (expression.Body is BlockExpression block)
        {
            var statements = block.Expressions.Take(block.Expressions.Count - 1);
            foreach (var stmt in statements)
            {
                sb.AppendLine("    " + ParseStatement(stmt, context, block.Variables));
            }

            // The last expression in a block is the return value
            if (block.Expressions.Last().NodeType != ExpressionType.Default)
            {
                resultExpression = ParseExpression(block.Expressions.Last(), context);
            }
            else
            {
                resultExpression = "vec4(0.0)";
            }
        }
        else
        {
            resultExpression = ParseExpression(expression.Body, context);
        }

        sb.AppendLine($"    FragColor = {resultExpression};");
        sb.AppendLine("}");
        return sb.ToString();
    }

    private string ParseStatement(Expression expression, ParsingContext context, IReadOnlyCollection<ParameterExpression> blockVariables)
    {
        if (expression is UnaryExpression ue && ue.NodeType == ExpressionType.Convert)
        {
            expression = ue.Operand;
        }

        if (expression is BinaryExpression binary && binary.NodeType == ExpressionType.Assign)
        {
            var variable = (ParameterExpression)binary.Left;
            var variableName = variable.Name;
            var value = ParseExpression(binary.Right, context);

            if (blockVariables.Contains(variable) && variableName != null && !context.DeclaredVariables.Contains(variableName))
            {
                context.DeclaredVariables.Add(variableName);
                var type = Glsl.FromType(variable.Type);
                return $"{type} {variableName} = {value};";
            }
            return $"{variableName} = {value};";
        }

        if (expression is ConditionalExpression conditional)
        {
            var test = ParseExpression(conditional.Test, context);
            var ifTrue = ParseBlock(conditional.IfTrue, context);
            var sb = new StringBuilder();
            sb.Append($"if ({test}) {ifTrue}");

            if (conditional.IfFalse != null && conditional.IfFalse.NodeType != ExpressionType.Default)
            {
                var ifFalse = ParseBlock(conditional.IfFalse, context);
                sb.Append($" else {ifFalse}");
            }

            return sb.ToString();
        }

        if (expression is MethodCallExpression call && call.Method.DeclaringType == typeof(ShaderMath) && call.Method.Name == "For")
        {
            var initializer = ParseStatement(((LambdaExpression)call.Arguments[0]).Body, context, blockVariables);
            var condition = ParseExpression(((LambdaExpression)call.Arguments[1]).Body, context);
            var iterator = ParseStatement(((LambdaExpression)call.Arguments[2]).Body, context, blockVariables);
            var body = ParseBlock(((LambdaExpression)call.Arguments[3]).Body, context);

            return $"for ({initializer.TrimEnd(';')}; {condition}; {iterator.TrimEnd(';')}) {body}";
        }

        return ParseExpression(expression, context) + ";";
    }

    private string ParseBlock(Expression expression, ParsingContext context)
    {
        if (expression is BlockExpression block)
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            foreach (var stmt in block.Expressions.Where(s => s.NodeType != ExpressionType.Default))
            {
                sb.AppendLine("    " + ParseStatement(stmt, context, block.Variables));
            }
            sb.AppendLine("}");
            return sb.ToString();
        }

        return "{ " + ParseStatement(expression, context, new List<ParameterExpression>()) + " }";
    }

    private string ParseExpression(Expression expression, ParsingContext context)
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

                if (constant.Value is Vec2 v2)
                {
                    return $"vec2({v2.X.ToString(CultureInfo.InvariantCulture)}, {v2.Y.ToString(CultureInfo.InvariantCulture)})";
                }
                if (constant.Value is Vec3 v3)
                {
                    return $"vec3({v3.X.ToString(CultureInfo.InvariantCulture)}, {v3.Y.ToString(CultureInfo.InvariantCulture)}, {v3.Z.ToString(CultureInfo.InvariantCulture)})";
                }
                if (constant.Value is Vec4 v4)
                {
                    return $"vec4({v4.X.ToString(CultureInfo.InvariantCulture)}, {v4.Y.ToString(CultureInfo.InvariantCulture)}, {v4.Z.ToString(CultureInfo.InvariantCulture)}, {v4.W.ToString(CultureInfo.InvariantCulture)})";
                }

                return constant.Value?.ToString() ?? "null";

            case BinaryExpression binary:
            {
                var left = ParseExpression(binary.Left, context);
                var right = ParseExpression(binary.Right, context);
                var op = binary.NodeType switch
                {
                    ExpressionType.Add => "+",
                    ExpressionType.Subtract => "-",
                    ExpressionType.Multiply => "*",
                    ExpressionType.Divide => "/",
                    ExpressionType.GreaterThan => ">",
                    ExpressionType.LessThan => "<",
                    ExpressionType.GreaterThanOrEqual => ">=",
                    ExpressionType.LessThanOrEqual => "<=",
                    ExpressionType.Equal => "==",
                    ExpressionType.NotEqual => "!=",
                    _ => throw new NotSupportedException($"Binary operator '{binary.NodeType}' is not supported.")
                };
                return $"({left} {op} {right})";
            }

            case NewExpression newExp:
                if (newExp.Constructor?.DeclaringType == typeof(Vec4))
                {
                    var args = newExp.Arguments.Select(a => ParseExpression(a, context));
                    return $"vec4({string.Join(", ", args)})";
                }
                if (newExp.Constructor?.DeclaringType == typeof(Vec3))
                {
                    var args = newExp.Arguments.Select(a => ParseExpression(a, context));
                    return $"vec3({string.Join(", ", args)})";
                }
                if (newExp.Constructor?.DeclaringType == typeof(Vec2))
                {
                    var args = newExp.Arguments.Select(a => ParseExpression(a, context));
                    return $"vec2({string.Join(", ", args)})";
                }
                break;

            case ParameterExpression parameter:
                return parameter.Name;

            case MemberExpression member:
                // Uniform access: e.g., uniforms.Time
                if (member.Expression is ParameterExpression param && param.Name != null && !context.DeclaredVariables.Contains(param.Name))
                {
                    return member.Member.Name.ToLower();
                }

                // Vector member access or local variable field access
                if (member.Expression != null)
                {
                    var parent = ParseExpression(member.Expression, context);
                    return $"{parent}.{member.Member.Name.ToLower()}";
                }
                break;

            case UnaryExpression unary:
                if (unary.NodeType == ExpressionType.Convert)
                {
                    return ParseExpression(unary.Operand, context);
                }
                if (unary.NodeType == ExpressionType.PostIncrementAssign)
                {
                    var operand = ParseExpression(unary.Operand, context);
                    return $"{operand}++";
                }
                break;

            case MethodCallExpression call:
                if (call.Method.DeclaringType == typeof(Math) || call.Method.DeclaringType == typeof(ShaderMath))
                {
                    var args = call.Arguments.Select(a => ParseExpression(a, context));
                    var functionName = call.Method.Name.ToLower();
                    return $"{functionName}({string.Join(", ", args)})";
                }

                var declaringType = call.Method.DeclaringType;
                if (declaringType == typeof(Vec2) || declaringType == typeof(Vec3) || declaringType == typeof(Vec4))
                {
                    var left = ParseExpression(call.Arguments[0], context);
                    var right = ParseExpression(call.Arguments[1], context);
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

            case ConditionalExpression conditional:
            {
                var test = ParseExpression(conditional.Test, context);
                var ifTrue = ParseExpression(conditional.IfTrue, context);
                var ifFalse = ParseExpression(conditional.IfFalse, context);
                return $"({test}) ? ({ifTrue}) : ({ifFalse})";
            }
        }

        throw new NotSupportedException($"Expression type '{expression.NodeType}' is not supported.");
    }
}
