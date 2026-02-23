using System;

namespace Lab1
{
    public static class Menu
    {
        public static void Start()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Lab1 - Delegates");
                Console.WriteLine("1. Task 1");
                Console.WriteLine("2. Task 2");
                Console.WriteLine("3. Task 3");
                Console.WriteLine("4. Task 4");
                Console.WriteLine("5. Task 5");
                Console.WriteLine("6. Task 6");
                Console.WriteLine("0. Вихід");

                Console.Write("\nВаш вибір: ");
                string input = Console.ReadLine();

                Console.Clear();

                switch (input)
                {
                    case "1":
                        // Tasks.Task1.Run();
                        break;

                    case "2":
                        // Tasks.Task2.Run();
                        break;

                    case "3":
                        // Tasks.Task3.Run();
                        break;

                    case "4":
                        // Tasks.Task4.Run();
                        break;

                    case "5":
                        // Tasks.Task5.Run();
                        break;

                    case "6":
                        // Tasks.Task6.Run();
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("Невірний вибір!");
                        break;
                }

                Console.WriteLine("\nНатисніть Enter...");
                Console.ReadLine();
            }
        }
    }
}