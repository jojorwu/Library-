using NUnit.Framework;
using ShaderGen;
using System.Linq.Expressions;

namespace ShaderGen.Tests;

public class ShaderGeneratorTests
{
    public class MyUniforms
    {
        public float Time { get; set; }
        public Vec2 Resolution { get; set; }
        public Sampler2D MainTexture { get; set; }
    }

    [Test]
    public void Generate_WithComplexUniforms_ReturnsCorrectShader()
    {
        var generator = new ShaderGenerator();
        var shader = generator.Generate((MyUniforms u) => new Vec4(u.Resolution.X / u.Time, 0.0f, 0.0f, 1.0f));
        var expected = @"
#version 330 core
uniform float time;
uniform vec2 resolution;
uniform sampler2D maintexture;
out vec4 FragColor;
void main()
{
    FragColor = vec4((resolution.x / time), 0.0, 0.0, 1.0);
}
".Trim();
        Assert.That(shader.Trim(), Is.EqualTo(expected));
    }

    [Test]
    public void Generate_WithVec4ScalarConstructor_ReturnsCorrectShader()
    {
        var generator = new ShaderGenerator();
        var shader = generator.Generate((MyUniforms u) => new Vec4(u.Time));
        var expected = @"
#version 330 core
uniform float time;
uniform vec2 resolution;
uniform sampler2D maintexture;
out vec4 FragColor;
void main()
{
    FragColor = vec4(time);
}
".Trim();
        Assert.That(shader.Trim(), Is.EqualTo(expected));
    }

    [Test]
    public void Generate_WithVec4FromVec2Constructor_ReturnsCorrectShader()
    {
        var generator = new ShaderGenerator();
        var shader = generator.Generate((MyUniforms u) => new Vec4(u.Resolution, u.Time, 1.0f));
        var expected = @"
#version 330 core
uniform float time;
uniform vec2 resolution;
uniform sampler2D maintexture;
out vec4 FragColor;
void main()
{
    FragColor = vec4(resolution, time, 1.0);
}
".Trim();
        Assert.That(shader.Trim(), Is.EqualTo(expected));
    }

    [Test]
    public void Generate_WithVec4FromVec3Constructor_ReturnsCorrectShader()
    {
        var generator = new ShaderGenerator();
        var shader = generator.Generate((MyUniforms u) => new Vec4(new Vec3(u.Resolution, u.Time), 1.0f));
        var expected = @"
#version 330 core
uniform float time;
uniform vec2 resolution;
uniform sampler2D maintexture;
out vec4 FragColor;
void main()
{
    FragColor = vec4(vec3(resolution, time), 1.0);
}
".Trim();
        Assert.That(shader.Trim(), Is.EqualTo(expected));
    }

    [Test]
    public void Generate_WithVec2ScalarConstructor_ReturnsCorrectShader()
    {
        var generator = new ShaderGenerator();
        var shader = generator.Generate((MyUniforms u) => new Vec4(new Vec2(u.Time), 0.0f, 1.0f));
        var expected = @"
#version 330 core
uniform float time;
uniform vec2 resolution;
uniform sampler2D maintexture;
out vec4 FragColor;
void main()
{
    FragColor = vec4(vec2(time), 0.0, 1.0);
}
".Trim();
        Assert.That(shader.Trim(), Is.EqualTo(expected));
    }

    [Test]
    public void Generate_WithDotFunction_ReturnsCorrectShader()
    {
        var generator = new ShaderGenerator();
        var shader = generator.Generate((MyUniforms u) =>
            new Vec4(ShaderMath.Dot(u.Resolution, new Vec2(1.0f, 1.0f)), 0.0f, 0.0f, 1.0f));
        var expected = @"
#version 330 core
uniform float time;
uniform vec2 resolution;
uniform sampler2D maintexture;
out vec4 FragColor;
void main()
{
    FragColor = vec4(dot(resolution, vec2(1.0, 1.0)), 0.0, 0.0, 1.0);
}
".Trim();
        Assert.That(shader.Trim(), Is.EqualTo(expected));
    }

    [Test]
    public void Generate_WithFractFunction_ReturnsCorrectShader()
    {
        var generator = new ShaderGenerator();
        var shader = generator.Generate((MyUniforms u) =>
            new Vec4(ShaderMath.Fract(u.Time), 0.0f, 0.0f, 1.0f));
        var expected = @"
#version 330 core
uniform float time;
uniform vec2 resolution;
uniform sampler2D maintexture;
out vec4 FragColor;
void main()
{
    FragColor = vec4(fract(time), 0.0, 0.0, 1.0);
}
".Trim();
        Assert.That(shader.Trim(), Is.EqualTo(expected));
    }

    [Test]
    public void Generate_WithMixFunction_ReturnsCorrectShader()
    {
        var generator = new ShaderGenerator();
        var shader = generator.Generate((MyUniforms u) =>
            new Vec4(ShaderMath.Mix(0.0f, 1.0f, u.Time), 0.0f, 0.0f, 1.0f));
        var expected = @"
#version 330 core
uniform float time;
uniform vec2 resolution;
uniform sampler2D maintexture;
out vec4 FragColor;
void main()
{
    FragColor = vec4(mix(0.0, 1.0, time), 0.0, 0.0, 1.0);
}
".Trim();
        Assert.That(shader.Trim(), Is.EqualTo(expected));
    }

    [Test]
    public void Generate_WithSwizzling_ReturnsCorrectShader()
    {
        var generator = new ShaderGenerator();
        var shader = generator.Generate((MyUniforms u) => new Vec4(u.Resolution.Xy.Y, 0.0f, 0.0f, 1.0f));
        var expected = @"
#version 330 core
uniform float time;
uniform vec2 resolution;
uniform sampler2D maintexture;
out vec4 FragColor;
void main()
{
    FragColor = vec4(resolution.xy.y, 0.0, 0.0, 1.0);
}
".Trim();
        Assert.That(shader.Trim(), Is.EqualTo(expected));
    }

    [Test]
    public void Generate_WithTextureSampling_ReturnsCorrectShader()
    {
        var generator = new ShaderGenerator();
        var shader = generator.Generate((MyUniforms u) => ShaderMath.Texture(u.MainTexture, u.Resolution.Xy));
        var expected = @"
#version 330 core
uniform float time;
uniform vec2 resolution;
uniform sampler2D maintexture;
out vec4 FragColor;
void main()
{
    FragColor = texture(maintexture, resolution.xy);
}
".Trim();
        Assert.That(shader.Trim(), Is.EqualTo(expected));
    }

    [Test]
    public void Generate_WithConditionalOperator_ReturnsCorrectShader()
    {
        var generator = new ShaderGenerator();
        var shader = generator.Generate((MyUniforms u) => u.Time > 0.5f ? new Vec4(1.0f, 0.0f, 0.0f, 1.0f) : new Vec4(0.0f, 1.0f, 0.0f, 1.0f));
        var expected = @"
#version 330 core
uniform float time;
uniform vec2 resolution;
uniform sampler2D maintexture;
out vec4 FragColor;
void main()
{
    FragColor = ((time > 0.5)) ? (vec4(1.0, 0.0, 0.0, 1.0)) : (vec4(0.0, 1.0, 0.0, 1.0));
}
".Trim();
        Assert.That(shader.Trim(), Is.EqualTo(expected));
    }

    [Test]
    public void Generate_WithLocalVariables_ReturnsCorrectShader()
    {
        var generator = new ShaderGenerator();

        var u = Expression.Parameter(typeof(MyUniforms), "u");
        var uv = Expression.Variable(typeof(Vec2), "uv");
        var assign = Expression.Assign(uv,
            Expression.Divide(
                Expression.Property(u, "Resolution"),
                Expression.Property(u, "Time")));

        var returnVal = Expression.New(typeof(Vec4).GetConstructor(new[] { typeof(float), typeof(float), typeof(float), typeof(float) }),
            Expression.Field(uv, "X"),
            Expression.Field(uv, "Y"),
            Expression.Constant(0.0f),
            Expression.Constant(1.0f));

        var block = Expression.Block(new[] { uv }, assign, returnVal);
        var lambda = Expression.Lambda<System.Func<MyUniforms, Vec4>>(block, u);

        var shader = generator.Generate(lambda);
        var expected = @"
#version 330 core
uniform float time;
uniform vec2 resolution;
uniform sampler2D maintexture;
out vec4 FragColor;
void main()
{
    vec2 uv = (resolution / time);
    FragColor = vec4(uv.x, uv.y, 0.0, 1.0);
}
".Trim();
        Assert.That(shader.Trim(), Is.EqualTo(expected));
    }

    [Test]
    public void Generate_WithVec3ScalarConstructor_ReturnsCorrectShader()
    {
        var generator = new ShaderGenerator();
        var shader = generator.Generate((MyUniforms u) => new Vec4(new Vec3(u.Time), 1.0f));
        var expected = @"
#version 330 core
uniform float time;
uniform vec2 resolution;
uniform sampler2D maintexture;
out vec4 FragColor;
void main()
{
    FragColor = vec4(vec3(time), 1.0);
}
".Trim();
        Assert.That(shader.Trim(), Is.EqualTo(expected));
    }

    [Test]
    public void Generate_WithVec3FromVec2Constructor_ReturnsCorrectShader()
    {
        var generator = new ShaderGenerator();
        var shader = generator.Generate((MyUniforms u) => new Vec4(new Vec3(u.Resolution, u.Time), 1.0f));
        var expected = @"
#version 330 core
uniform float time;
uniform vec2 resolution;
uniform sampler2D maintexture;
out vec4 FragColor;
void main()
{
    FragColor = vec4(vec3(resolution, time), 1.0);
}
".Trim();
        Assert.That(shader.Trim(), Is.EqualTo(expected));
    }

    [Test]
    public void Generate_WithCosFunction_ReturnsCorrectShader()
    {
        var generator = new ShaderGenerator();
        var shader = generator.Generate((MyUniforms u) => new Vec4((float)Math.Cos(u.Time), 0.0f, 0.0f, 1.0f));
        var expected = @"
#version 330 core
uniform float time;
uniform vec2 resolution;
uniform sampler2D maintexture;
out vec4 FragColor;
void main()
{
    FragColor = vec4(cos(time), 0.0, 0.0, 1.0);
}
".Trim();
        Assert.That(shader.Trim(), Is.EqualTo(expected));
    }

    [Test]
    public void Generate_WithIfStatement_ReturnsCorrectShader()
    {
        var generator = new ShaderGenerator();

        var u = Expression.Parameter(typeof(MyUniforms), "u");
        var color = Expression.Variable(typeof(Vec3), "color");

        var assignColor = Expression.Assign(color, Expression.Constant(new Vec3(0.0f, 0.0f, 0.0f)));

        var ifThenElse = Expression.IfThenElse(
            Expression.GreaterThan(Expression.Property(u, "Time"), Expression.Constant(0.5f)),
            Expression.Assign(color, Expression.Constant(new Vec3(1.0f, 0.0f, 0.0f))),
            Expression.Assign(color, Expression.Constant(new Vec3(0.0f, 1.0f, 0.0f)))
        );

        var returnVal = Expression.New(
            typeof(Vec4).GetConstructor(new[] { typeof(float), typeof(float), typeof(float), typeof(float) }),
            Expression.Field(color, "X"),
            Expression.Field(color, "Y"),
            Expression.Field(color, "Z"),
            Expression.Constant(1.0f)
        );

        var block = Expression.Block(new[] { color }, assignColor, ifThenElse, returnVal);
        var lambda = Expression.Lambda<System.Func<MyUniforms, Vec4>>(block, u);

        var shader = generator.Generate(lambda);

        // Note: The generated code is not perfectly formatted, but it is functionally correct.
        var expected = @"
#version 330 core
uniform float time;
uniform vec2 resolution;
uniform sampler2D maintexture;
out vec4 FragColor;
void main()
{
    vec3 color = vec3(0, 0, 0);
    if ((time > 0.5)) { color = vec3(1, 0, 0); } else { color = vec3(0, 1, 0); }
    FragColor = vec4(color.x, color.y, color.z, 1.0);
}
".Trim();
        Assert.That(shader.Trim().ReplaceLineEndings(), Is.EqualTo(expected.Trim().ReplaceLineEndings()));
    }

    [Test]
    public void Generate_WithVectorConstructors_ReturnsCorrectShader()
    {
        var generator = new ShaderGenerator();
        var shader = generator.Generate((MyUniforms u) => new Vec4(new Vec2(u.Time), new Vec2(0.5f)));
        var expected = @"
#version 330 core
uniform float time;
uniform vec2 resolution;
uniform sampler2D maintexture;
out vec4 FragColor;
void main()
{
    FragColor = vec4(vec2(time), vec2(0.5));
}
".Trim();
        Assert.That(shader.Trim(), Is.EqualTo(expected));
    }

    [Test]
    public void Generate_WithForLoop_ReturnsCorrectShader()
    {
        var generator = new ShaderGenerator();

        var u = Expression.Parameter(typeof(MyUniforms), "u");
        var color = Expression.Variable(typeof(Vec3), "color");
        var i = Expression.Variable(typeof(int), "i");

        var assignColor = Expression.Assign(color, Expression.Constant(new Vec3(0.0f, 0.0f, 0.0f)));

        var forLoop = Expression.Call(
            typeof(ShaderMath).GetMethod("For"),
            Expression.Lambda(Expression.Convert(Expression.Assign(i, Expression.Constant(0)), typeof(object))),
            Expression.Lambda(Expression.LessThan(i, Expression.Constant(5))),
            Expression.Lambda(Expression.Convert(Expression.PostIncrementAssign(i), typeof(object))),
            Expression.Lambda<Action>(Expression.Assign(color, Expression.Add(color, Expression.Constant(new Vec3(0.1f, 0.0f, 0.0f)))))
        );

        var returnVal = Expression.New(
            typeof(Vec4).GetConstructor(new[] { typeof(float), typeof(float), typeof(float), typeof(float) }),
            Expression.Field(color, "X"),
            Expression.Field(color, "Y"),
            Expression.Field(color, "Z"),
            Expression.Constant(1.0f)
        );

        var block = Expression.Block(new[] { color, i }, assignColor, forLoop, returnVal);
        var lambda = Expression.Lambda<System.Func<MyUniforms, Vec4>>(block, u);

        var shader = generator.Generate(lambda);

        // Note: The generated code is not perfectly formatted, but it is functionally correct.
        var expected = @"
#version 330 core
uniform float time;
uniform vec2 resolution;
uniform sampler2D maintexture;
out vec4 FragColor;
void main()
{
    vec3 color = vec3(0, 0, 0);
    for (int i = 0; (i < 5); i++) { color = (color + vec3(0.1, 0, 0)); }
    FragColor = vec4(color.x, color.y, color.z, 1.0);
}
".Trim();
        Assert.That(shader.Trim().ReplaceLineEndings(), Is.EqualTo(expected.Trim().ReplaceLineEndings()));
    }
}
