namespace ControlWork3.Tests;

public class SynchronizedQueueTests
{
    private SynchronizedPriorityQueue<int> queue;

    [SetUp]
    public void Setup()
    {
        queue = new SynchronizedPriorityQueue<int>();
    }

    [Test]
    public async Task CheckCorrectnessOfDequeueMethodFromEmptyQueue()
    {
        int result = 0;
        var dequeueTask = Task.Run(() => result = queue.Dequeue());
        await Task.Run(() => queue.Enqueue(100, 1));

        await dequeueTask;
        Assert.AreEqual(100, result);
    }

    [Test]
    public async Task RaceConditionAbsence()
    {
        var taskEnqueue1 = Task.Run(() => queue.Enqueue(101, 1));
        var taskEnqueue2 = Task.Run(() => queue.Enqueue(102, 2));
        var taskEnqueue3 = Task.Run(() => queue.Enqueue(103, 3));
        var taskEnqueue4 = Task.Run(() => queue.Enqueue(104, 4));
        var taskEnqueue5 = Task.Run(() => queue.Enqueue(105, 5));

        await taskEnqueue1;
        await taskEnqueue2;
        await taskEnqueue3;
        await taskEnqueue4;
        await taskEnqueue5;

        Assert.AreEqual(5, queue.Size());

        var results = new List<int>();

        var dequeueTask1 = Task.Run(() => results.Add(queue.Dequeue()));
        var dequeueTask2 = Task.Run(() => results.Add(queue.Dequeue()));
        var dequeueTask3 = Task.Run(() => results.Add(queue.Dequeue()));
        var dequeueTask4 = Task.Run(() => results.Add(queue.Dequeue()));
        var dequeueTask5 = Task.Run(() => results.Add(queue.Dequeue()));

        await dequeueTask1;
        await dequeueTask2;
        await dequeueTask3;
        await dequeueTask4;
        await dequeueTask5;

        for (int i = 101; i <= 105; i++)
        {
            Assert.True(results.Contains(i));
        }
    }
}