using Rtx.Core;
using System.Numerics;

namespace Rtx._2D
{
    public class Lambertian : IMaterial2D
    {
        public Vector3 Albedo { get; }

        public Lambertian(Vector3 albedo)
        {
            Albedo = albedo;
        }

        public bool Scatter(Ray2D rayIn, in HitRecord2D rec, out Vector3 attenuation, out Ray2D scattered)
        {
            var scatterDirection = rec.Normal + RandomInUnitCircle();

            // Catch degenerate scatter direction
            if (scatterDirection.LengthSquared() < 1e-8)
                scatterDirection = rec.Normal;

            scattered = new Ray2D(rec.Point, scatterDirection);
            attenuation = Albedo;
            return true;
        }

        private static Vector2 RandomInUnitCircle()
        {
            while (true)
            {
                var p = new Vector2(
                    (float)(Random.Shared.NextDouble() * 2 - 1),
                    (float)(Random.Shared.NextDouble() * 2 - 1)
                );
                if (p.LengthSquared() >= 1) continue;
                return p;
            }
        }
    }
}
