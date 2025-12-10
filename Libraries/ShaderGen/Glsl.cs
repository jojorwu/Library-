using System;

namespace ShaderGen;

public static class Glsl
{
    public static string FromType(Type type)
    {
        if (type == typeof(float)) return "float";
        if (type == typeof(Vec2)) return "vec2";
        if (type == typeof(Vec3)) return "vec3";
        if (type == typeof(Vec4)) return "vec4";
        throw new NotSupportedException($"The type '{type.Name}' is not supported as a uniform type.");
    }
}
