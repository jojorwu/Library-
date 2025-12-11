namespace ShaderGen;

public struct Vec2
{
    public float X, Y;

    public Vec2(float x, float y)
    {
        X = x;
        Y = y;
    }

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

    public Vec2 Xy => new(X, Y);
    public Vec2 Yx => new(Y, X);
}
