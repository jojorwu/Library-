namespace ShaderGen;

/// <summary>
/// Represents a 3D vector.
/// </summary>
public struct Vec3
{
    public float X, Y, Z;

    /// <summary>
    /// Initializes a new instance of the <see cref="Vec3"/> struct.
    /// </summary>
    public Vec3(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Vec3"/> struct.
    /// </summary>
    public Vec3(float v)
    {
        X = v;
        Y = v;
        Z = v;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Vec3"/> struct.
    /// </summary>
    public Vec3(Vec2 xy, float z)
    {
        X = xy.X;
        Y = xy.Y;
        Z = z;
    }

    public static Vec3 operator +(Vec3 a, Vec3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public static Vec3 operator -(Vec3 a, Vec3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    public static Vec3 operator *(Vec3 a, Vec3 b) => new(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
    public static Vec3 operator /(Vec3 a, Vec3 b) => new(a.X / b.X, a.Y / b.Y, a.Z / b.Z);

    public static Vec3 operator /(Vec3 a, float b) => new(a.X / b, a.Y / b, a.Z / b);

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
