using NUnit.Framework;
using Rtx._3D;
using System.Numerics;

namespace Rtx.Tests
{
    public class RayTracer3DTests
    {
        [Test]
        public void Render_SimpleScene_ReturnsNonBlackImage()
        {
            // Arrange
            var rayTracer = new RayTracer3D { SamplesPerPixel = 1, MaxDepth = 1 };
            var material = new Lambertian(new Vector3(0.5f, 0.5f, 0.5f));
            rayTracer.Add(new Sphere(new Vector3(0, 0, -1), 0.5f, material));
            var camera = new Camera(new Vector3(0, 0, 0), new Vector3(0, 0, -1), new Vector3(0, 1, 0), 90, 1.0f);

            // Act
            var image = rayTracer.Render(10, 10, camera);

            // Assert
            bool isBlack = true;
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (image[i, j] != Vector3.Zero)
                    {
                        isBlack = false;
                        break;
                    }
                }
                if (!isBlack) break;
            }
            Assert.That(isBlack, Is.False);
        }
    }
}
