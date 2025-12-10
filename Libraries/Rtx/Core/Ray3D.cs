using System.Numerics;

namespace Rtx.Core
{
    /// <summary>
    /// Represents a 3D ray with an origin and a direction.
    /// </summary>
    public readonly struct Ray3D
    {
        public Vector3 Origin { get; }
        public Vector3 Direction { get; }

        public Ray3D(Vector3 origin, Vector3 direction)
        {
            Origin = origin;
            Direction = Vector3.Normalize(direction);
        }

        /// <summary>
        /// Gets a point along the ray at a specified distance.
        /// </summary>
        public Vector3 At(float t) => Origin + Direction * t;
    }
}
