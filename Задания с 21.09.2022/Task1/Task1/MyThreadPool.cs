using System.Collections.Concurrent;

namespace MyThreadPool;

/// <summary>
/// Реализация пула потоков
/// </summary>
public class MyThreadPool
{
    private Thread[] pool;
    private ConcurrentQueue<Action> tasksForExecution = new();
    private AutoResetEvent autoResetEvent = new AutoResetEvent(false);
    private CancellationTokenSource tokenSource = new CancellationTokenSource();
    private Object lockObject = new Object();

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
                        task();
                    }
                    else
                    {
                        autoResetEvent.WaitOne();
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
        if (tokenSource.IsCancellationRequested)
        {
            throw new InvalidOperationException("Пул потоков закончил свою работу");
        }

        lock (lockObject)
        {
            var newTask = new MyTask<TResult>(function, this);
            tasksForExecution.Enqueue(newTask.CalculateResult);
            autoResetEvent.Set();
            return newTask;
        }
    }

    /// <summary>
    /// Коллаборативное прекращение работы потоков
    /// </summary>
    public void Shutdown()
    {
        tokenSource.Cancel();

        for (int i = 0; i < ThreadsCount; i++)
        {
            autoResetEvent.Set();
        }

        for (int i = 0; i < ThreadsCount; i++)
        {
            pool[i].Join();
        }

        var localLockObject = new Object();
        lock (localLockObject)
        {
            tokenSource.Dispose();
        }
    }
}