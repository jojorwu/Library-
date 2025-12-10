using Rtx.Core;
using System.Numerics;

namespace Rtx._3D
{
    public class Metal : IMaterial3D
    {
        public Vector3 Albedo { get; }
        public float Fuzz { get; }

        public Metal(Vector3 albedo, float fuzz)
        {
            Albedo = albedo;
            Fuzz = fuzz < 1 ? fuzz : 1;
        }

        public bool Scatter(Ray3D rayIn, in HitRecord3D rec, out Vector3 attenuation, out Ray3D scattered)
        {
            Vector3 reflected = Vector3.Reflect(Vector3.Normalize(rayIn.Direction), rec.Normal);
            scattered = new Ray3D(rec.Point, reflected + Fuzz * RandomInUnitSphere());
            attenuation = Albedo;
            return Vector3.Dot(scattered.Direction, rec.Normal) > 0;
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
