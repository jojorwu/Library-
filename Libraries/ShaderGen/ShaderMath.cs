using System;

namespace ShaderGen;

/// <summary>
/// Provides C# equivalents for GLSL's built-in functions.
/// The ShaderGenerator will translate calls to these methods to their native GLSL counterparts.
/// </summary>
public static class ShaderMath
{
    public static float Dot(Vec2 a, Vec2 b) => throw new NotImplementedException("This method is for shader generation only.");
    public static float Dot(Vec3 a, Vec3 b) => throw new NotImplementedException("This method is for shader generation only.");
    public static float Dot(Vec4 a, Vec4 b) => throw new NotImplementedException("This method is for shader generation only.");

    public static float Fract(float a) => throw new NotImplementedException("This method is for shader generation only.");
    public static Vec2 Fract(Vec2 a) => throw new NotImplementedException("This method is for shader generation only.");
    public static Vec3 Fract(Vec3 a) => throw new NotImplementedException("This method is for shader generation only.");
    public static Vec4 Fract(Vec4 a) => throw new NotImplementedException("This method is for shader generation only.");

    public static float Mix(float a, float b, float t) => throw new NotImplementedException("This method is for shader generation only.");
    public static Vec2 Mix(Vec2 a, Vec2 b, float t) => throw new NotImplementedException("This method is for shader generation only.");
    public static Vec3 Mix(Vec3 a, Vec3 b, float t) => throw new NotImplementedException("This method is for shader generation only.");
    public static Vec4 Mix(Vec4 a, Vec4 b, float t) => throw new NotImplementedException("This method is for shader generation only.");

    public static Vec2 Normalize(Vec2 a) => throw new NotImplementedException("This method is for shader generation only.");
    public static Vec3 Normalize(Vec3 a) => throw new NotImplementedException("This method is for shader generation only.");
    public static Vec4 Normalize(Vec4 a) => throw new NotImplementedException("This method is for shader generation only.");

    public static float Length(Vec2 a) => throw new NotImplementedException("This method is for shader generation only.");
    public static float Length(Vec3 a) => throw new NotImplementedException("This method is for shader generation only.");
    public static float Length(Vec4 a) => throw new NotImplementedException("This method is for shader generation only.");

    public static float Step(float edge, float x) => throw new NotImplementedException("This method is for shader generation only.");
    public static Vec2 Step(Vec2 edge, Vec2 x) => throw new NotImplementedException("This method is for shader generation only.");
    public static Vec3 Step(Vec3 edge, Vec3 x) => throw new NotImplementedException("This method is for shader generation only.");
    public static Vec4 Step(Vec4 edge, Vec4 x) => throw new NotImplementedException("This method is for shader generation only.");

    public static float Smoothstep(float edge0, float edge1, float x) => throw new NotImplementedException("This method is for shader generation only.");
    public static Vec2 Smoothstep(Vec2 edge0, Vec2 edge1, Vec2 x) => throw new NotImplementedException("This method is for shader generation only.");
    public static Vec3 Smoothstep(Vec3 edge0, Vec3 edge1, Vec3 x) => throw new NotImplementedException("This method is for shader generation only.");
    public static Vec4 Smoothstep(Vec4 edge0, Vec4 edge1, Vec4 x) => throw new NotImplementedException("This method is for shader generation only.");

    public static float Clamp(float x, float minVal, float maxVal) => throw new NotImplementedException("This method is for shader generation only.");
    public static Vec2 Clamp(Vec2 x, Vec2 minVal, Vec2 maxVal) => throw new NotImplementedException("This method is for shader generation only.");
    public static Vec3 Clamp(Vec3 x, Vec3 minVal, Vec3 maxVal) => throw new NotImplementedException("This method is for shader generation only.");
    public static Vec4 Clamp(Vec4 x, Vec4 minVal, Vec4 maxVal) => throw new NotImplementedException("This method is for shader generation only.");

    public static Vec4 Texture(Sampler2D sampler, Vec2 uv) => throw new NotImplementedException("This method is for shader generation only.");
}
