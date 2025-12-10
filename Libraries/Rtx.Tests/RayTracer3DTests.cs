using NUnit.Framework;
using Rtx._3D;
using Rtx.Core;
using System.Numerics;

namespace Rtx.Tests
{
    public class RayTracer3DTests
    {
        [Test]
        public void Trace_RayHitsSphere_ReturnsNonBackgroundColor()
        {
            // Arrange
            var rayTracer = new RayTracer3D();
            rayTracer.Add(new Sphere(new Vector3(0, 0, 0), 0.5f));
            var ray = new Ray3D(new Vector3(0, 0, -1), new Vector3(0, 0, 1));

            // Act
            var color = rayTracer.Trace(ray);

            // Assert
            Vector3 unitDirection = Vector3.Normalize(ray.Direction);
            float t = 0.5f * (unitDirection.Y + 1.0f);
            Vector3 backgroundColor = (1.0f - t) * Vector3.One + t * new Vector3(0.5f, 0.7f, 1.0f);
            Assert.That(color, Is.Not.EqualTo(backgroundColor));
        }

        [Test]
        public void Trace_RayMissesSphere_ReturnsBackgroundColor()
        {
            // Arrange
            var rayTracer = new RayTracer3D();
            rayTracer.Add(new Sphere(new Vector3(0, 0, 0), 0.5f));
            var ray = new Ray3D(new Vector3(1, 0, -1), new Vector3(0, 0, 1));

            // Act
            var color = rayTracer.Trace(ray);

            // Assert
            Vector3 unitDirection = Vector3.Normalize(ray.Direction);
            float t = 0.5f * (unitDirection.Y + 1.0f);
            Vector3 backgroundColor = (1.0f - t) * Vector3.One + t * new Vector3(0.5f, 0.7f, 1.0f);
            Assert.That(color, Is.EqualTo(backgroundColor));
        }
    }
}
