using System;
namespace Lab1
{
    public static class MathHelper
    {
        public static double FactorialInverse(int n)
        {
            double fact = 1;
            for(int i = 1; i <= n; i++) fact *= i;
            return 1.0 / fact;
        }

        public static double Formula_1(int n)
        {
            return 1.0 / Math.Pow(2, n);
        }

        public static double Formula_2(int n)
        {
            return FactorialInverse(n);
        }

        public static double Formula_3(int n)
        {
            return 1.0 / Math.Pow(-0.5, n);
        }
    }
}