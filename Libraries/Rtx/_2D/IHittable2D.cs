using Rtx.Core;
using System.Numerics;

namespace Rtx._2D
{
    /// <summary>
    /// Represents a record of a ray hitting a 2D object.
    /// </summary>
    public readonly record struct HitRecord2D(Vector2 Point, Vector2 Normal, IMaterial2D Material, float T);

    /// <summary>
    /// Defines a 2D object that can be intersected by a ray.
    /// </summary>
    public interface IHittable2D
    {
        /// <summary>
        /// Determines if a ray intersects with the object.
        /// </summary>
        /// <param name="ray">The ray to test for intersection.</param>
        /// <param name="tMin">The minimum distance for a valid intersection.</param>
        /// <param name="tMax">The maximum distance for a valid intersection.</param>
        /// <returns>A hit record if an intersection occurs, otherwise null.</returns>
        HitRecord2D? Hit(Ray2D ray, float tMin, float tMax);
    }
}
