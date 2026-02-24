using System;

namespace Lab1
{
    public class Task3
    {
        public static void Run()
        {
            double precision = -1;

            while (true)
            {
                Console.WriteLine("Choose method to calculate sum:");
                Console.WriteLine("1 - Using regular delegate");
                Console.WriteLine("2 - Using lFunc");
                Console.WriteLine("3 - Enter precision");
                Console.WriteLine("0 - Exit");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        if (precision <= 0)
                        {
                            Console.WriteLine("Please enter a presicion.");
                            break;
                        }
                        Method_A(precision);
                        break;
                    case "2":
                        if (precision <= 0)
                        {
                            Console.WriteLine("Please enter a presicion.");
                            break;
                        }
                        Method_B(precision);
                        break;
                    case "3":
                        while (true)
                        {
                            Console.Write("Enter precision (positive number): ");
                            if (double.TryParse(Console.ReadLine(), out double inputPrecision) && inputPrecision > 0)
                            {                            
                                precision = inputPrecision;
                                Console.WriteLine($"Precision set to {precision}");
                                break;
                            }
                            else                        
                            {
                                Console.WriteLine("Invalid input. Please enter a positive number.");
                            }
                        }
                        break;
                    case "0":
                        Console.WriteLine("Exiting...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
        public static void Method_A(double precision)
        {
            Console.WriteLine("Method A is working...");

            TermCalculator f1 = new TermCalculator(Formulas.Formula_1);
            TermCalculator f2 = new TermCalculator(Formulas.Formula_2);
            TermCalculator f3 = new TermCalculator(Formulas.Formula_3);

            double sum1 = JustDelegate.CalculateSum(precision, f1);
            double sum2 = JustDelegate.CalculateSum(precision, f2);
            double sum3 = JustDelegate.CalculateSum(precision, f3);

            PrintResults(sum1, sum2, sum3);
        }
        public static void Method_B(double precision)
        {
            Console.WriteLine("Method B is working...");
            Func<int, double> f1 = Formulas.Formula_1;
            Func<int, double> f2 = Formulas.Formula_2;
            Func<int, double> f3 = Formulas.Formula_3;

            double sum1 = WithFunc.Compute(precision, f1);
            double sum2 = WithFunc.Compute(precision, f2);
            double sum3 = WithFunc.Compute(precision, f3);

            PrintResults(sum1, sum2, sum3);   
        }
        public static void PrintResults(double sum1, double sum2, double sum3)
        {
            Console.WriteLine($"Sum using Formula 1: {sum1}");
            Console.WriteLine($"Sum using Formula 2: {sum2}");
            Console.WriteLine($"Sum using Formula 3: {sum3}");
            Console.WriteLine();
        }
    }
}