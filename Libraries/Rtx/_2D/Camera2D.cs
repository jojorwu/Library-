using Rtx.Core;
using System.Numerics;

namespace Rtx._2D
{
    /// <summary>
    /// Represents a virtual camera for rendering 2D scenes.
    /// </summary>
    public class Camera2D
    {
        private readonly Vector2 _origin;
        private readonly Vector2 _lowerLeftCorner;
        private readonly Vector2 _horizontal;

        public Camera2D(Vector2 lookFrom, float viewportWidth)
        {
            _origin = lookFrom;
            _horizontal = new Vector2(viewportWidth, 0);
            _lowerLeftCorner = _origin - _horizontal / 2 - new Vector2(0, 1); // Assuming a fixed vertical direction for simplicity
        }

        /// <summary>
        /// Gets a ray from the camera that passes through the specified viewport coordinate.
        /// </summary>
        /// <param name="u">The horizontal coordinate on the viewport (0 to 1).</param>
        /// <returns>The generated ray.</returns>
        public Ray2D GetRay(float u)
        {
            return new Ray2D(_origin, _lowerLeftCorner + u * _horizontal - _origin);
        }
    }
}
