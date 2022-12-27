using System.Collections.Concurrent;

namespace MyThreadPool;

/// <summary>
/// Реализация пула потоков
/// </summary>
public class MyThreadPool
{
    /// <summary>
    /// Реализация интерфейса IMyTask
    /// </summary>
    private class MyTask<TResult> : IMyTask<TResult>
    {
        private Func<TResult> function;
        private MyThreadPool threadPool;

        private ManualResetEvent manualResetEvent = new(false);
        private Object lockObject = new();
        private Exception? catchedException;
        private TResult? result;

        public MyTask(Func<TResult>? function, MyThreadPool threadPool)
        {
            if (function == null || threadPool == null)
            {
                throw new ArgumentNullException();
            }

            this.function = function;
            this.threadPool = threadPool;
        }

        public bool IsCompleted { get; private set; } = false;

        public TResult? Result
        {
            get
            {
                if (!IsCompleted)
                {
                    manualResetEvent.WaitOne();

                    if (catchedException != null)
                    {
                        throw new AggregateException(catchedException);
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Посчитать функцию
        /// </summary>
        public void CalculateResult()
        {
            try
            {
                result = function();
            }
            catch (Exception exception)
            {
                catchedException = new AggregateException(exception);
            }
            finally
            {
                IsCompleted = true;
                Interlocked.Decrement(ref threadPool.busyThreadsCount);
                manualResetEvent.Set();
            }
        }

        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult?, TNewResult> function)
        {
            if (function == null)
            {
                throw new ArgumentNullException();
            }

            return threadPool.Submit(() => function(Result));
        }
    }

    private Thread[] pool;
    private ConcurrentQueue<Action> tasksForExecution = new();
    private AutoResetEvent autoResetEvent = new(false);
    private CancellationTokenSource tokenSource = new();
    private Object lockObject = new();
    private int busyThreadsCount = 0;

    public MyThreadPool(int threadsCount)
    {
        if (threadsCount <= 0)
        {
            throw new InvalidDataException("Количество потоков должно быть > 0");
        }

        ThreadsCount = threadsCount;
        pool = new Thread[ThreadsCount];
        CreateAndStartThreads();
    }

    /// <summary>
    /// Количество потоков в пуле
    /// </summary>
    public int ThreadsCount { get; private set; }

    /// <summary>
    /// Количество занятых потоков
    /// </summary>
    public int BusyThreadsCount => busyThreadsCount;

    /// <summary>
    /// Создать потоки и запустить их
    /// </summary>
    private void CreateAndStartThreads()
    {
        for (int i = 0; i < ThreadsCount; i++)
        {
            pool[i] = new Thread(() =>
            {
                while (!tokenSource.IsCancellationRequested)
                {
                    if (tasksForExecution.TryDequeue(out var task))
                    {
                        if (!tasksForExecution.IsEmpty || tokenSource.IsCancellationRequested)
                        {
                            autoResetEvent.Set();
                        }
                        Interlocked.Increment(ref busyThreadsCount);
                        task();
                    }
                    else
                    {
                        autoResetEvent.WaitOne();
                        if (tokenSource.IsCancellationRequested)
                        {
                            autoResetEvent.Set();
                        }
                    }
                }
            });
        }

        for (int i = 0; i < ThreadsCount; i++)
        {
            pool[i].Start();
        }
    }

    /// <summary>
    /// Поставить выполнение задачи на один из свободных потоков
    /// </summary>
    public IMyTask<TResult> Submit<TResult>(Func<TResult> function)
    {
        lock (lockObject)
        {
            var newTask = new MyTask<TResult>(function, this);
            tasksForExecution.Enqueue(newTask.CalculateResult);
            autoResetEvent.Set();

            if (tokenSource.IsCancellationRequested)
            {
                throw new InvalidOperationException("Пул потоков закончил свою работу");
            }

            return newTask;
        }
    }

    /// <summary>
    /// Коллаборативное прекращение работы потоков
    /// </summary>
    public void Shutdown()
    {
        lock (lockObject)
        {
            tokenSource.Cancel();
        }

        autoResetEvent.Set();

        for (int i = 0; i < ThreadsCount; i++)
        {
            pool[i].Join();
        }

        lock (lockObject)
        {
            tokenSource.Dispose();
        }
    }
}