using Rtx.Core;
using System.Numerics;

namespace Rtx._3D
{
    /// <summary>
    /// Represents a virtual camera for rendering 3D scenes.
    /// </summary>
    public class Camera
    {
        private readonly Vector3 _origin;
        private readonly Vector3 _lowerLeftCorner;
        private readonly Vector3 _horizontal;
        private readonly Vector3 _vertical;

        public Camera(Vector3 lookFrom, Vector3 lookAt, Vector3 vup, float vfov, float aspectRatio)
        {
            var theta = vfov * (MathF.PI / 180f);
            var h = MathF.Tan(theta / 2);
            var viewportHeight = 2.0f * h;
            var viewportWidth = aspectRatio * viewportHeight;

            var w = Vector3.Normalize(lookFrom - lookAt);
            var u = Vector3.Normalize(Vector3.Cross(vup, w));
            var v = Vector3.Cross(w, u);

            _origin = lookFrom;
            _horizontal = viewportWidth * u;
            _vertical = viewportHeight * v;
            _lowerLeftCorner = _origin - _horizontal / 2 - _vertical / 2 - w;
        }

        /// <summary>
        /// Gets a ray from the camera that passes through the specified viewport coordinates.
        /// </summary>
        /// <param name="s">The horizontal coordinate on the viewport (0 to 1).</param>
        /// <param name="t">The vertical coordinate on the viewport (0 to 1).</param>
        /// <returns>The generated ray.</returns>
        public Ray3D GetRay(float s, float t)
        {
            return new Ray3D(_origin, _lowerLeftCorner + s * _horizontal + t * _vertical - _origin);
        }
    }
}
