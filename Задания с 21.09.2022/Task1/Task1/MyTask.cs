namespace MyThreadPool;

/// <summary>
/// Реализация интерфейса IMyTask
/// </summary>
public class MyTask<TResult> : IMyTask<TResult>
{
    private Func<TResult> function;
    private MyThreadPool threadPool;

    private ManualResetEvent manualResetEvent = new ManualResetEvent(false);
    private Object lockObject = new Object();
    private Exception? catchedException;
    private TResult result;

    public MyTask(Func<TResult>? function, MyThreadPool threadPool)
    {
        if (function == null || threadPool == null)
        {
            throw new ArgumentNullException();
        }

        this.function = function;
        this.threadPool = threadPool;
    }

    public bool IsComplited { get; set; } = false;

    public TResult Result
    {
        get
        {
            if (!IsComplited)
            {
                manualResetEvent.WaitOne();

                if (catchedException != null)
                {
                    throw new AggregateException(catchedException);
                }
            }

            return result;
        }
        private set
        {
            result = value;
        }
    }

    /// <summary>
    /// Посчитать функцию
    /// </summary>
    public void CalculateResult()
    {
        try
        {
            Result = function();
            IsComplited = true;
            manualResetEvent.Set();
        }
        catch (Exception exception)
        {
            catchedException = new AggregateException(exception);
        }
    }

    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> function)
    {
        if (function == null)
        {
            throw new ArgumentNullException();
        }

        return threadPool.Submit(() => function(Result));
    }
}