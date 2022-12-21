namespace ControlWork3;

/// <summary>
/// Realization of synchronized priority queue
/// </summary>
public class SynchronizedPriorityQueue<T>
{
    private List<(T, int)> priorityQueue = new();

    /// <summary>
    /// Add new element to synchronized priority queue
    /// </summary>
    public void Enqueue(T value, int priority)
    {
        lock (priorityQueue)
        {
            priorityQueue.Add((value, priority));
            Monitor.Pulse(priorityQueue);
        }
    }

    /// <summary>
    /// Return from synchronized priority queue the element with highest priority if queue is not empty
    /// </summary>
    public T Dequeue()
    {
        lock (priorityQueue)
        {
            while (Size() == 0)
            {
                Monitor.Wait(priorityQueue);
            }

            int maxPriority = Int32.MinValue;
            T result = priorityQueue[0].Item1;

            int currentIndex = 0;
            int elementToRemoveIndex = 0;

            foreach (var (value, priority) in priorityQueue)
            {
                if (priority > maxPriority)
                {
                    maxPriority = priority;
                    result = value;
                    elementToRemoveIndex = currentIndex;
                }
                currentIndex++;
            }

            priorityQueue.RemoveAt(elementToRemoveIndex);
            return result;
        }
    }

    /// <summary>
    /// Get size of synchronized priority queue
    /// </summary>
    public int Size()
    {
        return priorityQueue.Count;
    }
}