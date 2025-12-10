using NUnit.Framework;
using Rtx._2D;
using Rtx.Core;
using System.Numerics;

namespace Rtx.Tests
{
    public class RayTracer2DTests
    {
        [Test]
        public void Trace_RayHitsCircle_ReturnsNonBackgroundColor()
        {
            // Arrange
            var rayTracer = new RayTracer2D();
            rayTracer.Add(new Circle(new Vector2(0, 0), 0.5f));
            var ray = new Ray2D(new Vector2(0, -1), new Vector2(0, 1));

            // Act
            var color = rayTracer.Trace(ray);

            // Assert
            Assert.That(color, Is.Not.EqualTo(new Vector3(0.5f, 0.7f, 1.0f)));
        }

        [Test]
        public void Trace_RayMissesCircle_ReturnsBackgroundColor()
        {
            // Arrange
            var rayTracer = new RayTracer2D();
            rayTracer.Add(new Circle(new Vector2(0, 0), 0.5f));
            var ray = new Ray2D(new Vector2(1, -1), new Vector2(0, 1));

            // Act
            var color = rayTracer.Trace(ray);

            // Assert
            Assert.That(color, Is.EqualTo(new Vector3(0.5f, 0.7f, 1.0f)));
        }
    }
}
