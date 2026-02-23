using System;
using System.Diagnostics;

namespace Lab1
{
    class Task2
    {
        public static void Run()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            int[] originalArray = null;

            while (true)
            {
                Console.WriteLine("=== STAGE 1: ARRAY GENERATION ===");
                Console.WriteLine("1 - Enter manually");
                Console.WriteLine("2 - Generate randomly");
                Console.WriteLine("0 - Exit");
                Console.Write("Your choice: ");
                string genChoice = Console.ReadLine();

                switch (genChoice)
                {
                    case "1":
                        originalArray = ArrayGenertor.GenerateArray();
                        break;

                    case "2":
                        int size = 0;
                        while (true)
                        {
                            Console.Write("Enter size of array: ");
                            if (int.TryParse(Console.ReadLine(), out size) && size > 0)
                                break;
                            Console.WriteLine("Invalid input. Please enter a positive integer.");
                        }
                        originalArray = ArrayGenertor.GenerateRandom(size);
                        break;
                        
                    case "0":
                        Console.WriteLine("Exiting...");
                        return;

                    default:
                        Console.WriteLine("Invalid choice!");
                        break;
                }
                Console.WriteLine($"\nInitial array: [ {string.Join(", ", originalArray)} ]");

                int k;
                while(true)
                {
                    Console.Write("\nEnter k (number of elements to move): ");
                    if(int.TryParse(Console.ReadLine(), out k) && k != 0)
                        break;
                    Console.WriteLine("Invalid input. Please enter a non-zero integer.");
                }
                FilterDelegate isMultipleOfK = (number) => number % k == 0;

                bool isRunning = true;
                while (isRunning)
                {
                    Console.WriteLine("\n=== STAGE 2: MENU STARTING STAGE ===");
                    Console.WriteLine("1 - Method A: using Enumerable.Where");
                    Console.WriteLine("2 - Method B: using custom loop");
                    Console.WriteLine("3 - Using both methods and comparing results");
                    Console.WriteLine("0 - Exit");
                    Console.Write("Your choice: ");

                    string actionChoice = Console.ReadLine();
                    Console.WriteLine();

                    switch (actionChoice)
                    {
                        case "1":
                            int[] resWhere = ArrayProcessor.FilterWithWhere(originalArray, isMultipleOfK);
                            Console.WriteLine($"Result using method A: [ {string.Join(", ", resWhere)} ]");
                            break;
                        case "2":
                            int[] resCustom = ArrayProcessor.FilterCustom(originalArray, isMultipleOfK);
                            Console.WriteLine($"Result using method B: [ {string.Join(", ", resCustom)} ]");
                            break;
                        case "3":
                            int[] arrayA = ArrayProcessor.FilterWithWhere(originalArray, isMultipleOfK);
                            int[] arrayB = ArrayProcessor.FilterCustom(originalArray, isMultipleOfK);
                            Console.WriteLine($"Result using method A: [ {string.Join(", ", arrayA)} ]");
                            Console.WriteLine($"Result using method B: [ {string.Join(", ", arrayB)} ]");
                            
                            bool areEqual = arrayA.SequenceEqual(arrayB);
                            Console.WriteLine($"Are results equal? {(areEqual ? "Yes" : "No")}");
                            break;
                        case "0":
                            Console.WriteLine("Exiting...");
                            isRunning = false;
                            break;
                        default:
                            Console.WriteLine("Invalid choice!");
                            break;
                    }
                }
            }
        }
    }
}