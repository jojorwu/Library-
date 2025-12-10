using Rtx.Core;
using System.Collections.Generic;
using System.Numerics;

namespace Rtx._2D
{
    /// <summary>
    /// A simple ray tracer for 2D scenes.
    /// </summary>
    public class RayTracer2D
    {
        private readonly List<IHittable2D> _hittables = new();
        public int MaxDepth { get; set; } = 50;
        public int SamplesPerPixel { get; set; } = 100;

        /// <summary>
        /// Adds a hittable object to the scene.
        /// </summary>
        public void Add(IHittable2D hittable) => _hittables.Add(hittable);

        /// <summary>
        /// Renders a 1D image of the 2D scene.
        /// </summary>
        public Vector3[] Render(int imageWidth, Camera2D camera)
        {
            var image = new Vector3[imageWidth];
            for (int i = 0; i < imageWidth; ++i)
            {
                Vector3 pixelColor = Vector3.Zero;
                for (int s = 0; s < SamplesPerPixel; ++s)
                {
                    var u = (i + (float)Random.Shared.NextDouble()) / (imageWidth - 1);
                    var ray = camera.GetRay(u);
                    pixelColor += Trace(ray, MaxDepth);
                }
                image[i] = pixelColor / SamplesPerPixel;
            }
            return image;
        }

        /// <summary>
        /// Traces a ray through the scene and determines the color of the pixel.
        /// </summary>
        /// <param name="ray">The ray to trace.</param>
        /// <param name="depth">The current recursion depth.</param>
        /// <returns>The color of the pixel.</returns>
        public Vector3 Trace(Ray2D ray, int depth)
        {
            if (depth <= 0)
            {
                return Vector3.Zero;
            }

            HitRecord2D? closestHit = null;
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
            return new Vector3(0.5f, 0.7f, 1.0f); // A nice blue sky
        }
    }
}
