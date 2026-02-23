using System;

namespace Lab1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            int[] orginalArray = null;

            while (true)
            {
                Console.WriteLine("=== STAGE 1: ARRAY GENERATION ===");
                Console.WriteLine("1 - Enter manually");
                Console.WriteLine("2 - Generate randomly");
                Console.Write("Your choice: ");
                string genChoice = Console.ReadLine();

                switch (genChoice)
                {
                    case "1":
                        orginalArray = ArrayGenertor.GenerateArray();
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
                        orginalArray = ArrayGenertor.GenerateRandom(size);
                        break;
                        
                    case "0":
                        Console.WriteLine("Exiting...");
                        return;

                    default:
                        Console.WriteLine("Invalid choice!");
                        break;
                }
                Console.WriteLine($"\nПочатковий масив: [ {string.Join(", ", originalArray)} ]");
            }
        }
    }
}