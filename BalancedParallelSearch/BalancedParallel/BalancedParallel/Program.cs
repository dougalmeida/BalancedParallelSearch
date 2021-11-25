using System;
using System.Collections.Generic;

namespace BalancedParallel
{
    class Program
    {
        static void Main(string[] args)
        {
            int totalThread = 0;

            while (true)
            {
                Console.WriteLine("Threads to be processed:");

                string input = Console.ReadLine();

                if (int.TryParse(input, out totalThread))
                    break;
            }

            List<string> threads = new List<string>();

            for (int i = 1; i <= totalThread; i++)
                threads.Add($"Thread#{i}");

            SemaphoreManager manager = new SemaphoreManager();

            manager.DoManagement(threads);
        }
    }
}