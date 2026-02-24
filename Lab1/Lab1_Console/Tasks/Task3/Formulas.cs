using System;
namespace Lab1
{
    public static class Formulas
    {
        private static long GetFactorial(int num)
        {
            long result = 1;
            for (int i = 2; i <= num; i++)
            {
                result *= i;
            }
            return result;
        }

        public static double Formula_1(int n)
        {
            return 1.0 / Math.Pow(2, n);
        }

        public static double Formula_2(int n)
        {
            return 1.0 / GetFactorial(n + 1);
        }

        public static double Formula_3(int n)
        {
            return -1.0 * Math.Pow(-0.5, n);
        }
    }
}