namespace ILazy;

/// <summary>
/// Интерфейс для реализации ленивых вычислений (ха-ха, какие же они ленивые ...)
/// </summary>
public interface ILazy<T>
{
    /// <summary>
    /// Получить значение вычисления
    /// </summary>
    public T? Get();
}