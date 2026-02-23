using System;
using System.Linq;

namespace Lab1
{
    public class ArrayGenertor
    {
        public static int[] GenerateArray()
        {
            Console.WriteLine("Input array elements separated by spaces:");
            string input = Console.ReadLine();

            try
            {
                return input.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToArray();
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid input! Please enter integers separated by spaces.");
                return new int[0];
            }
        }

        public static int[] GenerateRandom(int size, int minValue = -100, int maxValue = 100)
        {
            Random random = new Random();
            int[] array = new int[size];
            for(int i = 0; i < size; i++)
            {
                array[i] = random.Next(minValue, maxValue + 1);
            }
            return array;
        }
    }
}