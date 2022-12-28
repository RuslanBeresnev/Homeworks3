using System.Diagnostics;

namespace ILazy.Tests;

public class ILazyTests
{
    private static Stopwatch time = new();

    [TestCaseSource(nameof(LazyEvaluationsForStandardCase))]
    public void StandardCaseTest(ILazy<int> realization)
    {
        time.Restart();
        var firstValue = realization.Get();
        time.Stop();
        var firstGetCallDuration = (float)time.Elapsed.TotalSeconds;

        time.Restart();
        var secondValue = realization.Get();
        time.Stop();
        var secondGetCallDuration = (float)time.Elapsed.TotalSeconds;

        Assert.AreEqual(1, firstValue);
        Assert.AreEqual(1, secondValue);
        Assert.IsTrue(firstGetCallDuration > 2f);
        Assert.IsTrue(secondGetCallDuration < 0.1f);
    }

    [TestCaseSource(nameof(LazyEvaluationsForNullValueReturnedCase))]
    public void NullValueReturnedCaseTest(ILazy<Object?> realization)
    {
        Assert.IsNull(realization.Get());
    }

    [Test]
    public void RacingCaseTest()
    {
        var lazyEvaluation = new MultiLazy<int>(new Func<int>(FunctionWithDelay));
        var threads = new Thread[8];

        for (var i = 0; i < threads.Length; i++)
        {
            threads[i] = new Thread(() =>
            {
                Assert.AreEqual(1, lazyEvaluation.Get());

            });
        }

        var numberOfThreadsWithLongEvaluation = 0;
        foreach (var thread in threads)
        {
            thread.Start();
        }
        foreach (var thread in threads)
        {
            time.Restart();
            thread.Join();
            time.Stop();
            if ((float)time.Elapsed.TotalSeconds > 2f)
            {
                numberOfThreadsWithLongEvaluation++;
            }
        }

        // Ёто проверка на то, что только один из потоков начал вычисл€ть функцию (проверка на отсутствие "гонок")
        Assert.AreEqual(1, numberOfThreadsWithLongEvaluation);
    }

    private static int FunctionWithDelay()
    {
        Thread.Sleep(2000);
        return 1;
    }

    private static IEnumerable<TestCaseData> LazyEvaluationsForStandardCase
        => new TestCaseData[]
        {
            new TestCaseData(new SingleLazy<int>(new Func<int>(FunctionWithDelay))),
            new TestCaseData(new MultiLazy<int>(new Func<int>(FunctionWithDelay)))
        };

    private static IEnumerable<TestCaseData> LazyEvaluationsForNullValueReturnedCase
    => new TestCaseData[]
    {
            new TestCaseData(new SingleLazy<Object?>(() => null)),
            new TestCaseData(new MultiLazy<Object?>(() => null))
    };
}