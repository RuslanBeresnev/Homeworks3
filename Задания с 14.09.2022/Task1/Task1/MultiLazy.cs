namespace ILazy;

/// <summary>
/// Многопоточная реализация ленивых вычислений
/// </summary>
public class MultiLazy<T> : ILazy<T>
{
    private Func<T>? function;
    private T? value;
    private volatile bool valueCalculated = false;
    private Object lockObject = new();

    public MultiLazy(Func<T> function)
    {
        if (function == null)
        {
            throw new ArgumentNullException("В качестве лямбда-функции передана нулевая функция");
        }
        this.function = function;
    }

    public T? Get()
    {
        if (valueCalculated)
        {
            return value;
        }

        lock (lockObject)
        {
            if (!valueCalculated)
            {
                value = function!();
                function = null;
                valueCalculated = true;
            }
            return value;
        }
    }
}