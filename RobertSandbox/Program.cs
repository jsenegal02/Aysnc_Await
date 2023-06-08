using System.Collections.Concurrent;

namespace RobertSandbox
{


    public class Program
    {
        public static int GetThreadId()
        {
            return Thread.CurrentThread.ManagedThreadId;
        }

        /// <summary>
        /// Showing serial vs parallel
        /// </summary>
        public static async Task FirstExample()
        {

            object lockObj = new object();
            List<int> aList = new();
            //ConcurrentBag<int> aList = new();


            Task<string> t = Task.Run(() =>
            {
                Console.WriteLine("Start Task: " + GetThreadId());

                for (int j = 0; j < 500; j++)
                {
                    Console.WriteLine("Task 1 waiting for lockObj");
                    lock (lockObj)
                    {
                        Console.WriteLine("Task 1 Got lockObj");
                        //Thread.Sleep(5000);
                        aList.Add(j);
                    }
                    Console.WriteLine("Task 1 released lockObj");
                }

                Console.WriteLine("End Task");

                return "Task 1 result";
            });

            Task<string> t2 = Task.Run(() =>
            {
                Console.WriteLine("Start Task");

                for (int j = 0; j < 500; j++)
                {
                    Console.WriteLine("Task 2 waiting for lockObj");
                    lock (lockObj)
                    {
                        Console.WriteLine("Task 2 Got lockObj");
                        //Thread.Sleep(2500);
                        aList.Add(j);
                    }
                    Console.WriteLine("Task 2 released lockObj");
                }

                Console.WriteLine("End Task");

                return "Task 2 result";
            });

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("Main Working... " + i);
                await Task.Delay(1000);
            }

            t.Wait();
            t2.Wait();

            Console.WriteLine("Tasks finished: " + t.Result + " : " + t2.Result);
        }

        #region Async/Await
        public static void MixIngredients()
        {
            Console.WriteLine("MixIngredients: " + GetThreadId());
            Console.Write("Mixing ingrdients");
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(1000);
                Console.Write(".");
            }
            Console.WriteLine();
            Console.WriteLine("Done mixing");
        }

        public static Task PreheatOvenSync()
        {
            return Task.Run(() =>
            {
                Console.WriteLine("PreheatOvenSync: " + GetThreadId());
                Console.WriteLine("Preheating oven...");
                Thread.Sleep(15000);
                Console.WriteLine("Done preheating");
            });
        }

        public static async Task PreheatOvenSync2()
        {
            Console.WriteLine("PreheatOvenSync: " + GetThreadId());
            Console.WriteLine("Preheating oven...");
            Thread.Sleep(15000);
            Console.WriteLine("Done preheating");
        }

        public static async Task PreheatOvenAsync()
        {
            Console.WriteLine("PreheatOvenAsync: " + GetThreadId());
            Console.WriteLine("Preheating oven...");
            await Task.Delay(15000);
            Console.WriteLine("Done preheating");
        }

        public static async void VoidAsync()
        {
            Thread.Sleep(1000);
        }

        //  1 worker make a cake async
        public static async Task MakeACakeAsync()
        {
            // Get ingredients

            // Preheat Oven (30s)
            Task preheatTask = PreheatOvenAsync(); // good - non-blocking way (async)
            //Task preheatTask = PreheatOvenSync2(); // bad - we said it was non-blocking but it is blocking (sync)

            // If preheatTask isn't really async then it blocks the mixing of ingredients

            // Mix ingredients
            MixIngredients();

            await preheatTask;

            // Bake cake
        }

        static async Task Main(string[] args)
        {
            Console.WriteLine("Start Main: " + GetThreadId());

            // Parallel: Make 3 cakes
            List<Task> cakeTasks = new();
            for (int i = 0; i < 1; i++)
            {
                cakeTasks.Add(MakeACakeAsync());
            }
            await Task.WhenAll(cakeTasks);

            Console.WriteLine("End Main");
        }
        #endregion

        public static void SomeSyncMethod()
        {
            Task t1 = null;
            Task t2 = null;
            Task t3 = null;

            Task.WaitAll(t1, t2, t3);

            Console.WriteLine("DONE");
        }

        public static async Task SomeAsyncMethod()
        {
            Task t1 = null;
            Task t2 = null;
            Task t3 = null;

            await Task.WhenAll(t1, t2, t3);

            Console.WriteLine("DONE");

            SomeVoidAsyncMethod();
            // do something
        }

        public static async Task SomeVoidAsyncMethod()
        {
            await Task.Delay(1000);
        }




        #region Exposing Async and Sync from same method
        public static object GetObject()
        {
            return new object();
        }

        public static async Task<object> GetObjectAsync()
        {
            // await
            return new object();
        }

        public class MyService
        {
            private async Task<object> GetObjectBase(bool isAsync)
            {
                if (isAsync)
                {
                    return await GetObjectAsync();
                }
                else
                {
                    return GetObject();
                }
            }

            public object GetObject()
            {
                return this.GetObjectBase(false).Result;
            }

            public async Task<object> GetObjectAsync()
            {
                return await this.GetObjectBase(true);
            }
        }
        #endregion

        #region Tail Recursion - not related to async/await
        public void TailRecursion(int i)
        {
            if (i <= 0) return;

            TailRecursion(i-1);
        }

        public void TailRecursionOptimized(int i)
        {
            if (i <= 0) return;
            i--;
            if (i <= 0) return;
            i--;
            if (i <= 0) return;
            i--;
            if (i <= 0) return;
            // etc.
        }
        #endregion

        #region Tail Optimization
        public async Task TailAsyncMethod()
        {
            // do something

            await Task.Delay(1000);
        }

        public Task TailAsyncMethod2()
        {
            return Task.Delay(1000);
        }

        public async Task TailAsyncCaller()
        {
            await TailAsyncMethod();
        }
        #endregion
    }
}