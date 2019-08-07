using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
namespace ConcurrencyDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region DeadlockDemo
        //UI method
        private void Deadlock_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine($"uiThread[{Thread.CurrentThread.ManagedThreadId}]");

            //1. The UI method calls the Outer async method(in the UI context).
            DoNothingTask().Wait(); //4. The UI method synchronously blocks (.Wait()) on the Task returned by the Outer async method. This blocks the context (UI) thread. 
        }

        // Starting a task on thread that is not the calling thread will also avoid the deadlock. 
        private void No_Deadlock_Background_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine($"uiThread[{Thread.CurrentThread.ManagedThreadId}]");
            Task.Run(() => DoNothingTask()).Wait();
        }

        private void No_Deadlock_Configure_Await_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine($"uiThread[{Thread.CurrentThread.ManagedThreadId}]");
            DoNothingTaskConfigureAwaitFalse().Wait();
        }

        //Outter async method
        private async Task DoNothingTask()
        {
            Console.WriteLine($"taskThread[{Thread.CurrentThread.ManagedThreadId}]");

            // 2.The Outer async method calls another Inner async method (still within the context), which returns an uncompleted Task.
            // 3. The Outer async method awaits for the Task returned by Inner async method to complete.The context(UI) is captured for use by the continuation of the Outer async method later.The outer async method returns an uncompleted Task to the UI method.

            await Task.Delay(new Random(Thread.CurrentThread.ManagedThreadId).Next((int)1E2, (int)1E4)); //Inner async method

            // 5. Eventually, the Inner async method completes. 
            // 6.T he continuation for Outer async method is now ready to run, and it waits for the context to be available so it can execute in the context.
            // 7. Deadlock. The UI method is blocking the context thread waiting for Outer async method to complete, while the Outer async method is itself waiting for the context to be available so it can execute its continuation.

            //continuation here
        }

        // Configure await false tells the continuation that it doesn't have to use the calling context, 
        // making it such that the Outer async method doesn't need to wait for that context to be available to execute its continuation.
        private async Task DoNothingTaskConfigureAwaitFalse()
        {
            Console.WriteLine($"taskThread[{Thread.CurrentThread.ManagedThreadId}]");
            await Task.Delay(new Random(Thread.CurrentThread.ManagedThreadId).Next((int)1E2, (int)1E4)).ConfigureAwait(false);
        }
        #endregion

        #region ParallelismDemo

        private void Data_Parallel_Class_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine($"uiThread[{Thread.CurrentThread.ManagedThreadId}]");

            var inputs = GetRandomPositiveNumbers(100, 1E10);
            Parallel.ForEach(inputs, i =>
            {
                var sqrt = Math.Sqrt(i);
                var threadId = Thread.CurrentThread.ManagedThreadId;

                Console.WriteLine($"thread[{Thread.CurrentThread.ManagedThreadId}] {i} => {sqrt}");
            });
        }

        private void Data_PLINQ_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine($"uiThread[{Thread.CurrentThread.ManagedThreadId}]");
            GetRandomPositiveNumbers(100, 1E10).AsParallel().ForAll(
                i =>
                {
                    var sqrt = Math.Sqrt(i);
                    var threadId = Thread.CurrentThread.ManagedThreadId;

                    Console.WriteLine($"thread[{Thread.CurrentThread.ManagedThreadId}] {i} => {sqrt}");
                });
        }

        private void Task_ParallelTask_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine($"uiThread[{Thread.CurrentThread.ManagedThreadId}]");

            void GetActionThatTakesRandomAmountOfTime(int order)
            {
                Console.WriteLine($"action[{order}] - thread[{Thread.CurrentThread.ManagedThreadId}] - startTime[{DateTime.Now.TimeOfDay}]");
                Thread.Sleep(new Random(Thread.CurrentThread.ManagedThreadId).Next((int)1E2, (int)1E4));
                Console.WriteLine($"action[{order}] - thread[{Thread.CurrentThread.ManagedThreadId}] - endTime[{DateTime.Now.TimeOfDay}]");
            }

            Parallel.Invoke(
                 () => GetActionThatTakesRandomAmountOfTime(0),
                  () => GetActionThatTakesRandomAmountOfTime(1),
                 () => GetActionThatTakesRandomAmountOfTime(2),
                  () => GetActionThatTakesRandomAmountOfTime(3),
                  () => GetActionThatTakesRandomAmountOfTime(4),
                 () => GetActionThatTakesRandomAmountOfTime(5),
                 () => GetActionThatTakesRandomAmountOfTime(6)
                );
        }


        private static List<double> GetRandomPositiveNumbers(int count, double max)
        {
            var numbers = new List<double>();
            for (int i = 0; i < count; i++)
            {
                numbers.Add(new Random(i).NextDouble() * max);
            }

            return numbers;
        }

        #endregion

        #region Lock Demo
        private void Race_Example(object sender, RoutedEventArgs e)
        {
            int sharedResource;
            int expectedResourceTotal = 100;
            int maxIterations = 100000;
            int iterations = 1;

            void ModifyMySharedResource()
            {
                // Console.WriteLine($"thread[{Thread.CurrentThread.ManagedThreadId}]");
                sharedResource++;
            }

            do
            {
                sharedResource = 0;

                var actions = new List<Action>();

                for (int i = 0; i < expectedResourceTotal; i++)
                {
                    actions.Add(ModifyMySharedResource);
                }
                Parallel.Invoke(actions.ToArray());

                iterations++;
            } while (sharedResource == expectedResourceTotal && iterations < maxIterations);

            Console.WriteLine($"uiThread[{Thread.CurrentThread.ManagedThreadId}] - sharedResourceTotal[{sharedResource}] - numberOfIterations[{iterations}]");
        }

        object exampleLock = new object();
        private void Lock_Example(object sender, RoutedEventArgs e)
        {
            int sharedResource;
            int expectedResourceTotal = 100;
            int maxIterations = 100000;
            int iterations = 1;

            void ModifyMySharedResource()
            {
                // Console.WriteLine($"thread[{Thread.CurrentThread.ManagedThreadId}]");
                lock (exampleLock)
                {
                    sharedResource++;
                }
            }

            do
            {
                sharedResource = 0;

                var actions = new List<Action>();

                for (int i = 0; i < expectedResourceTotal; i++)
                {
                    actions.Add(ModifyMySharedResource);
                }
                Parallel.Invoke(actions.ToArray());

                iterations++;
            } while (sharedResource == expectedResourceTotal && iterations < maxIterations);

            Console.WriteLine($"uiThread[{Thread.CurrentThread.ManagedThreadId}] - sharedResourceTotal[{sharedResource}] - numberOfIterations[{iterations}]");
        }

        #endregion

        #region Asynchrony Demo

        object badLock = new object();
        //when all
        private void When_All(object sender, RoutedEventArgs e)
        {
            int sharedResource;
            int expectedResourceTotal = 100;
            int maxIterations = 10000;
            int iterations = 1;

            Task DoNothingAsync()
            {
                return Task.Run(async () =>
                {
                    //Console.WriteLine($"thread[{Thread.CurrentThread.ManagedThreadId}]");
                    //lock(badLock){                     
                    await Task.Delay(0);
                    sharedResource++;
                    // }                     
                });
            }

            do
            {
                sharedResource = 0;

                var tasks = new List<Task>();

                for (int i = 0; i < expectedResourceTotal; i++)
                {
                    tasks.Add(DoNothingAsync());
                }

                Task.WhenAll(tasks).Wait();

                iterations++;
            } while (sharedResource == expectedResourceTotal && iterations < maxIterations);

            Console.WriteLine($"uiThread[{Thread.CurrentThread.ManagedThreadId}] - sharedResourceTotal[{sharedResource}] - numberOfIterations[{iterations}]");
        }

        //semaphore demo
        private void When_All_Semaphore_Slim(object sender, RoutedEventArgs e)
        {
            Console.WriteLine($"uiThread[{Thread.CurrentThread.ManagedThreadId}]");

            var semaphore = new SemaphoreSlim(1, 1);
            int sharedResource;
            int expectedResourceTotal = 100;
            int maxIterations = 10000;
            int iterations = 1;

            Task DoNothingAsync()
            {
                return Task.Run(async () =>
                {
                    //Console.WriteLine($"thread[{Thread.CurrentThread.ManagedThreadId}]");
                    await Task.Delay(0);

                    await semaphore.WaitAsync();
                    sharedResource++;
                    semaphore.Release();
                });
            }

            do
            {
                sharedResource = 0;

                var tasks = new List<Task>();

                for (int i = 0; i < expectedResourceTotal; i++)
                {
                    tasks.Add(DoNothingAsync());
                }

                Task.WhenAll(tasks).Wait();

                iterations++;
            } while (sharedResource == expectedResourceTotal && iterations < maxIterations);

            Console.WriteLine($"uiThread[{Thread.CurrentThread.ManagedThreadId}] - sharedResourceTotal[{sharedResource}] - numberOfIterations[{iterations}]");
        }
        #endregion
    }
}
