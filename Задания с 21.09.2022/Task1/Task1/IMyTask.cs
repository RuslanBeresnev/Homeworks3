namespace MyThreadPool;

/// <summary>
/// Реализация задачи для вычисления в потоке пула
/// </summary>
public interface IMyTask<TResult>
{
    /// <summary>
    /// Выполнена ли задача на данный момент
    /// </summary>
    public bool IsComplited { get; }

    /// <summary>
    /// Результат вычисленной задачи
    /// </summary>
    public TResult Result { get; }

    /// <summary>
    /// Создать новую задачу, которая принимает результат текущей
    /// </summary>
    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> func);
}