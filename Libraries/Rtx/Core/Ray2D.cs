using System.Numerics;

namespace Rtx.Core
{
    /// <summary>
    /// Represents a 2D ray with an origin and a direction.
    /// </summary>
    public readonly struct Ray2D
    {
        public Vector2 Origin { get; }
        public Vector2 Direction { get; }

        public Ray2D(Vector2 origin, Vector2 direction)
        {
            Origin = origin;
            Direction = Vector2.Normalize(direction);
        }

        /// <summary>
        /// Gets a point along the ray at a specified distance.
        /// </summary>
        public Vector2 At(float t) => Origin + Direction * t;
    }
}
