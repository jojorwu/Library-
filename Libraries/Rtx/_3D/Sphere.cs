using Rtx.Core;
using System.Numerics;

namespace Rtx._3D
{
    /// <summary>
    /// Represents a sphere primitive for 3D ray tracing.
    /// </summary>
    public class Sphere : IHittable3D
    {
        public Vector3 Center { get; }
        public float Radius { get; }

        public Sphere(Vector3 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        public HitRecord3D? Hit(Ray3D ray, float tMin, float tMax)
        {
            Vector3 oc = ray.Origin - Center;
            float a = ray.Direction.LengthSquared();
            float halfB = Vector3.Dot(oc, ray.Direction);
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

            Vector3 point = ray.At(root);
            Vector3 outwardNormal = (point - Center) / Radius;
            return new HitRecord3D(point, outwardNormal, root);
        }
    }
}
