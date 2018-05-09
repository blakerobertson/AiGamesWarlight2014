using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WarLightAi.Math;

namespace WarLightAiTests
{
    [TestClass]
    public class BinomialDistributionTests
    {
        [TestMethod]
        public void ProbabilityMass_Successes5Trials10Probability60Percent_Returns20Percent()
        {
            var result = BinomialDistribution.ProbabilityMass(5, 10, 0.6);

            Assert.IsTrue(0.2006581m < result && result < 0.2006582m, string.Format("Actual value is <{0}>", result));
        }

        [TestMethod]
        public void SuccessesHighEstimate_Trials10Probability60PercentThreshold80Percent_Returns7()
        {
            var result = BinomialDistribution.SuccessesHighEstimate(10, 0.6, 0.8);

            Assert.AreEqual(7, result);
        }

        [TestMethod]
        public void SuccessesHighEstimate_Trials50Probability60PercentThreshold80Percent_Returns33()
        {
            var result = BinomialDistribution.SuccessesHighEstimate(50, 0.6, 0.8);

            Assert.AreEqual(33, result);
        }

        [TestMethod]
        public void SuccessesHighEstimateNormal_Trials1000Probability60PercentThreshold90Percent_Returns33()
        {
            var result = BinomialDistribution.SuccessesHighEstimateNormal(1000, 0.6, 0.9);

            Assert.AreEqual(625, result); // exact number would be 620
        }

        [TestMethod]
        public void SuccessesLowEstimateNormal_Trials1000Probability60PercentThreshold90Percent_Returns33()
        {
            var result = BinomialDistribution.SuccessesLowEstimateNormal(1000, 0.6, 0.9);

            Assert.AreEqual(574, result); // exact number would be 580
        }

        [TestMethod]
        public void SuccessesLowEstimate_Trials10Probability60PercentThreshold80Percent_Returns5()
        {
            var result = BinomialDistribution.SuccessesLowEstimate(10, 0.6, 0.8);

            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void SuccessesLowEstimate_Trials50Probability60PercentThreshold80Percent_Returns27()
        {
            var result = BinomialDistribution.SuccessesLowEstimate(50, 0.6, 0.8);

            Assert.AreEqual(27, result);
        }
    }
}
