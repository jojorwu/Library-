using NUnit.Framework;
using SoftShadows;

namespace SoftShadows.Tests
{
    public class SoftShadowGeneratorTests
    {
        [Test]
        public void Generate_DoesNotThrowException()
        {
            var generator = new SoftShadowGenerator();
            Assert.DoesNotThrow(() => generator.Generate(100, 100));
        }
    }
}
