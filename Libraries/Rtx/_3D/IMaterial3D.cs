using Rtx.Core;
using System.Numerics;

namespace Rtx._3D
{
    /// <summary>
    /// Defines the material properties of a 3D surface.
    /// </summary>
    public interface IMaterial3D
    {
        /// <summary>
        /// Scatters an incoming ray based on the material's properties.
        /// </summary>
        /// <param name="rayIn">The incoming ray.</param>
        /// <param name="rec">The hit record of the intersection.</param>
        /// <param name="attenuation">The color attenuation of the scattered ray.</param>
        /// <param name="scattered">The resulting scattered ray.</param>
        /// <returns>True if the ray was scattered; false otherwise.</returns>
        bool Scatter(Ray3D rayIn, in HitRecord3D rec, out Vector3 attenuation, out Ray3D scattered);
    }
}
