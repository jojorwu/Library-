using NUnit.Framework;
using ShaderGen;

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
}
