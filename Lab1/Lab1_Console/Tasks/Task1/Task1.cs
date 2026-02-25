using System;

namespace Lab1
{
    public static class Task1
    {
        public static void Run()
        {
            Console.WriteLine("=== Task 1. Timers ===\n");

            int timersCount = 0;

            while (true)
            {
                Console.WriteLine("Enter number of timers to create (from 1 tp 6): ");
                if (int.TryParse(Console.ReadLine(), out timersCount) && timersCount >= 1 && timersCount <= 6)
                    break;
                Console.WriteLine("Invalid input. Please enter a number between 1 and 6.");
            }

            Timer[] timers = new Timer[timersCount];

            Console.WriteLine();

            for (int i = 0; i < timersCount; i++)
            {
                int interval = 0;
                while (true)
                {
                    Console.WriteLine("Enter interval for timer {0} (in seconds): ", i + 1);
                    if (int.TryParse(Console.ReadLine(), out interval) && interval > 0)
                        break;
                    Console.WriteLine("Invalid input. Please enter a positive integer.");
                }
                int timerNumber = i + 1;
                TimerAction action = () => 
                Console.WriteLine("Timer {0} triggered at {1}", timerNumber, DateTime.Now.ToString("HH:mm:ss"));
                timers[i] = new Timer(action, interval);
            }

            Console.WriteLine("\nStarting timers...");

            foreach (var timer in timers)
                timer.Start();

            Console.WriteLine("--- Timers are working ---");
            Console.WriteLine("Press ENTER, to stop them and exit.\n");
            Console.ReadLine();

            foreach (var timer in timers)
                timer.Stop();

            Console.WriteLine("Work completed. All timers stopped.");
        }
    }
}