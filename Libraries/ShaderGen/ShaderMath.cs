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
}
