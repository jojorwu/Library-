using Rtx.Core;
using System.Collections.Generic;
using System.Numerics;

namespace Rtx._3D
{
    /// <summary>
    /// A simple ray tracer for 3D scenes.
    /// </summary>
    public class RayTracer3D
    {
        private readonly List<IHittable3D> _hittables = new();

        /// <summary>
        /// Adds a hittable object to the scene.
        /// </summary>
        public void Add(IHittable3D hittable) => _hittables.Add(hittable);

        /// <summary>
        /// Traces a ray through the scene and determines the color of the pixel.
        /// </summary>
        /// <param name="ray">The ray to trace.</param>
        /// <returns>The color of the pixel.</returns>
        public Vector3 Trace(Ray3D ray)
        {
            HitRecord3D? closestHit = null;
            float closestT = float.MaxValue;

            foreach (var hittable in _hittables)
            {
                var hit = hittable.Hit(ray, 0.001f, closestT);
                if (hit.HasValue && hit.Value.T < closestT)
                {
                    closestT = hit.Value.T;
                    closestHit = hit;
                }
            }

            if (closestHit.HasValue)
            {
                // Simple visualization of the normal
                return (closestHit.Value.Normal + Vector3.One) * 0.5f;
            }

            // Background color
            Vector3 unitDirection = Vector3.Normalize(ray.Direction);
            float t = 0.5f * (unitDirection.Y + 1.0f);
            return (1.0f - t) * Vector3.One + t * new Vector3(0.5f, 0.7f, 1.0f); // A nice blue sky gradient
        }
    }
}
