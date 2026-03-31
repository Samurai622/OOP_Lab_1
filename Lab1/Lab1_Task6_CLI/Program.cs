using System;
using Lab1_Task6; // <-- доступ до TestingService і ReferenceData

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        if (args.Length < 4 || args[1] != "--cli")
        {
            Console.WriteLine("CLI MODE ERROR: expected: -- --cli <lang> <algo>");
            return;
        }

        string lang = args[2];
        string algo = args[3].Replace("_", " ");

        // отримання коду
        string code = Console.In.ReadToEnd();

        // запуск перевірки
        TestingService.RunCheck(
            code,
            lang,
            algo,
            (msg, success, critical) =>
            {
                Console.WriteLine(msg);
            }
        );
    }
}