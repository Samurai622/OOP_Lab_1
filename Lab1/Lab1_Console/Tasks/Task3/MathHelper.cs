using System;
namespace Lab1
{
    public class MathHelper
    {
        public static double FactorialInverse(int n)
        {
            double fact = 1;
            for(int i = 1; i <= n; i++) fact *= i;
            return 1.0 / fact;
        }
    }
}