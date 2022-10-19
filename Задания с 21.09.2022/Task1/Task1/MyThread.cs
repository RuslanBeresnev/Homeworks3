using System.Threading;

namespace MyThreadPool;

/// <summary>
/// Реализация потока
/// </summary>
public class MyThread<TResult>
{
    private Thread? thread;

    /// <summary>
    /// Исполняет ли задачу поток в данный момент
    /// </summary>
    public bool ThreadIsExecutingTask { get; private set; } = false;

    /// <summary>
    /// Поставить задачу на поток
    /// </summary>
    private void PutTaskOnThread(MyTask<TResult> task)
    {
        ThreadIsExecutingTask = true;
        // pass
    }
}