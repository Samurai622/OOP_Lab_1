using System;
namespace Lab1
{
    public class WithFunc
    {
        public static double Compute(double precision, Func<int, double> getTerm)
        {
            double sum = 0;
            int n = 0;
            double currentTerm;

            do
            {
                currentTerm = getTerm(n);
                if(Math.Abs(currentTerm) >= precision)
                {
                    sum += currentTerm;
                }
                n++;
            }
            while (Math.Abs(currentTerm) >= precision);

            return sum;
        }
    }
}