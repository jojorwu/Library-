using NUnit.Framework;
using Rtx;

namespace Rtx.Tests
{
    public class RtxGeneratorTests
    {
        [Test]
        public void Generate_DoesNotThrowException()
        {
            var generator = new RtxGenerator();
            Assert.DoesNotThrow(() => generator.Generate());
        }
    }
}
