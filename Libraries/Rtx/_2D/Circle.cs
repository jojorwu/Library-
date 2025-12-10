using Rtx.Core;
using System.Numerics;

namespace Rtx._2D
{
    /// <summary>
    /// Represents a circle primitive for 2D ray tracing.
    /// </summary>
    public class Circle : IHittable2D
    {
        public Vector2 Center { get; }
        public float Radius { get; }
        public IMaterial2D Material { get; }

        public Circle(Vector2 center, float radius, IMaterial2D material)
        {
            Center = center;
            Radius = radius;
            Material = material;
        }

        public HitRecord2D? Hit(Ray2D ray, float tMin, float tMax)
        {
            Vector2 oc = ray.Origin - Center;
            float a = Vector2.Dot(ray.Direction, ray.Direction);
            float halfB = Vector2.Dot(oc, ray.Direction);
            float c = oc.LengthSquared() - Radius * Radius;
            float discriminant = halfB * halfB - a * c;

            if (discriminant < 0)
            {
                return null;
            }

            float sqrtD = MathF.Sqrt(discriminant);

            // Find the nearest root that lies in the acceptable range
            float root = (-halfB - sqrtD) / a;
            if (root < tMin || root > tMax)
            {
                root = (-halfB + sqrtD) / a;
                if (root < tMin || root > tMax)
                {
                    return null;
                }
            }

            Vector2 point = ray.At(root);
            Vector2 normal = Vector2.Normalize(point - Center);
            return new HitRecord2D(point, normal, Material, root);
        }
    }
}
