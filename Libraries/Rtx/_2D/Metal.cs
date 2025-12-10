using Rtx.Core;
using System.Numerics;

namespace Rtx._2D
{
    public class Metal : IMaterial2D
    {
        public Vector3 Albedo { get; }
        public float Fuzz { get; }

        public Metal(Vector3 albedo, float fuzz)
        {
            Albedo = albedo;
            Fuzz = fuzz < 1 ? fuzz : 1;
        }

        public bool Scatter(Ray2D rayIn, in HitRecord2D rec, out Vector3 attenuation, out Ray2D scattered)
        {
            Vector2 reflected = Vector2.Reflect(Vector2.Normalize(rayIn.Direction), rec.Normal);
            scattered = new Ray2D(rec.Point, reflected + Fuzz * RandomInUnitCircle());
            attenuation = Albedo;
            return Vector2.Dot(scattered.Direction, rec.Normal) > 0;
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
