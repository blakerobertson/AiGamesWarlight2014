using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WarLightAi.Math;

namespace WarLightAiTests
{
    [TestClass]
    public class MathExtensionsTests
    {
        [TestMethod]
        public void Choose_10Choose1_Returns10()
        {
            var result = 10.Choose(1);

            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void Choose_10Choose2_Returns45()
        {
            var result = 10.Choose(2);

            Assert.AreEqual(45, result);
        }

        [TestMethod]
        public void Choose_100Choose5_Returns75287520()
        {
            var result = 100.Choose(5);

            Assert.AreEqual(75287520, result);
        }
    }
}
