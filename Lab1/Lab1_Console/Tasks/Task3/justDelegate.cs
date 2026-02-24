using System;

namespace Lab1
{
    public delegate double TermCalculator(int n);
    public class JustDelegate
    {
        public static double CalculateSum(double precision, TermCalculator termFunc)
        {
            double sum = 0;
            int i = 0;
            double currentTerm = termFunc(i);

            while (Math.Abs(currentTerm) > precision)
            {
                sum += currentTerm;
                i++;
                currentTerm = termFunc(i);
            }
            return sum;
        }
    }
}