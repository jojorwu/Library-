namespace ShaderGen;

/// <summary>
/// Represents a 4D vector.
/// </summary>
public struct Vec4
{
    public float X, Y, Z, W;

    /// <summary>
    /// Initializes a new instance of the <see cref="Vec4"/> struct.
    /// </summary>
    public Vec4(float x, float y, float z, float w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Vec4"/> struct.
    /// </summary>
    public Vec4(float v)
    {
        X = v;
        Y = v;
        Z = v;
        W = v;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Vec4"/> struct.
    /// </summary>
    public Vec4(Vec2 xy, float z, float w)
    {
        X = xy.X;
        Y = xy.Y;
        Z = z;
        W = w;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Vec4"/> struct.
    /// </summary>
    public Vec4(Vec2 xy, Vec2 zw)
    {
        X = xy.X;
        Y = xy.Y;
        Z = zw.X;
        W = zw.Y;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Vec4"/> struct.
    /// </summary>
    public Vec4(Vec3 xyz, float w)
    {
        X = xyz.X;
        Y = xyz.Y;
        Z = xyz.Z;
        W = w;
    }

    public static Vec4 operator +(Vec4 a, Vec4 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
    public static Vec4 operator -(Vec4 a, Vec4 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
    public static Vec4 operator *(Vec4 a, Vec4 b) => new(a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W);
    public static Vec4 operator /(Vec4 a, Vec4 b) => new(a.X / b.X, a.Y / b.Y, a.Z / b.Z, a.W / b.W);

    public static Vec4 operator /(Vec4 a, float b) => new(a.X / b, a.Y / b, a.Z / b, a.W / b);

    /// <summary>
    /// Gets a swizzled <see cref="Vec2"/> with the components (X, Y).
    /// </summary>
    public Vec2 Xy => new(X, Y);
    /// <summary>
    /// Gets a swizzled <see cref="Vec3"/> with the components (X, Y, Z).
    /// </summary>
    public Vec3 Xyz => new(X, Y, Z);
    /// <summary>
    /// Gets a swizzled <see cref="Vec3"/> with the components (R, G, B).
    /// </summary>
    public Vec3 Rgb => new(X, Y, Z);
}
