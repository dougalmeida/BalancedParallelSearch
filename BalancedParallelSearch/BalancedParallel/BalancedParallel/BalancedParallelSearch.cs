using System;
using System.Threading;

namespace BalancedParallel
{
    public class BalancedParallelSearch
    {
        private int procNum;
        private long[] affinityMask;
        private bool[] affinityPool;

        private Semaphore sem;
        private int stopCountoOnlyForTests = 0;
        private int counter;
        private System.Diagnostics.Stopwatch watch;

        public int ProcNum { get => procNum; }
        public int Counter { get => counter; set => counter = value; }
        public BalancedParallelSearch()
        {
            procNum = Environment.ProcessorCount;

            affinityMask = new long[procNum];
            affinityPool = new bool[procNum];

            sem = new Semaphore(0, procNum);

            Counter = 0;

            for (int i = 0; i < procNum; i++)
            {
                affinityMask[i] |= 1L << i;
                affinityPool[i] = true;
            }
        }

        public void startSearch()
        {
            sem.Release(ProcNum);
        }

        private int DefineAffinity(IntPtr CurrentThread)
        {
            int aPool = -1;

            lock (affinityPool)
            {
                while (aPool == -1)
                {
                    for (int i = 0; i < ProcNum; i++)
                    {
                        if (affinityPool[i])
                        {
                            aPool = i;
                            break;
                        }
                    }
                }

                affinityPool[aPool] = false;

                Native32.SetThreadAffinityMask(CurrentThread, (UIntPtr)affinityMask[aPool]);
            }

            return aPool;
        }

        public void ThreadSemaphore_ProcNum(object semaphoreArtifacts)
        {
            object[] artifacts = (object[])semaphoreArtifacts;

            string targetThread = (string)artifacts[0];

            SemaphoreManager manager = (SemaphoreManager)artifacts[1];

            sem.WaitOne();

            Thread.BeginThreadAffinity();

            Console.WriteLine($"STARTED Processor: {Native32.GetCurrentProcessorNumber()} threadID: {Native32.GetCurrentThreadId()} Name: {Thread.CurrentThread.Name}");

            IntPtr CurrentThread = Native32.GetCurrentThread();

            int aPool = DefineAffinity(CurrentThread);

            if (targetThread != null)
            {
                Console.WriteLine("Doing stuff for :" + targetThread);

                Thread.Sleep(new Random().Next(1, 10000));
            }

            lock (affinityPool) affinityPool[aPool] = true;

            Console.WriteLine($"FINISHED Processor: {Native32.GetCurrentProcessorNumber()} threadID: {Native32.GetCurrentThreadId()} Name: {Thread.CurrentThread.Name}");

            Thread.EndThreadAffinity();

            sem.Release();

            this.counter--;

            lock (manager)
            {
                Monitor.Pulse(manager);
            }
        }
    }
}