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
        public int MaxDepth { get; set; } = 50;
        public int SamplesPerPixel { get; set; } = 100;

        /// <summary>
        /// Adds a hittable object to the scene.
        /// </summary>
        public void Add(IHittable3D hittable) => _hittables.Add(hittable);

        /// <summary>
        /// Traces a ray through the scene and determines the color of the pixel.
        /// </summary>
        /// <param name="ray">The ray to trace.</param>
        /// <param name="depth">The current recursion depth.</param>
        /// <returns>The color of the pixel.</returns>
        public Vector3[,] Render(int imageWidth, int imageHeight, Camera camera)
        {
            var image = new Vector3[imageWidth, imageHeight];

            for (int j = imageHeight - 1; j >= 0; --j)
            {
                for (int i = 0; i < imageWidth; ++i)
                {
                    Vector3 pixelColor = Vector3.Zero;
                    for (int s = 0; s < SamplesPerPixel; ++s)
                    {
                        var u = (i + (float)Random.Shared.NextDouble()) / (imageWidth - 1);
                        var v = (j + (float)Random.Shared.NextDouble()) / (imageHeight - 1);
                        var ray = camera.GetRay(u, v);
                        pixelColor += Trace(ray, MaxDepth);
                    }
                    image[i, j] = pixelColor / SamplesPerPixel;
                }
            }

            return image;
        }

        public Vector3 Trace(Ray3D ray, int depth)
        {
            if (depth <= 0)
            {
                return Vector3.Zero;
            }

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
                if (closestHit.Value.Material.Scatter(ray, closestHit.Value, out var attenuation, out var scattered))
                {
                    return attenuation * Trace(scattered, depth - 1);
                }
                return Vector3.Zero;
            }

            // Background color
            Vector3 unitDirection = Vector3.Normalize(ray.Direction);
            float t = 0.5f * (unitDirection.Y + 1.0f);
            return (1.0f - t) * Vector3.One + t * new Vector3(0.5f, 0.7f, 1.0f); // A nice blue sky gradient
        }
    }
}
