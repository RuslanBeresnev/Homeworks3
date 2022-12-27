using System.Diagnostics;

namespace MyThreadPool.Tests;

public class MyThreadPoolTests
{
    private const int THREADS_COUNT = 8;
    private MyThreadPool pool = new(THREADS_COUNT);

    [SetUp]
    public void Setup()
    {
        pool = new MyThreadPool(THREADS_COUNT);
    }

    [Test]
    public void PoolHasSpecifiedNumberOfThreadsTest()
    {
        var tasks = new IMyTask<int>[THREADS_COUNT * 2];
        for (int i = 0; i < THREADS_COUNT * 2; i++)
        {
            tasks[i] = pool.Submit(() =>
            {
                Thread.Sleep(1000);
                return i;
            });
        }

        Assert.That(pool.BusyThreadsCount <= THREADS_COUNT);

        Stopwatch time = new();
        time.Start();
        for (int i = 0; i < THREADS_COUNT * 2; i++)
        {
            var result = tasks[i].Result;
        }
        time.Stop();
        Assert.True(time.Elapsed.TotalMilliseconds < 2100);
    }

    [Test]
    public void SubmitMethodTest()
    {
        var tasks = new IMyTask<int>[10];
        for (int i = 0; i < 10; i++)
        {
            tasks[i] = pool.Submit(() => i);
            Assert.That(i == tasks[i].Result);
        }
    }

    [Test]
    public void ContinueWithMethodTest()
    {
        var tasks = new IMyTask<string>[10];
        for (int i = 0; i < 10; i++)
        {
            tasks[i] = pool.Submit(() => i).ContinueWith(x => x.ToString());
            Assert.That(i.ToString() == tasks[i].Result);
        }
    }

    [Test]
    public void SeveralContinuationsTest()
    {
        var tasks = new IMyTask<string>[10];
        for (int i = 0; i < 10; i++)
        {
            tasks[i] = pool.Submit(() => i).ContinueWith(x => x.ToString()).ContinueWith(x => "i" + x);
            Assert.That("i" + i.ToString() == tasks[i].Result);
        }
    }

    [Test]
    public void ShutdownMethodTest()
    {
        var task = pool.Submit(() => 0);
        pool.Shutdown();
        Assert.Throws<InvalidOperationException>(() => pool.Submit(() => 0));
    }

    [Test]
    public void ShutdownMethodOnContinuationTest()
    {
        var task = pool.Submit(() => 0);
        pool.Shutdown();
        Assert.Throws<InvalidOperationException>(() => task.ContinueWith(x => x * 2));
    }

    [Test]
    public void TaskWithExceptionSubmittingTest()
    {
        var task = pool.Submit(() =>
        {
            throw new Exception();
            return 0;
        });

        var result = 0;
        Assert.Throws<AggregateException>(() => result = task.Result);
    }
}