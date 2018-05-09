namespace WarLightAi.Math
{
    public static class BinomialDistribution
    {
        // http://keisan.casio.com/exec/system/1180573199
        /// <returns>the probability that the specified number of successes will be the actual result</returns>
        public static decimal ProbabilityMass(int successes, int trials, double successProbability)
        {
            int failures = trials - successes;
            double failureProbability = (1 - successProbability);
            long combinations = trials.Choose(successes);

            decimal exactSuccessesProbability = (decimal)System.Math.Pow(successProbability, successes);
            decimal exactFailuresProbability = (decimal)System.Math.Pow(failureProbability, failures);

            decimal probabilityMass = combinations * exactSuccessesProbability * exactFailuresProbability;
            return probabilityMass;
        }

        ///// <returns>Calculates the number of successes which the true result has (at least) probability probabilityThreshold to be less than or equal to</returns>
        //public static int SuccessesHighEstimate(int trials, double successProbability, double probabilityThreshold)
        //{
        //    double probability = 0;
        //    int i = -1;

        //    while (i < trials && probability <= probabilityThreshold)
        //    {
        //        i++;
        //        probability += (double)ProbabilityMass(i, trials, successProbability);
        //    }
        //    return i;
        //}

        /// <summary>
        /// Checks whether the number of trials is so large we can't calculate it exactly
        /// </summary>
        private static bool IsTrialSizeTooBig(int trials)
        {
            return (trials > 65);
        }

        private static double GetZScore(double confidence)
        {
            double zeta;
            if (confidence <= 0.80001)
                zeta = 1.281;
            else if (confidence <= 0.90001)
                zeta = 1.645;
            else if (confidence <= 0.950001)
                zeta = 1.96;
            else if (confidence <= 0.990001)
                zeta = 2.575;
            else
                zeta = 3;
            return zeta;
        }

        // http://www.sigmazone.com/binomial_confidence_interval.htm
        public static int SuccessesHighEstimateNormal(int trials, double successProbability, double probabilityThreshold)
        {
            double zeta = GetZScore(probabilityThreshold);
            double normalEstimate = successProbability + zeta * System.Math.Sqrt(successProbability * (1 - successProbability) / trials);
            return (int)(trials * normalEstimate);
        }

        // http://www.sigmazone.com/binomial_confidence_interval.htm
        // http://www.regentsprep.org/Regents/math/algtrig/ATS7/ZChart.htm
        public static int SuccessesLowEstimateNormal(int trials, double successProbability, double probabilityThreshold)
        {
            double zeta = GetZScore(probabilityThreshold);
            double normalEstimate = successProbability - zeta * System.Math.Sqrt(successProbability * (1 - successProbability) / trials);
            return (int)(trials * normalEstimate);
        }

        /// <summary>This tries to calculate it more efficiently by coming from the direction most likely to hit the threshold first</summary>
        /// <returns>Calculates the number of successes which the true result has (at least) probability probabilityThreshold to be less than or equal to</returns>
        public static int SuccessesHighEstimate(int trials, double successProbability, double probabilityThreshold)
        {
            if (IsTrialSizeTooBig(trials))
                return SuccessesHighEstimateNormal(trials, successProbability, probabilityThreshold);

            decimal cumulativeProbability = 1.0m;
            int successes = trials + 1;

            while (successes > 0 && cumulativeProbability > (decimal)probabilityThreshold)
            {
                successes--;
                cumulativeProbability -= ProbabilityMass(successes, trials, successProbability);
            }

            return successes;
        }

        

        /// <summary>This tries to calculate it more efficiently by coming from the direction most likely to hit the threshold first</summary>
        /// <returns>Calculates the number of successes which the true result has (at least) probability probabilityThreshold to be greater than or equal to</returns>
        public static int SuccessesLowEstimate(int trials, double successProbability, double probabilityThreshold)
        {
            if (IsTrialSizeTooBig(trials))
                return SuccessesLowEstimateNormal(trials, successProbability, probabilityThreshold);

            decimal cumulativeProbability = 1.0m;
            int successes = -1;

            while (successes <= trials && cumulativeProbability > (decimal)probabilityThreshold)
            {
                successes++;
                cumulativeProbability -= ProbabilityMass(successes, trials, successProbability);
            }
            return successes;
        }

        ///// <returns>Calculates the number of successes which the true result has (at least) probability probabilityThreshold to be greater than or equal to</returns>
        //public static int SuccessesLowEstimate(int trials, double successProbability, double probabilityThreshold)
        //{
        //    double probability = 0;
        //    int i = trials + 1;

        //    while (i > 0 && probability <= probabilityThreshold)
        //    {
        //        i--;
        //        probability += ProbabilityMass(i, trials, successProbability);
        //    }
        //    return i;
        //}
    }
}
