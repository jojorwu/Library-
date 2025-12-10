using NUnit.Framework;
using Rtx._2D;
using System.Numerics;

namespace Rtx.Tests
{
    public class RayTracer2DTests
    {
        [Test]
        public void Render_SimpleScene_ReturnsNonBlackImage()
        {
            // Arrange
            var rayTracer = new RayTracer2D { SamplesPerPixel = 1, MaxDepth = 1 };
            var material = new Lambertian(new Vector3(0.5f, 0.5f, 0.5f));
            rayTracer.Add(new Circle(new Vector2(0, 0), 0.5f, material));
            var camera = new Camera2D(new Vector2(0, -1), 2.0f);

            // Act
            var image = rayTracer.Render(10, camera);

            // Assert
            bool isBlack = true;
            for (int i = 0; i < 10; i++)
            {
                if (image[i] != Vector3.Zero)
                {
                    isBlack = false;
                    break;
                }
            }
            Assert.That(isBlack, Is.False);
        }
    }
}
