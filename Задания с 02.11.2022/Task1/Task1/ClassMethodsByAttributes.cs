namespace MyNUnit;

using System.Reflection;
using System.Collections.Concurrent;

/// <summary>
/// Queues of methods by attributes in each class
/// </summary>
public class ClassMethodsByAttributes
{
    private Type classType;

    /// <summary>
    /// Queue of mwthods with "BeforeClass" attribute
    /// </summary>
    public ConcurrentQueue<MethodInfo> BeforeClassTestMethods { get; private set; } = new ConcurrentQueue<MethodInfo>();

    /// <summary>
    /// Queue of mwthods with "Before" attribute
    /// </summary>
    public ConcurrentQueue<MethodInfo> BeforeTestMethods { get; private set; } = new ConcurrentQueue<MethodInfo>();

    /// <summary>
    /// Queue of mwthods with "Test" attribute
    /// </summary>
    public ConcurrentQueue<MethodInfo> TestMethods { get; private set; } = new ConcurrentQueue<MethodInfo>();

    /// <summary>
    /// Queue of mwthods with "After" attribute
    /// </summary>
    public ConcurrentQueue<MethodInfo> AfterTestMethods { get; private set; } = new ConcurrentQueue<MethodInfo>();

    /// <summary>
    /// Queue of mwthods with "AfterClass" attribute
    /// </summary>
    public ConcurrentQueue<MethodInfo> AfterClassTestMethods { get; private set; } = new ConcurrentQueue<MethodInfo>();

    /// <summary>
    /// Amount of methods to test in this class
    /// </summary>
    public int TestsCount
        => TestMethods.Count;

    public ClassMethodsByAttributes(Type classType)
    {
        this.classType = classType;
        FillQueuesOfMethods();
    }

    /// <summary>
    /// Gets all the methods from the type and fills the queues according to the method's attributes
    /// </summary>
    private void FillQueuesOfMethods()
    {
        Parallel.ForEach(classType.GetMethods(), method =>
        {
            if (method.GetCustomAttribute<TestAttribute>() != null)
            {
                TryToEnqueueMethod(method, TestMethods);
            }
            else if (method.GetCustomAttribute<BeforeClassAttribute>() != null)
            {
                if (!method.IsStatic)
                {
                    throw new FormatException("Methods invoked before testing the class must be static");
                }

                TryToEnqueueMethod(method, BeforeClassTestMethods);
            }
            else if (method.GetCustomAttribute<BeforeAttribute>() != null)
            {
                TryToEnqueueMethod(method, BeforeTestMethods);
            }
            else if (method.GetCustomAttribute<AfterAttribute>() != null)
            {
                TryToEnqueueMethod(method, AfterTestMethods);
            }
            else if (method.GetCustomAttribute<AfterClassAttribute>() != null)
            {
                if (!method.IsStatic)
                {
                    throw new FormatException("Methods invoked after testing the class must be static");
                }

                TryToEnqueueMethod(method, AfterClassTestMethods);
            }
        });
    }

    /// <summary>
    /// Enqueues a method to the queue
    /// </summary>
    private void TryToEnqueueMethod(MethodInfo method, ConcurrentQueue<MethodInfo> queue)
    {
        if (method.GetParameters().Length != 0 || method.ReturnType != typeof(void))
        {
            throw new FormatException("Method shouldn't return value or get parameters");
        }

        queue.Enqueue(method);
    }
}