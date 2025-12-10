using NUnit.Framework;
using ShaderGen;
using System.Linq.Expressions;

namespace ShaderGen.Tests;

public class ShaderGeneratorTests
{
    [Test]
    public void Generate_WithConstantExpression_ReturnsCorrectShader()
    {
        var generator = new ShaderGenerator();
        var shader = generator.Generate(() => 0.5f);
        var expected = @"
#version 330 core
out vec4 FragColor;

void main()
{
    float result = 0.5;
    FragColor = vec4(result, result, result, 1.0);
}
";
        Assert.That(shader.Trim(), Is.EqualTo(expected.Trim()));
    }

    [Test]
    public void Generate_WithAddition_ReturnsCorrectShader()
    {
        var generator = new ShaderGenerator();
        var body = Expression.Add(Expression.Constant(0.2f), Expression.Constant(0.3f));
        var lambda = Expression.Lambda<System.Func<float>>(body);
        var shader = generator.Generate(lambda);
        var expected = @"
#version 330 core
out vec4 FragColor;

void main()
{
    float result = (0.2 + 0.3);
    FragColor = vec4(result, result, result, 1.0);
}
";
        Assert.That(shader.Trim(), Is.EqualTo(expected.Trim()));
    }

    [Test]
    public void Generate_WithComplexExpression_ReturnsCorrectShader()
    {
        var generator = new ShaderGenerator();
        var add = Expression.Add(Expression.Constant(0.2f), Expression.Constant(0.3f));
        var multiply = Expression.Multiply(add, Expression.Constant(2.0f));
        var lambda = Expression.Lambda<System.Func<float>>(multiply);
        var shader = generator.Generate(lambda);
        var expected = @"
#version 330 core
out vec4 FragColor;

void main()
{
    float result = ((0.2 + 0.3) * 2.0);
    FragColor = vec4(result, result, result, 1.0);
}
";
        Assert.That(shader.Trim(), Is.EqualTo(expected.Trim()));
    }

    [Test]
    public void Generate_WithVec4Expression_ReturnsCorrectShader()
    {
        var generator = new ShaderGenerator();
        var shader = generator.Generate(() => new Vec4(0.1f, 0.2f, 0.3f, 1.0f));
        var expected = @"
#version 330 core
out vec4 FragColor;

void main()
{
    FragColor = vec4(0.1, 0.2, 0.3, 1.0);
}
";
        Assert.That(shader.Trim(), Is.EqualTo(expected.Trim()));
    }

    [Test]
    public void Generate_WithUniformExpression_ReturnsCorrectShader()
    {
        var generator = new ShaderGenerator();
        var shader = generator.Generate((Vec2 uv) => new Vec4(uv.X, uv.Y, 0.0f, 1.0f));
        var expected = @"
#version 330 core
uniform vec2 uv;
out vec4 FragColor;

void main()
{
    FragColor = vec4(uv.x, uv.y, 0.0, 1.0);
}
";
        Assert.That(shader.Trim(), Is.EqualTo(expected.Trim()));
    }

    [Test]
    public void Generate_WithSinFunction_ReturnsCorrectShader()
    {
        var generator = new ShaderGenerator();
        var shader = generator.Generate((Vec2 uv) => new Vec4((float)Math.Sin(uv.X), 0.0f, 0.0f, 1.0f));
        var expected = @"
#version 330 core
uniform vec2 uv;
out vec4 FragColor;

void main()
{
    FragColor = vec4(sin(uv.x), 0.0, 0.0, 1.0);
}
";
        Assert.That(shader.Trim(), Is.EqualTo(expected.Trim()));
    }
}
