using Rtx.Core;
using System.Numerics;

namespace Rtx._3D
{
    /// <summary>
    /// Represents a record of a ray hitting a 3D object.
    /// </summary>
    public readonly record struct HitRecord3D(Vector3 Point, Vector3 Normal, float T);

    /// <summary>
    /// Defines a 3D object that can be intersected by a ray.
    /// </summary>
    public interface IHittable3D
    {
        /// <summary>
        /// Determines if a ray intersects with the object.
        /// </summary>
        /// <param name="ray">The ray to test for intersection.</param>
        /// <param name="tMin">The minimum distance for a valid intersection.</param>
        /// <param name="tMax">The maximum distance for a valid intersection.</param>
        /// <returns>A hit record if an intersection occurs, otherwise null.</returns>
        HitRecord3D? Hit(Ray3D ray, float tMin, float tMax);
    }
}
