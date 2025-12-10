using NUnit.Framework;
using Rtx._3D;
using Rtx.Core;
using System.Numerics;

namespace Rtx.Tests
{
    public class RayTracer3DTests
    {
        [Test]
        public void Trace_RayHitsLambertianSphere_ReturnsAttenuatedColor()
        {
            // Arrange
            var rayTracer = new RayTracer3D();
            var material = new Lambertian(new Vector3(0.5f, 0.5f, 0.5f));
            rayTracer.Add(new Sphere(new Vector3(0, 0, 0), 0.5f, material));
            var ray = new Ray3D(new Vector3(0, 0, -1), new Vector3(0, 0, 1));

            // Act
            var color = rayTracer.Trace(ray, rayTracer.MaxDepth);

            // Assert
            Assert.That(color, Is.Not.EqualTo(Vector3.Zero));
            Assert.That(color, Is.Not.EqualTo(new Vector3(0.5f, 0.7f, 1.0f)));
        }

        [Test]
        public void Trace_RayHitsMetalSphere_ReturnsReflectedColor()
        {
            // Arrange
            var rayTracer = new RayTracer3D();
            var material = new Metal(new Vector3(0.8f, 0.6f, 0.2f), 0.0f);
            rayTracer.Add(new Sphere(new Vector3(0, 0, 0), 0.5f, material));
            rayTracer.Add(new Sphere(new Vector3(0, -100.5f, -1), 100f, new Lambertian(new Vector3(0.8f, 0.8f, 0.0f))));
            var ray = new Ray3D(new Vector3(0, 0, -1), new Vector3(0, 0, 1));

            // Act
            var color = rayTracer.Trace(ray, rayTracer.MaxDepth);

            // Assert
            Assert.That(color, Is.Not.EqualTo(Vector3.Zero));
            Assert.That(color, Is.Not.EqualTo(new Vector3(0.5f, 0.7f, 1.0f)));
        }
    }
}
