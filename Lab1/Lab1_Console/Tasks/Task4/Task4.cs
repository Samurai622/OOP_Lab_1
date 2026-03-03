using System;

namespace Lab1
{
    public class Task4
    {
        public static void Run()
        {
            Console.WriteLine("Task 4. No branching");

            Console.WriteLine("\nFomat: <operation> <number1>");
            Console.WriteLine("Operations: sin cos tan cot exit");

            while(true)
            {
                try
                {
                    Console.Write("Your input:");
                    var line = Console.ReadLine();
                    var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    var op = parts[0];
                    var value = double.Parse(parts[1]);

                    double res = TrigEngine.Execute(op, value);

                    Console.WriteLine("Result: {0}", res);
                }
                catch(Exception)
                {
                    Console.WriteLine("Program has been finished. Have a good day!");
                    break;
                }
            }
        }
    }
}