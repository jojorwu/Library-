using NUnit.Framework;
using SoftShadows;

namespace SoftShadows.Tests
{
    public class SoftShadowGeneratorTests
    {
        [Test]
        public void Generate_WithPcf_CreatesSoftShadows()
        {
            var generator = new SoftShadowGenerator();
            var surfaceDepthFromLight = new float[,]
            {
                { 0.5f, 0.5f, 0.5f },
                { 0.5f, 0.5f, 0.5f },
                { 0.5f, 0.5f, 0.5f }
            };
            var shadowMap = new float[,]
            {
                { 0.2f, 0.2f, 0.2f },
                { 0.2f, 0.8f, 0.2f },
                { 0.2f, 0.2f, 0.2f }
            };

            var softShadowMap = generator.Generate(surfaceDepthFromLight, shadowMap, 3, 0.01f);

            // A surface at depth 0.5 is behind an occluder at 0.2.
            // When sampling around the center pixel (1,1), 8 samples will be occluded and 1 will be lit (through the hole at 0.8).
            // The resulting shadow value should be the percentage of lit samples, which is 1/9.
            Assert.That(softShadowMap[1, 1], Is.EqualTo(1.0f / 9.0f).Within(0.001f));

            // For the corner (0,0), we take 4 samples. 3 are occluded, 1 is lit.
            // The result should be 1/4.
            Assert.That(softShadowMap[0, 0], Is.EqualTo(1.0f / 4.0f).Within(0.001f));

            // For the edge (0,1), we take 6 samples. 5 are occluded, 1 is lit.
            // The result should be 1/6.
            Assert.That(softShadowMap[0, 1], Is.EqualTo(1.0f / 6.0f).Within(0.001f));
        }

        [Test]
        public void Generate_WithFilterSize1_CreatesHardShadows()
        {
            var generator = new SoftShadowGenerator();
            var surfaceDepthFromLight = new float[,]
            {
                { 0.5f, 0.5f },
                { 0.5f, 0.5f }
            };
            var shadowMap = new float[,]
            {
                { 0.2f, 0.8f },
                { 0.8f, 0.2f }
            };

            var softShadowMap = generator.Generate(surfaceDepthFromLight, shadowMap, 1, 0.01f);

            Assert.That(softShadowMap[0, 0], Is.EqualTo(0.0f).Within(0.001f)); // Shadowed
            Assert.That(softShadowMap[0, 1], Is.EqualTo(1.0f).Within(0.001f)); // Lit
            Assert.That(softShadowMap[1, 0], Is.EqualTo(1.0f).Within(0.001f)); // Lit
            Assert.That(softShadowMap[1, 1], Is.EqualTo(0.0f).Within(0.001f)); // Shadowed
        }

        [Test]
        public void Generate_WithNoOccluders_ReturnsFullyLitMap()
        {
            var generator = new SoftShadowGenerator();
            var surfaceDepthFromLight = new float[,]
            {
                { 0.5f, 0.5f },
                { 0.5f, 0.5f }
            };
            var shadowMap = new float[,]
            {
                { 0.8f, 0.8f },
                { 0.8f, 0.8f }
            };

            var softShadowMap = generator.Generate(surfaceDepthFromLight, shadowMap, 3, 0.01f);

            Assert.That(softShadowMap[0, 0], Is.EqualTo(1.0f).Within(0.001f));
            Assert.That(softShadowMap[0, 1], Is.EqualTo(1.0f).Within(0.001f));
            Assert.That(softShadowMap[1, 0], Is.EqualTo(1.0f).Within(0.001f));
            Assert.That(softShadowMap[1, 1], Is.EqualTo(1.0f).Within(0.001f));
        }

        [Test]
        public void Generate_WithFullOcclusion_ReturnsFullyShadowedMap()
        {
            var generator = new SoftShadowGenerator();
            var surfaceDepthFromLight = new float[,]
            {
                { 0.5f, 0.5f },
                { 0.5f, 0.5f }
            };
            var shadowMap = new float[,]
            {
                { 0.2f, 0.2f },
                { 0.2f, 0.2f }
            };

            var softShadowMap = generator.Generate(surfaceDepthFromLight, shadowMap, 3, 0.01f);

            Assert.That(softShadowMap[0, 0], Is.EqualTo(0.0f).Within(0.001f));
            Assert.That(softShadowMap[0, 1], Is.EqualTo(0.0f).Within(0.001f));
            Assert.That(softShadowMap[1, 0], Is.EqualTo(0.0f).Within(0.001f));
            Assert.That(softShadowMap[1, 1], Is.EqualTo(0.0f).Within(0.001f));
        }
    }
}
