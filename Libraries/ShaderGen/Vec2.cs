namespace ShaderGen;

/// <summary>
/// Represents a 2D vector.
/// </summary>
public struct Vec2
{
    public float X, Y;

    /// <summary>
    /// Initializes a new instance of the <see cref="Vec2"/> struct.
    /// </summary>
    public Vec2(float x, float y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Vec2"/> struct.
    /// </summary>
    public Vec2(float v)
    {
        X = v;
        Y = v;
    }

    public static Vec2 operator +(Vec2 a, Vec2 b) => new(a.X + b.X, a.Y + b.Y);
    public static Vec2 operator -(Vec2 a, Vec2 b) => new(a.X - b.X, a.Y - b.Y);
    public static Vec2 operator *(Vec2 a, Vec2 b) => new(a.X * b.X, a.Y * b.Y);
    public static Vec2 operator /(Vec2 a, Vec2 b) => new(a.X / b.X, a.Y / b.Y);

    public static Vec2 operator /(Vec2 a, float b) => new(a.X / b, a.Y / b);

    /// <summary>
    /// Gets a swizzled <see cref="Vec2"/> with the components (X, Y).
    /// </summary>
    public Vec2 Xy => new(X, Y);
    /// <summary>
    /// Gets a swizzled <see cref="Vec2"/> with the components (Y, X).
    /// </summary>
    public Vec2 Yx => new(Y, X);
}
