namespace MyThreadPool;

/// <summary>
/// Реализация задачи для вычисления в потоке пула
/// </summary>
public interface IMyTask<TResult>
{
    /// <summary>
    /// Выполнена ли задача на данный момент
    /// </summary>
    public bool IsComplited { get; set; }

    /// <summary>
    /// Результат вычисленной задачи
    /// </summary>
    public TResult Result { get; protected set; }

    /// <summary>
    /// Создать новую задачу, которая принимает результат текущей
    /// </summary>
    public IMyTask<TResult> ContinueWith(Func<TResult, TNewResult> task);
}