using Rtx.Core;
using System.Numerics;

namespace Rtx._3D
{
    public class Lambertian : IMaterial3D
    {
        public Vector3 Albedo { get; }

        public Lambertian(Vector3 albedo)
        {
            Albedo = albedo;
        }

        public bool Scatter(Ray3D rayIn, in HitRecord3D rec, out Vector3 attenuation, out Ray3D scattered)
        {
            var scatterDirection = rec.Normal + RandomInUnitSphere();

            // Catch degenerate scatter direction
            if (scatterDirection.LengthSquared() < 1e-8)
                scatterDirection = rec.Normal;

            scattered = new Ray3D(rec.Point, scatterDirection);
            attenuation = Albedo;
            return true;
        }

        private static Vector3 RandomInUnitSphere()
        {
            while (true)
            {
                var p = new Vector3(
                    (float)(Random.Shared.NextDouble() * 2 - 1),
                    (float)(Random.Shared.NextDouble() * 2 - 1),
                    (float)(Random.Shared.NextDouble() * 2 - 1)
                );
                if (p.LengthSquared() >= 1) continue;
                return p;
            }
        }
    }
}
