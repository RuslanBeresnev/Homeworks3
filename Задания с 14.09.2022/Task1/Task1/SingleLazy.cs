namespace ILazy;

/// <summary>
/// Однопоточная реализация ленивых вычислений
/// </summary>
public class SingleLazy<T> : ILazy<T>
{
    private Func<T>? function;
    private T? value;
    private bool valueCalculated = false;

    public SingleLazy(Func<T> function)
    {
        if (function == null)
        {
            throw new InvalidOperationException("В качестве лямбда-функции передана нулевая функция");
        }
        this.function = function;
    }

    public T? Get()
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