namespace MyThreadPool;

/// <summary>
/// Реализация интерфейса IMyTask
/// </summary>
public class MyTask<TResult> : IMyTask<TResult>
{
    private Func<TResult> task;

    public MyTask(Func<TResult> task)
    {
        this.task = task;
    }

    public bool IsComplited { get; set; } = false;
}