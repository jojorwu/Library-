using NUnit.Framework;
using Rtx._2D;
using Rtx.Core;
using System.Numerics;

namespace Rtx.Tests
{
    public class RayTracer2DTests
    {
        [Test]
        public void Trace_RayHitsLambertianCircle_ReturnsAttenuatedColor()
        {
            // Arrange
            var rayTracer = new RayTracer2D();
            var material = new Lambertian(new Vector3(0.5f, 0.5f, 0.5f));
            rayTracer.Add(new Circle(new Vector2(0, 0), 0.5f, material));
            var ray = new Ray2D(new Vector2(0, -1), new Vector2(0, 1));

            // Act
            var color = rayTracer.Trace(ray, rayTracer.MaxDepth);

            // Assert
            Assert.That(color, Is.Not.EqualTo(Vector3.Zero));
            Assert.That(color, Is.Not.EqualTo(new Vector3(0.5f, 0.7f, 1.0f)));
        }

        [Test]
        public void Trace_RayHitsMetalCircle_ReturnsReflectedColor()
        {
            // Arrange
            var rayTracer = new RayTracer2D();
            var material = new Metal(new Vector3(0.8f, 0.6f, 0.2f), 0.0f);
            rayTracer.Add(new Circle(new Vector2(0, 0), 0.5f, material));
            rayTracer.Add(new Circle(new Vector2(0, -100.5f), 100f, new Lambertian(new Vector3(0.8f, 0.8f, 0.0f))));
            var ray = new Ray2D(new Vector2(0, -1), new Vector2(0, 1));

            // Act
            var color = rayTracer.Trace(ray, rayTracer.MaxDepth);

            // Assert
            Assert.That(color, Is.Not.EqualTo(Vector3.Zero));
            Assert.That(color, Is.Not.EqualTo(new Vector3(0.5f, 0.7f, 1.0f)));
        }
    }
}
