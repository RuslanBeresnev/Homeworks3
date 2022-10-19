using System.Collections.Generic;

namespace MyThreadPool;


/// <summary>
/// Реализация пула потоков
/// </summary>
public class MyThreadPool
{
    private Queue<Task> tasksForExecution = new Queue<Task>();

    public MyThreadPool(int threadsConunt)
    {
        ThreadsCount = threadsConunt;
        StartThreads();
    }

    /// <summary>
    /// Количество потоков в пуле
    /// </summary>
    public int ThreadsCount { get; private set; }

    /// <summary>
    /// Создать и запустить потоки
    /// </summary>
    private void StartThreads()
    {
        // pass
    }

    public MyTask<TResult> Submit(Func<TResult> function)
    {
        
    }
}