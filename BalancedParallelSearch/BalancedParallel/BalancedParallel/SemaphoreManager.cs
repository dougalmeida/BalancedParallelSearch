using System;
using System.Collections.Generic;
using System.Threading;

namespace BalancedParallel
{
    public class SemaphoreManager
    {
        public void DoManagement(List<string> threads)
        {
            BalancedParallelSearch balancedParallel = new BalancedParallelSearch();

            balancedParallel.startSearch();

            while (true)
            {
                foreach (var thread in threads)
                {
                    if (balancedParallel.Counter == balancedParallel.ProcNum)
                    {
                        lock (this)
                        {
                            Monitor.Wait(this);
                            GC.Collect();
                        }
                    }

                    balancedParallel.Counter++;

                    var threadsAcompanhamentoProcessual = new Thread(balancedParallel.ThreadSemaphore_ProcNum);
                    threadsAcompanhamentoProcessual.Name = "thread_" + thread;
                    threadsAcompanhamentoProcessual.Priority = ThreadPriority.Normal;
                    object[] semaphoreArtifacts = new object[] { thread, this };
                    threadsAcompanhamentoProcessual.Start(semaphoreArtifacts);
                }

                while (true)
                {
                    if (balancedParallel.Counter == 0)
                    {
                        Monitor.Enter(balancedParallel);
                        Console.WriteLine("Slept");
                        Thread.Sleep(20000);
                        Console.WriteLine("Woke up");
                        Monitor.Exit(balancedParallel);
                        break;
                    }
                }
            }
        }
    }
}