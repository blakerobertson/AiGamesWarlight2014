namespace WarLightAi.Math
{
    public static class MathExtensions
    {
        // http://stackoverflow.com/questions/12983731/algorithm-for-calculating-binomial-coefficient
        public static long Choose(this long n, long k)
        {
            double sum = 0;
            for (long i = 0; i < k; i++)
            {
                sum += System.Math.Log10(n - i);
                sum -= System.Math.Log10(i + 1);
            }
            return (long)System.Math.Round(System.Math.Pow(10, sum));
        }

        public static long Choose(this int n, long k)
        {
            double sum = 0;
            for (long i = 0; i < k; i++)
            {
                sum += System.Math.Log10(n - i);
                sum -= System.Math.Log10(i + 1);
            }
            return (long)System.Math.Round(System.Math.Pow(10, sum));
        }
    }
}
